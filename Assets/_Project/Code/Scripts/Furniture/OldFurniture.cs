using Meta.XR.MRUtilityKit;
using UnityEngine;

public class OldFurniture : MonoBehaviour
{
    [Header("Furniture Info")]
    [SerializeField] private string furnitureName;
    [SerializeField] private MeshRenderer modelRenderer;

    [Header("Modules")]
    [SerializeField] private FurniturePlacement placement;
    [SerializeField] private FurnitureInteraction interaction;
    [SerializeField] private FurnitureUIHandler uiHandler;

    [Header("Furniture Interactors")]
    [SerializeField] private FurnitureRayInteractor rayInteractor;
    [SerializeField] private FurnitureGizmoMovementInteractor gizmoMovementInteractor;
    [SerializeField] private FurnitureGizmoRotationInteractor gizmoRotationInteractor;

    [Header("Room Labels")]
    [SerializeField] private MRUKAnchor.SceneLabels sceneLabels;


    void Start()
    {
        placement.BeginPlacement(rayInteractor);
    }

    void Update()
    {
        if (!placement.isPlaced) placement.HandlePlacementInput();
        else if (interaction.isMoving) interaction.HandleInteractionInput();
    }

    public string GetFurnitureName() => furnitureName;
    
    public MeshRenderer GetModelRenderer() => modelRenderer;

    public MRUKAnchor.SceneLabels GetSceneLabels() => sceneLabels;

    public void SetSceneLabel(MRUKAnchor.SceneLabels label) => sceneLabels = label;

    public void OnOptionSelected(string option)
    {
        if (IsPlacingOrMoving()) 
        {
            SoundManager.Instance.PlayErrorSound();
            return;
        }
        
        ToggleOptions();

        switch (option)
        {
            case "ray": interaction.StartContinuousMovement(rayInteractor); break;
            case "gizmo_movement": gizmoMovementInteractor.ActivateGizmo(); break;
            case "gizmo_rotation": gizmoRotationInteractor.ActivateGizmo(); break;
            case "duplicate": interaction.Duplicate(); break;
            case "delete": interaction.Delete(); break;
            default: SoundManager.Instance.PlayErrorSound(); break;
        }
    }

    public void ToggleOptions()
    {
        gizmoMovementInteractor.DeactivateGizmo();
        gizmoRotationInteractor.DeactivateGizmo();
        uiHandler.ToggleOptionsCanvas();
    }

    public bool IsPlacingOrMoving() => !placement.isPlaced || interaction.isMoving;
}