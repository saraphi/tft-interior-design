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

    public void HideOptionsCanvas()
    {
        optionsCanvas.SetActive(false);
    }

    public void ShowOptionsCanvas()
    {
        SoundManager.Instance.PlayEnterClip();
        follower.PositionCanvas();
        optionsCanvas.SetActive(true);
    }

    public void ToggleOptionsCanvas()
    {
        if (FurnitureManager.Instance.IsUsingFurniture()) return;
        
        int id = furniture.GetID();

        if (FurnitureManager.Instance.IsFurnitureSelected(id))
        {
            FurnitureManager.Instance.DeselectFurniture();
            SoundManager.Instance.PlayExitClip();
        }
        else
        {
            ShowOptionsCanvas();
            FurnitureManager.Instance.SelectFurniture(id);
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