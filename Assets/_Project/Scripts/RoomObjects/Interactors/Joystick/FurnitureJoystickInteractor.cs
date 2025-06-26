using Meta.XR.MRUtilityKit;
using UnityEngine;

public class FurnitureJoystickInteractor : JoystickInteractor
{
    [Header("Furniture")]
    [SerializeField] private Furniture furniture;

    private MRUKAnchor.SceneLabels sceneLabel;

    void Start()
    {
        sceneLabel = furniture.GetSceneLabel();
    }

    protected override bool HandleJoystickMovement(Vector2 input)
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

    protected override bool HandleJoystickRotation(Vector2 input)
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
}