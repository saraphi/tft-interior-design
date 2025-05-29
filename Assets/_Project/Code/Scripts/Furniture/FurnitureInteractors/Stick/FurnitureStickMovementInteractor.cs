using UnityEngine;
using Meta.XR.MRUtilityKit;

public class FurnitureStickMovementInteractor : MonoBehaviour
{
    [SerializeField] private Furniture furniture;
    [SerializeField] private float movementStep = 0.05f;
    [SerializeField] private float movementCooldown = 0.25f;

    private MRUKAnchor.SceneLabels sceneLabel;
    private float lastMoveTime = -Mathf.Infinity;

    void Start()
    {
        sceneLabel = furniture.GetSceneLabel();
    }

    public bool Move()
    {
        if (Time.time - lastMoveTime < movementCooldown) return true;

        Vector2 input = ControllerManager.Instance.GetPrimaryControllerStickInput();

        if (input == Vector2.zero) return true;

        Vector3 localDirection = GetDirection(input);

        if (localDirection == Vector3.zero) return true;

        Vector3 step = localDirection * movementStep;
        Vector3 targetPosition = furniture.transform.position + step;

        bool wouldCollide = WouldCollideAtPosition(targetPosition, localDirection);
        if (wouldCollide)
        {
            SoundManager.Instance.PlayDeleteClip();
            return false;
        }

        furniture.transform.position = targetPosition;
        SoundManager.Instance.PlayPressClip();
        lastMoveTime = Time.time;

        return true;
    }

    private Vector3 GetDirection(Vector2 input)
    {
        const float threshold = 0.9f;
        Vector2 direction2D = Vector2.zero;

        if (input.magnitude >= threshold)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                direction2D = new Vector2(Mathf.Sign(input.x), 0f);
            else
                direction2D = new Vector2(0f, Mathf.Sign(input.y));
        }
        else return direction2D;

        return sceneLabel switch
        {
            MRUKAnchor.SceneLabels.FLOOR => new Vector3(direction2D.x, 0f, direction2D.y),
            MRUKAnchor.SceneLabels.CEILING => new Vector3(direction2D.x, 0f, direction2D.y),
            MRUKAnchor.SceneLabels.WALL_FACE => new Vector3(direction2D.x, direction2D.y, 0f),
            _ => Vector3.zero
        };
    }

    private bool WouldCollideAtPosition(Vector3 targetPosition, Vector3 localDirection)
    {
        Collider furnitureCollider = furniture.GetModelCollider();
        if (furnitureCollider == null) return false;

        Bounds bounds = furnitureCollider.bounds;
        Vector3 halfExtents = bounds.extents;
        Vector3 center = targetPosition + (bounds.center - furniture.transform.position);

        int mask = ~LayerMask.GetMask("UI", "Gizmo");

        Collider[] hits = Physics.OverlapBox(center, halfExtents, furniture.transform.rotation, mask);

        foreach (var hit in hits)
        {
            if (hit.transform.IsChildOf(furniture.transform)) continue;

            var anchor = hit.GetComponentInParent<MRUKAnchor>();
            if (anchor != null && anchor.Label.HasFlag(sceneLabel)) continue;

            if (Physics.Raycast(furniture.transform.position, localDirection, out RaycastHit hitInfo, movementStep, mask))
            {
                Vector3 safePosition = hitInfo.point - localDirection * 0.001f; 
                furniture.transform.position = safePosition;
            }

            return true;
        }

        return false;
    }
}