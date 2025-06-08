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
    [SerializeField] private ControllerType primaryController = ControllerType.RTouch;
    [SerializeField] private ControllerType secondaryController = ControllerType.LTouch;

    [Header("Controller Input Configuration")]
    [SerializeField] private OVRInput.RawButton confirmButton = OVRInput.RawButton.A;
    [SerializeField] private OVRInput.RawButton cancelButton = OVRInput.RawButton.B;
    [SerializeField] private OVRInput.RawButton primaryInteractionButton = OVRInput.RawButton.RIndexTrigger;
    [SerializeField] private OVRInput.RawButton secondaryInteractionButton = OVRInput.RawButton.LIndexTrigger;

    private Coroutine vibrationCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public bool OnConfirm() => OVRInput.GetDown(confirmButton);
    public bool OnCancel() => OVRInput.GetDown(cancelButton);

    public bool OnPrimaryIndexTrigger() => OVRInput.GetDown(primaryInteractionButton);
    public bool OnSecondaryIndexTrigger() => OVRInput.GetDown(secondaryInteractionButton);

    public Vector3 GetRayControllerLocalPosition() => OVRInput.GetLocalControllerPosition((OVRInput.Controller)primaryController);
    public Quaternion GetRayControllerLocalRotation() => OVRInput.GetLocalControllerRotation((OVRInput.Controller)primaryController);

    public Vector2 GetPrimaryControllerJoystickInput() => OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)primaryController);
    public Vector2 GetSecondaryControllerJoystickInput() => OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)secondaryController);

    public void OnPrimaryControllerVibration(float duration = 0.1f, float frequency = 1f, float amplitude = 0.5f)
    {
        if (vibrationCoroutine != null) StopCoroutine(vibrationCoroutine);
        OVRInput.SetControllerVibration(frequency, amplitude, (OVRInput.Controller)primaryController);
        vibrationCoroutine = StartCoroutine(StopHapticsAfterDelay(duration));
    }

    public void OnSecondaryControllerVibration(float duration = 0.1f, float frequency = 1f, float amplitude = 0.5f)
    {
        if (vibrationCoroutine != null) StopCoroutine(vibrationCoroutine);
        OVRInput.SetControllerVibration(frequency, amplitude, (OVRInput.Controller)secondaryController);
        vibrationCoroutine = StartCoroutine(StopHapticsAfterDelay(duration));
    }

    private IEnumerator StopHapticsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OVRInput.SetControllerVibration(0f, 0f, (OVRInput.Controller)primaryController);
        vibrationCoroutine = null;
    }
}