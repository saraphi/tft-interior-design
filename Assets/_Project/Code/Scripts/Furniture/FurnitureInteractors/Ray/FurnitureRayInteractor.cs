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

    private MRUKAnchor.SceneLabels sceneLabels;
    private Collider furnitureCollider;

    void Awake()
    {
        rb = furniture.GetComponent<Rigidbody>();
        sceneLabels = furniture.GetSceneLabel();
        furnitureCollider = furniture.GetModelCollider();
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

        if (room.Raycast(ray, Mathf.Infinity, new LabelFilter(sceneLabels), out RaycastHit hit))
        {
            targetPosition = hit.point;

            if (sceneLabels == MRUKAnchor.SceneLabels.WALL_FACE)
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

        wouldCollide = WouldCollideAtPosition(targetPosition);

        return hasValidPosition && !wouldCollide;
    }

    private bool WouldCollideAtPosition(Vector3 targetPosition)
    {
        if (furnitureCollider == null) return false;

        Bounds bounds = furnitureCollider.bounds;
        Vector3 halfExtents = bounds.extents;
        Vector3 center = targetPosition + (bounds.center - furniture.transform.position);

        int collisionMask = ~LayerMask.GetMask("UI", "Gizmo");

        Collider[] overlaps = Physics.OverlapBox(center, halfExtents, furniture.transform.rotation, collisionMask);

        foreach (var hit in overlaps)
        {
            if (hit == null || hit.transform.IsChildOf(furniture.transform)) continue;

            var anchor = hit.GetComponentInParent<MRUKAnchor>();
            if (anchor != null && anchor.Label.HasFlag(sceneLabels))
                continue;

            return true;
        }

        return false;
    }

}