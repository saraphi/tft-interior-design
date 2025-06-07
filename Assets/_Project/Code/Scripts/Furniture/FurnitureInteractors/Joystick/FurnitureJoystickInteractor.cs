using Meta.XR.MRUtilityKit;
using UnityEngine;

public class FurnitureJoystickInteractor : MonoBehaviour
{
    [SerializeField] private Furniture furniture;
    [SerializeField] private float cooldown = 0.25f;

    [Header("Movement")]
    [SerializeField] private FurnitureGizmo movementGizmo;
    [SerializeField] private float movementStep = 0.05f;

    [Header("Rotation")]
    [SerializeField] private FurnitureGizmo rotationGizmo;
    [SerializeField] private float rotationStep = 5f;

    private MRUKAnchor.SceneLabels sceneLabel;
    private Collider furnitureModelCollider;
    private bool hasValidPosition = false;
    private float lastMoveTime = -Mathf.Infinity;
    private float threshold = 0.9f;
    private int mask;

    void Start()
    {
        sceneLabel = furniture.GetSceneLabel();
        FurnitureModel furnitureModel = furniture.GetFurnitureModel();
        furnitureModelCollider = furnitureModel.GetCollider();
        mask = ~LayerMask.GetMask("UI", "Gizmo");
    }

    public bool Move()
    {
        if (Time.time - lastMoveTime < cooldown) return true;

        movementGizmo.DeactiveAllDirections();
        rotationGizmo.DeactiveAllDirections();

        Vector2 movementInput = ControllerManager.Instance.GetPrimaryControllerJoystickInput();
        Vector2 rotationInput = ControllerManager.Instance.GetSecondaryControllerJoystickInput();

        if (movementInput.magnitude >= threshold) hasValidPosition = HandleJoystickMovement(movementInput);
        else if (rotationInput.magnitude >= threshold) hasValidPosition = HandleJoystickRotation(rotationInput);
        else return true;

        if (hasValidPosition) SoundManager.Instance.PlayPressClip();
        else SoundManager.Instance.PlayDeleteClip();

        lastMoveTime = Time.time;

        return hasValidPosition;
    }

    private bool HandleJoystickMovement(Vector2 input)
    {
        Vector2 direction = GetDirection(input);
        Vector3 localDirection = GetLocalDirection(direction);

        movementGizmo.UseDirection(GetGizmoDirection(localDirection), localDirection);

        Vector3 worldDirection = furniture.transform.TransformDirection(localDirection);
        Vector3 targetPosition = furniture.transform.position + worldDirection * movementStep;

        if (WouldCollide(targetPosition, furniture.transform.rotation)) return false;

        furniture.transform.position = targetPosition;

        return true;
    }

    private bool HandleJoystickRotation(Vector2 input)
    {
        Vector2 direction = GetDirection(input);
        Vector3 localAxis = GetLocalAxis();

        float angle = direction.x != 0 ? rotationStep * Mathf.Sign(direction.x) : rotationStep * Mathf.Sign(direction.y);
        Quaternion targetRotation = Quaternion.AngleAxis(angle, furniture.transform.TransformDirection(localAxis)) * furniture.transform.rotation;

        rotationGizmo.UseDirection(GetGizmoDirection(localAxis * Mathf.Sign(angle)), GetRotationArrowDirection(localAxis, Mathf.Sign(angle)));

        if (WouldCollide(furniture.transform.position, targetRotation)) return false;

        furniture.transform.rotation = targetRotation;

        return true;
    }

    private bool WouldCollide(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (furnitureModelCollider == null) return false;

        Bounds bounds = furnitureModelCollider.bounds;
        Vector3 center = targetPosition + (bounds.center - furniture.transform.position);
        Vector3 halfExtents = bounds.extents;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, targetRotation, mask);

        foreach (var hit in hits)
        {
            if (hit.transform.IsChildOf(furniture.transform)) continue;

            var anchor = hit.GetComponentInParent<MRUKAnchor>();
            if (anchor != null && anchor.Label.HasFlag(sceneLabel)) continue;

            return true;
        }

        return false;
    }

    private Vector2 GetDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return new Vector2(Mathf.Sign(input.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(input.y));
    }

    private Vector3 GetLocalDirection(Vector2 input)
    {
        return sceneLabel switch
        {
            MRUKAnchor.SceneLabels.FLOOR => new Vector3(input.x, 0f, input.y),
            MRUKAnchor.SceneLabels.CEILING => new Vector3(input.x, 0f, input.y),
            MRUKAnchor.SceneLabels.WALL_FACE => new Vector3(input.x, input.y, 0f),
            _ => Vector3.right
        };
    }

    private Vector3 GetLocalAxis()
    {
        return sceneLabel switch
        {
            MRUKAnchor.SceneLabels.FLOOR => Vector3.up,
            MRUKAnchor.SceneLabels.CEILING => Vector3.up,
            MRUKAnchor.SceneLabels.WALL_FACE => Vector3.forward,
            _ => Vector3.up
        };
    }

    private FurnitureGizmo.GizmoDirection GetGizmoDirection(Vector3 dir)
    {
        if (dir == Vector3.right || Vector3.Dot(dir.normalized, Vector3.right) >= threshold)
            return FurnitureGizmo.GizmoDirection.X;
        else if (dir == Vector3.left || Vector3.Dot(dir.normalized, Vector3.left) >= threshold)
            return FurnitureGizmo.GizmoDirection.NegX;
        else if (dir == Vector3.up || Vector3.Dot(dir.normalized, Vector3.up) >= threshold)
            return FurnitureGizmo.GizmoDirection.Y;
        else if (dir == Vector3.down || Vector3.Dot(dir.normalized, Vector3.down) >= threshold)
            return FurnitureGizmo.GizmoDirection.NegY;
        else if (dir == Vector3.forward || Vector3.Dot(dir.normalized, Vector3.forward) >= threshold)
            return FurnitureGizmo.GizmoDirection.Z;
        else if (dir == Vector3.back || Vector3.Dot(dir.normalized, Vector3.back) >= threshold)
            return FurnitureGizmo.GizmoDirection.NegZ;
        else return FurnitureGizmo.GizmoDirection.X;
    }

    private Vector3 GetRotationArrowDirection(Vector3 axis, float angle)
    {
        Vector3 rotation = axis * angle;
        if (Vector3.Dot(rotation.normalized, Vector3.up) >= threshold) return Vector3.back;
        else if (Vector3.Dot(rotation.normalized, Vector3.down) >= threshold) return Vector3.forward;
        else if (Vector3.Dot(rotation.normalized, Vector3.right) >= threshold) return Vector3.down;
        else if (Vector3.Dot(rotation.normalized, Vector3.left) >= threshold) return Vector3.up;
        else if (Vector3.Dot(rotation.normalized, Vector3.forward) >= threshold) return Vector3.down;
        else if (Vector3.Dot(rotation.normalized, Vector3.back) >= threshold) return Vector3.up;
        return Vector3.up;
    }

    public void DeactivateAllFurnitureGizmos()
    {
        movementGizmo.DeactiveAllDirections();
        rotationGizmo.DeactiveAllDirections();
    }
}