using UnityEngine;

public abstract class JoystickInteractor : MonoBehaviour
{
    [SerializeField] protected float cooldown = 0.25f;

    [Header("Movement")]
    [SerializeField] protected Gizmo movementGizmo;
    [SerializeField] protected float movementStep = 0.05f;

    [Header("Rotation")]
    [SerializeField] protected Gizmo rotationGizmo;
    [SerializeField] protected float rotationStep = 5f;

    protected bool hasValidPosition = false;
    protected float lastMoveTime = -Mathf.Infinity;
    protected float threshold = 0.9f;

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

    protected abstract bool HandleJoystickMovement(Vector2 input);
    protected abstract bool HandleJoystickRotation(Vector2 input);

    protected Vector2 GetDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return new Vector2(Mathf.Sign(input.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(input.y));
    }

    public void DeactivateAllGizmos()
    {
        movementGizmo.DeactiveAllDirections();
        rotationGizmo.DeactiveAllDirections();
    }
}