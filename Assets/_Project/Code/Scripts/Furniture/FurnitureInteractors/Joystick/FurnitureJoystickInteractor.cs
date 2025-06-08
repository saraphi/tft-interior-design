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
    private bool hasValidPosition = false;
    private float lastMoveTime = -Mathf.Infinity;
    private float threshold = 0.9f;

    void Start()
    {
        sceneLabel = furniture.GetSceneLabel();
        FurnitureModel furnitureModel = furniture.GetFurnitureModel();
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

        movementGizmo.UseDirection(localDirection);

        Vector3 worldDirection = furniture.transform.TransformDirection(localDirection);
        Vector3 targetPosition = furniture.transform.position + worldDirection * movementStep;

        if (furniture.WouldCollide(targetPosition, furniture.transform.rotation)) return false;

        furniture.transform.position = targetPosition;

        return true;
    }

    private bool HandleJoystickRotation(Vector2 input)
    {
        Vector2 direction = GetDirection(input);
        Vector3 localAxis = GetLocalAxis();

        float angle = direction.x != 0 ? rotationStep * Mathf.Sign(direction.x) : rotationStep * Mathf.Sign(direction.y);
        Quaternion targetRotation = Quaternion.AngleAxis(angle, furniture.transform.TransformDirection(localAxis)) * furniture.transform.rotation;

        rotationGizmo.UseDirection(localAxis * Mathf.Sign(angle));

        if (furniture.WouldCollide(furniture.transform.position, targetRotation)) return false;

        furniture.transform.rotation = targetRotation;

        return true;
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

    public void DeactivateAllFurnitureGizmos()
    {
        movementGizmo.DeactiveAllDirections();
        rotationGizmo.DeactiveAllDirections();
    }
}