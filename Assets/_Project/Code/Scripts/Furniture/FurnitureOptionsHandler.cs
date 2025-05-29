using UnityEngine;

public class FurnitureOptionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject optionsCanvas;

    private Furniture furniture;
    private FurnitureOptionsFollower follower;

    void Awake()
    {
        furniture = GetComponent<Furniture>();
        if (optionsCanvas != null)
            follower = optionsCanvas.GetComponent<FurnitureOptionsFollower>();
    }

    public void ToggleOptionsCanvas()
    {
        if (!furniture.IsIdling())
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnPrimaryControllerVibration();
            return;
        }

        if (optionsCanvas.activeInHierarchy)
        {
            SoundManager.Instance.PlayExitClip();
            optionsCanvas.SetActive(false);
        }
        else
        {
            follower.SetTarget(furniture);
            SoundManager.Instance.PlayEnterClip();
            optionsCanvas.SetActive(true);
        }
    }

    public void OnOptionSelected(string option)
    {
        if (!furniture.IsIdling()) return;

        ToggleOptionsCanvas();

        switch (option)
        {
            case "ray":
                furniture.StartMovement(Furniture.State.Moving);
                break;
            case "stick_movement":
                furniture.StartMovement(Furniture.State.StickMoving);
                break;
            case "stick_rotation":
                furniture.StartMovement(Furniture.State.StickRotating);
                break;
            case "duplicate":
                furniture.Duplicate();
                break;
            case "delete": furniture.Delete();
                break;
            default:
                SoundManager.Instance.PlayErrorClip();
                ControllerManager.Instance.OnPrimaryControllerVibration();
                break;
        }
    }
}