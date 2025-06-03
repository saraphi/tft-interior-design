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
        furniture.ToggleSelected();
        if (optionsCanvas.activeInHierarchy)
        {
            SoundManager.Instance.PlayExitClip();
            optionsCanvas.SetActive(false);
            if (FurnitureManager.Instance.GetCurrentFurnitureID() == furniture.GetID())
                FurnitureManager.Instance.ClearFurniture();
        }
        else
        {
            SoundManager.Instance.PlayEnterClip();
            follower.PositionCanvas();
            optionsCanvas.SetActive(true);
            FurnitureManager.Instance.RegisterFurniture(furniture);
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
            case "joystick":
                furniture.StartMovement(Furniture.State.JoystickMoving);
                break;
            case "duplicate":
                furniture.Duplicate();
                break;
            case "delete":
                furniture.Delete();
                break;
            default:
                SoundManager.Instance.PlayErrorClip();
                ControllerManager.Instance.OnPrimaryControllerVibration();
                break;
        }
    }
}