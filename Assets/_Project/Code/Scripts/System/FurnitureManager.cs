using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    [SerializeField] private GameObject[] floorFurniture;
    [SerializeField] private GameObject[] wallFurniture;
    [SerializeField] private GameObject[] ceilingFurniture;
    private Furniture currentFurniture;

    public void RegisterFurniture(Furniture furniture)
    {
        currentFurniture = furniture;
    }

    public void ClearFurniture()
    {
        currentFurniture = null;
    }

    public bool IsUsingFurniture()
    {
        return currentFurniture != null; // && currentFurniture.IsPlacingOrMoving();
    }

    public void AddFurniture(string type)
    {
        if (!IsUsingFurniture())
        {
            string[] data = type.Split(",");
            GameObject[] furniture = GetFurnitureByAnchor(data[0]);
            string name = data[1];

            if (furniture != null)
            {
                foreach (var furniturePiece in furniture)
                {
                    Furniture furnitureComponent = furniturePiece.GetComponent<Furniture>();
                    // if (furnitureComponent.GetFurnitureName() == name)
                    // {
                    //     Instantiate(furniturePiece, Vector3.zero, Quaternion.identity);
                    //     SoundManager.Instance.PlayPressSound();
                    //     return;
                    // }
                }
            }
        }

        // SoundManager.Instance.PlayErrorSound();
    }

    private GameObject[] GetFurnitureByAnchor(string anchor)
    {
        return anchor switch
        {
            "floor" => floorFurniture,
            "wall" => wallFurniture,
            "ceiling" => ceilingFurniture,
            _ => null
        };
    }
}