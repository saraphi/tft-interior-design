using UnityEngine;
using Meta.XR.MRUtilityKit;
public class FurnitureRayInteractor : MonoBehaviour
{
    [SerializeField] private Furniture furniture;
    [SerializeField] public float maxRayLength = 2f;
    [SerializeField] private float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private bool hasValidPosition = false;
    private bool wouldCollide = false;

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
            Vector3 closestPoint = furnitureModel.GetClosestPointToDirection(GetDominantAxisDirection(hit.normal));
            Vector3 offset = hit.point - closestPoint;
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

        rb.MovePosition(Vector3.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
        rb.rotation = targetRotation;

        wouldCollide = furniture.WouldCollide(targetPosition, targetRotation);

        return hasValidPosition && !wouldCollide;
    }

    private Vector3 GetDominantAxisDirection(Vector3 normal)
    {
        if (normal.x > normal.y && normal.x > normal.z) return new Vector3(Mathf.Sign(normal.x), 0f, 0f);
        else if (normal.y > normal.z) return new Vector3(0f, Mathf.Sign(normal.y), 0f);
        else return new Vector3(0f, 0f, Mathf.Sign(normal.z));
    }
}