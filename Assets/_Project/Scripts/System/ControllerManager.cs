using UnityEngine;
using System.Collections;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager Instance { get; private set; }

    public enum ControllerType
    {
        RTouch = OVRInput.Controller.RTouch,
        LTouch = OVRInput.Controller.LTouch
    }

    [Header("Controller Ray Configuration")]
    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private ControllerType primaryController = ControllerType.RTouch;
    [SerializeField] private ControllerType secondaryController = ControllerType.LTouch;

    [Header("Controller Input Configuration")]
    [SerializeField] private OVRInput.RawButton confirmButton = OVRInput.RawButton.A;
    [SerializeField] private OVRInput.RawButton cancelButton = OVRInput.RawButton.B;
    [SerializeField] private OVRInput.RawButton menuButton = OVRInput.RawButton.Y;
    [SerializeField] private OVRInput.RawButton primaryInteractionButton = OVRInput.RawButton.RIndexTrigger;
    [SerializeField] private OVRInput.RawButton secondaryInteractionButton = OVRInput.RawButton.LIndexTrigger;

    private Coroutine primaryVibrationCoroutine;
    private Coroutine secondaryVibrationCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public bool OnConfirm() => OVRInput.GetDown(confirmButton);
    public bool OnCancel() => OVRInput.GetDown(cancelButton);

    public bool OnMenu() => OVRInput.GetDown(menuButton);

    public bool OnPrimaryIndexTrigger() => OVRInput.GetDown(primaryInteractionButton);
    public bool OnSecondaryIndexTrigger() => OVRInput.GetDown(secondaryInteractionButton);

    public Vector3 GetRayControllerLocalPosition()
    {
        return primaryController switch
        {
            ControllerType.RTouch => rightControllerTransform.position,
            ControllerType.LTouch => leftControllerTransform.position,
            _ => rightControllerTransform.position
        };
    }

    public Quaternion GetRayControllerLocalRotation()
    {
        return primaryController switch
        {
            ControllerType.RTouch => rightControllerTransform.rotation,
            ControllerType.LTouch => leftControllerTransform.rotation,
            _ => rightControllerTransform.rotation
        };
    }

    public Vector2 GetPrimaryControllerJoystickInput() => OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)primaryController);
    public Vector2 GetSecondaryControllerJoystickInput() => OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)secondaryController);

    public void OnPrimaryControllerVibration(float duration = 0.1f, float frequency = 1f, float amplitude = 0.5f)
    {
        if (primaryVibrationCoroutine != null) StopCoroutine(primaryVibrationCoroutine);
        OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        primaryVibrationCoroutine = StartCoroutine(StopHapticsAfterDelay(duration, OVRInput.Controller.RTouch));
    }

    public void OnSecondaryControllerVibration(float duration = 0.1f, float frequency = 1f, float amplitude = 0.5f)
    {
        if (secondaryVibrationCoroutine != null) StopCoroutine(secondaryVibrationCoroutine);
        OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
        secondaryVibrationCoroutine = StartCoroutine(StopHapticsAfterDelay(duration, OVRInput.Controller.LTouch));
    }

    private IEnumerator StopHapticsAfterDelay(float delay, OVRInput.Controller controller)
    {
        yield return new WaitForSeconds(delay);
        OVRInput.SetControllerVibration(0f, 0f, controller);
    }
}