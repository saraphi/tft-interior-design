using UnityEngine;

public class FurnitureOptionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject optionsCanvas;

    [Header("Gizmos")]
    [SerializeField] private FurnitureGizmoInteractor gizmoMovementInteractor;
    [SerializeField] private FurnitureGizmoInteractor gizmoRotationInteractor;

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
        gizmoMovementInteractor.DeactivateGizmo();
        gizmoRotationInteractor.DeactivateGizmo();

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
            case "gizmo_movement":
                gizmoMovementInteractor.ActivateGizmo();
                break;
            case "gizmo_rotation":
                gizmoRotationInteractor.ActivateGizmo();
                break;
            case "duplicate":
                furniture.Duplicate();
                break;
            case "delete": furniture.Delete();
                break;
            default:
                SoundManager.Instance.PlayErrorClip();
                ControllerManager.Instance.OnControllerVibration();
                break;
        }
    }
}