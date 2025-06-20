using UnityEngine;

public class DecorationRayInteractor : RayInteractor
{
    [SerializeField] private Decoration decoration;
    [SerializeField] private float movementStep = 0.05f;
    private float currentMovement = 0f;

    void Awake()
    {
        rb = decoration.GetComponent<Rigidbody>();
    }

    public override bool Move()
    {
        Vector3 controllerPosition = ControllerManager.Instance.GetRayControllerLocalPosition();
        Quaternion controllerRotation = ControllerManager.Instance.GetRayControllerLocalRotation();

        Vector3 forwardDirection = controllerRotation * Vector3.forward;
        Vector3 targetPosition = controllerPosition + forwardDirection * maxRayLength;
        Quaternion targetRotation = Quaternion.identity;

        Quaternion additionalRotation = GetAddiotionalRotation();
        targetRotation *= additionalRotation;

        Vector3 additionalMovement = GetAdditionalMovement(forwardDirection);
        targetPosition += additionalMovement;

        rb.MovePosition(Vector3.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
        rb.rotation = targetRotation;

        wouldCollide = decoration.WouldCollide(targetPosition, targetRotation);

        return !wouldCollide;
    }

    protected override Quaternion GetAddiotionalRotation()
    {
        Vector2 joystickInput = ControllerManager.Instance.GetSecondaryControllerJoystickInput();

        if (Mathf.Abs(joystickInput.x) > 0.9f)
            currentRotation += joystickInput.x * rotationDegrees;

        Vector3 rotationAxis = Vector3.up;

        return Quaternion.AngleAxis(currentRotation, rotationAxis);
    }

    private Vector3 GetAdditionalMovement(Vector3 forward)
    {
        Vector2 joystickInput = ControllerManager.Instance.GetSecondaryControllerJoystickInput();

        if (Mathf.Abs(joystickInput.y) > 0.9f)
            currentMovement += joystickInput.y * movementStep;

        return forward * currentMovement;
    }
}