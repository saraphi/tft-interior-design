using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private Furniture currentFurniture;

    public void RegisterPlacingFurniture(Furniture furniture)
    {
        currentFurniture = furniture;
    }

    public void ClearFurniture()
    {
        currentFurniture = null;
    }

    public bool IsUsingFurniture()
    {
        return currentFurniture != null && currentFurniture.IsPlacingOrMoving();
    }
}