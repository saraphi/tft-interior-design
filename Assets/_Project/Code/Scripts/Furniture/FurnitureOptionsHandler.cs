using UnityEngine;

public class FurnitureOptionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject optionsCanvas;

    [Header("Gizmos")]
    [SerializeField] private FurnitureGizmoInteractor gizmoMovementInteractor;
    [SerializeField] private FurnitureGizmoInteractor gizmoRotationInteractor;

    private Furniture furniture;

    void Awake()
    {
        furniture = GetComponent<Furniture>();
    }

    public void ToggleOptionsCanvas()
    {
        gizmoMovementInteractor.DeactivateGizmo();
        gizmoRotationInteractor.DeactivateGizmo();

        if (optionsCanvas.activeInHierarchy)
        {
            SoundManager.Instance.PlayExitClip();
            optionsCanvas.SetActive(false);
            optionsCanvas.transform.position = transform.position;
            optionsCanvas.transform.rotation = transform.rotation;
        }
        else
        {
            Vector3 toCamera = (Camera.main.transform.position - transform.position).normalized;
            float offset = furniture.GetModelRenderer().bounds.extents.magnitude * 1.2f;

            optionsCanvas.transform.position = transform.position + toCamera * offset + transform.right * 0.5f;
            optionsCanvas.transform.rotation = Quaternion.LookRotation(-toCamera);

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