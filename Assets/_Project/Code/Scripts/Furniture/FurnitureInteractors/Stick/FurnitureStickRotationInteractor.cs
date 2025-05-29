using UnityEngine;
using Meta.XR.MRUtilityKit;

public class FurnitureStickRotationInteractor : MonoBehaviour
{
    [SerializeField] private Furniture furniture;
    [SerializeField] private float rotationStep = 5f;
    [SerializeField] private float rotationCooldown = 0.25f;

    private MRUKAnchor.SceneLabels sceneLabel;
    private float lastRotationTime = -Mathf.Infinity;

    void Start()
    {
        sceneLabel = furniture.GetSceneLabel();
    }

    public bool Rotate()
    {
        if (Time.time - lastRotationTime < rotationCooldown) return true;

        Vector2 input = ControllerManager.Instance.GetPrimaryControllerStickInput();
        if (input == Vector2.zero) return true;

        Vector2 direction = GetDirection(input);
        if (direction == Vector2.zero) return true;

        Vector3 localAxis = GetRotationAxis(sceneLabel);
        float angle = direction.x != 0 ? rotationStep * Mathf.Sign(direction.x) : rotationStep * Mathf.Sign(direction.y);

        Quaternion targetRotation = Quaternion.AngleAxis(angle, furniture.transform.TransformDirection(localAxis)) * furniture.transform.rotation;

        bool wouldCollide = WouldCollideAtRotation(targetRotation, localAxis);
        if (wouldCollide)
        {
            SoundManager.Instance.PlayDeleteClip();
            return false;
        }

        furniture.transform.rotation = targetRotation;
        SoundManager.Instance.PlayPressClip();
        lastRotationTime = Time.time;

        return true;
    }

    private Vector2 GetDirection(Vector2 input)
    {
        const float threshold = 0.9f;
        if (input.magnitude < threshold) return Vector2.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return new Vector2(Mathf.Sign(input.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(input.y));
    }

    private Vector3 GetRotationAxis(MRUKAnchor.SceneLabels label)
    {
        return label switch
        {
            MRUKAnchor.SceneLabels.FLOOR => Vector3.up,
            MRUKAnchor.SceneLabels.CEILING => Vector3.up,
            MRUKAnchor.SceneLabels.WALL_FACE => Vector3.forward,
            _ => Vector3.up
        };
    }

    private bool WouldCollideAtRotation(Quaternion targetRotation, Vector3 localAxis)
    {
        Collider furnitureCollider = furniture.GetModelCollider();
        if (furnitureCollider == null) return false;

        Bounds bounds = furnitureCollider.bounds;
        Vector3 halfExtents = bounds.extents;
        Vector3 center = furniture.transform.position + (bounds.center - furniture.transform.position);

        int mask = ~LayerMask.GetMask("UI", "Gizmo");

        Collider[] hits = Physics.OverlapBox(center, halfExtents, targetRotation, mask);

        foreach (var hit in hits)
        {
            if (hit.transform.IsChildOf(furniture.transform)) continue;

            var anchor = hit.GetComponentInParent<MRUKAnchor>();
            if (anchor != null && anchor.Label.HasFlag(sceneLabel)) continue;

            // No fallback de raycast como en movimiento porque no tiene sentido empujar rotación "justo antes" del contacto
            return true;
        }

        return false;
    }
}