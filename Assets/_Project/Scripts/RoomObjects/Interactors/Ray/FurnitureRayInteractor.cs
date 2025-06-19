using UnityEngine;
using Meta.XR.MRUtilityKit;
public class FurnitureRayInteractor : RayInteractor
{
    [SerializeField] private Furniture furniture;

    private MRUKAnchor.SceneLabels sceneLabel;

    void Awake()
    {
        rb = furniture.GetComponent<Rigidbody>();
        sceneLabel = furniture.GetSceneLabel();
    }

    public override bool Move()
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

    protected override Quaternion GetAddiotionalRotation()
    {
        Vector2 joystickInput = ControllerManager.Instance.GetSecondaryControllerJoystickInput();

        if (Mathf.Abs(joystickInput.x) > 0.9f)
            currentRotation += joystickInput.x * rotationDegrees;

        Vector3 rotationAxis = sceneLabel == MRUKAnchor.SceneLabels.WALL_FACE ? Vector3.forward : Vector3.up;

        return Quaternion.AngleAxis(currentRotation, rotationAxis);
    }
}