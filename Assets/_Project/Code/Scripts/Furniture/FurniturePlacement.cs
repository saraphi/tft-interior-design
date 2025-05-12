
using UnityEngine;

public class FurniturePlacement : MonoBehaviour
{
    private Material modelMaterial;
    private Rigidbody rb;
    private bool hasValidSurface = false;
    public bool isPlaced { get; private set; } = false;

    private Furniture furniture;
    private FurnitureRayInteractor rayInteractor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        furniture = GetComponent<Furniture>();

        if (furniture.GetModelRenderer() != null)
        {
            modelMaterial = new Material(furniture.GetModelRenderer().material);
            furniture.GetModelRenderer().material = modelMaterial;
        }
    }

    public void BeginPlacement(FurnitureRayInteractor interactor)
    {
        rayInteractor = interactor;
        PlacementManager.Instance.RegisterPlacingFurniture(furniture);
        SoundManager.Instance.PlayPressSound();
        rb.isKinematic = false;
        rb.useGravity = false;
    }

    public void HandlePlacementInput()
    {
        SetAlpha(hasValidSurface ? 1f : 0.2f);

        hasValidSurface = rayInteractor.Move(furniture.GetSceneLabels());

        if (ControllerManager.Instance.OnConfirm()) TryPlace();
        else if (ControllerManager.Instance.OnCancel()) CancelPlacement();
    }

    private void TryPlace()
    {
        if (!hasValidSurface)
        {
            SoundManager.Instance.PlayErrorSound();
            ControllerManager.Instance.OnControllerVibration();
            return;
        }

        isPlaced = true;
        rb.isKinematic = true;
        
        SoundManager.Instance.PlayReleaseSound();
        PlacementManager.Instance.ClearFurniture();
    }

    private void CancelPlacement()
    {
        PlacementManager.Instance.ClearFurniture();
        SoundManager.Instance.PlayDeleteSound();
        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        if (modelMaterial == null) return;
        Color color = modelMaterial.color;
        color.a = alpha;
        modelMaterial.color = color;
    }
}