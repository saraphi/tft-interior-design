using UnityEngine;
using System.Collections;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager Instance { get; private set; }

    public enum RayControllerType
    {
        RTouch = OVRInput.Controller.RTouch,
        LTouch = OVRInput.Controller.LTouch
    }

    [Header("Controller Ray Configuration")]
    [SerializeField] private RayControllerType rayController = RayControllerType.RTouch;

    [Header("Controller Input Configuration")]
    [SerializeField] private OVRInput.RawButton confirmButton = OVRInput.RawButton.A;
    [SerializeField] private OVRInput.RawButton cancelButton = OVRInput.RawButton.B;

    private Coroutine vibrationCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public Vector3 GetRayControllerLocalPosition()
    {
        return OVRInput.GetLocalControllerPosition((OVRInput.Controller)rayController);
    }

    public Quaternion GetRayControllerLocalRotation()
    {
        return OVRInput.GetLocalControllerRotation((OVRInput.Controller)rayController);
    }

    public bool OnConfirm()
    {
        return OVRInput.GetDown(confirmButton);
    }

    public bool OnCancel()
    {
        return OVRInput.GetDown(cancelButton);
    }

    public void OnControllerVibration(float duration = 0.1f, float frequency = 1f, float amplitude = 0.5f)
    {
        if (vibrationCoroutine != null) StopCoroutine(vibrationCoroutine);
        OVRInput.SetControllerVibration(frequency, amplitude, (OVRInput.Controller)rayController);
        vibrationCoroutine = StartCoroutine(StopHapticsAfterDelay(duration));
    }

    private IEnumerator StopHapticsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OVRInput.SetControllerVibration(0f, 0f, (OVRInput.Controller)rayController);
        vibrationCoroutine = null;
    }
}