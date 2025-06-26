using Meta.XR.MRUtilityKit;
using UnityEngine;

public class DecorationRayInteractor : RayInteractor
{
    [SerializeField] private float movementStep = 0.02f;
    [SerializeField] private float maxMovementDistance = 5f;
    [SerializeField] private Decoration decoration;
    
    private float currentMovement = 0f;
    private Vector3 currentRotation = Vector3.zero;

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

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room != null && !room.GetRoomBounds().Contains(targetPosition)) hasValidPosition = false;
        else hasValidPosition = true;

        rb.MovePosition(Vector3.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
        rb.rotation = targetRotation;

        wouldCollide = decoration.WouldCollide(targetPosition, targetRotation);

        return hasValidPosition && !wouldCollide;
    }

    protected override Quaternion GetAddiotionalRotation()
    {
        Vector2 joystickInput = ControllerManager.Instance.GetSecondaryControllerJoystickInput();

        Decoration.DecorationRotationAxis rotationAxis = decoration.GetRotationAxis();

        float delta = 0f;
        if (Mathf.Abs(joystickInput.x) > rotationThreshold)
            delta = joystickInput.x * rotationDegrees;

        switch (rotationAxis)
        {
            case Decoration.DecorationRotationAxis.X: currentRotation.x += delta; break;
            case Decoration.DecorationRotationAxis.Y: currentRotation.y += delta; break;
            case Decoration.DecorationRotationAxis.Z: currentRotation.z += delta; break;
        }

        return Quaternion.Euler(currentRotation);
    }

    private Vector3 GetAdditionalMovement(Vector3 forward)
    {
        Vector2 joystickInput = ControllerManager.Instance.GetPrimaryControllerJoystickInput();

        if (Mathf.Abs(joystickInput.y) > 0.9f)
        {
            float addition = joystickInput.y * movementStep;
            if (currentMovement + addition <= maxMovementDistance && currentMovement + addition >= -maxRayLength)
                currentMovement += addition;
            else ControllerManager.Instance.OnPrimaryControllerVibration();
        }

        return forward * currentMovement;
    }
}