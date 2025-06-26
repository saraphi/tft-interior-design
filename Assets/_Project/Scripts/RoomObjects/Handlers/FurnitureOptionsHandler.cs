public class FurnitureOptionsHandler : OptionsHandler
{
    public override void OnOptionSelected(string option)
    {
        if (!roomObject.IsIdling()) return;

        ToggleOptionsCanvas();

        switch (option)
        {
            case "color":
                roomObject.SetChangingColor();
                break;
            case "ray":
                roomObject.StartMovement(RoomObject.State.Moving);
                break;
            case "joystick":
                roomObject.StartMovement(RoomObject.State.JoystickMoving);
                if (!GameManager.Instance.IsFirstJoystickMovementDone())
                    GameManager.Instance.SetJoystickMovementDone();
                break;
            case "duplicate":
                roomObject.Duplicate();
                break;
            case "delete":
                roomObject.Delete();
                break;
            default:
                SoundManager.Instance.PlayErrorClip();
                ControllerManager.Instance.OnPrimaryControllerVibration();
                break;
        }
    }
}