using UnityEngine;
using Meta.XR.MRUtilityKit;
public class FurnitureRayInteractor : MonoBehaviour
{
    [SerializeField] private Furniture furniture;
    [SerializeField] public float maxRayLength = 2f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float rotationDegrees = 0.9f;

    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private bool hasValidPosition = false;
    private bool wouldCollide = false;
    private float currentRotation = 0f;

    private MRUKAnchor.SceneLabels sceneLabel;
    private FurnitureModel furnitureModel;

    void Awake()
    {
        rb = furniture.GetComponent<Rigidbody>();
        sceneLabel = furniture.GetSceneLabel();
        furnitureModel = furniture.GetFurnitureModel();
    }

    public bool Move()
    {
        Vector3 controllerPosition = ControllerManager.Instance.GetRayControllerLocalPosition();
        Quaternion controllerRotation = ControllerManager.Instance.GetRayControllerLocalRotation();

        Vector3 forwardDirection = controllerRotation * Vector3.forward;
        Vector3 targetPosition = controllerPosition + forwardDirection * maxRayLength;
        Quaternion targetRotation = Quaternion.identity;

        Ray ray = new Ray(controllerPosition, forwardDirection);
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();

        if (room.Raycast(ray, Mathf.Infinity, new LabelFilter(sceneLabel), out RaycastHit hit))
        {
            Vector3 insertionPointWorld = furniture.GetInsertionPoint();
            Vector3 offset = hit.point - insertionPointWorld;
            targetPosition = rb.position + offset;

            if (sceneLabel == MRUKAnchor.SceneLabels.WALL_FACE)
                targetRotation = Quaternion.LookRotation(-hit.normal);

            lastValidPosition = targetPosition;
            lastValidRotation = targetRotation;
            hasValidPosition = true;
        }
        else if (hasValidPosition)
        {
            targetPosition = lastValidPosition;
            targetRotation = lastValidRotation;
        }

        Quaternion additionalRotation = GetAddiotionalRotation();
        targetRotation *= additionalRotation;

        rb.MovePosition(Vector3.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
        rb.rotation = targetRotation;

        wouldCollide = furniture.WouldCollide(targetPosition, targetRotation);

        return hasValidPosition && !wouldCollide;
    }

    private Quaternion GetAddiotionalRotation()
    {
        Vector2 joystickInput = ControllerManager.Instance.GetSecondaryControllerJoystickInput();

        if (Mathf.Abs(joystickInput.x) > 0.9f)
            currentRotation += joystickInput.x * rotationDegrees;

        Vector3 rotationAxis = sceneLabel == MRUKAnchor.SceneLabels.WALL_FACE ? furniture.transform.forward : Vector3.up;

        return Quaternion.AngleAxis(currentRotation, rotationAxis);
    }
}