using UnityEngine;

public class FurnitureInstantiator : MonoBehaviour
{
    [SerializeField] private GameObject[] floorFurnitures;
    [SerializeField] private GameObject[] wallFurnitures;
    [SerializeField] private GameObject[] ceilingFurnitures;

    public void AddFurniture(string furnitureType)
    {
        if (PlacementManager.Instance.IsUsingFurniture()) return;

        string[] furnitureInfo = furnitureType.Split(",");
        GameObject[] furnitures = GetFurnituresByAnchor(furnitureInfo[0]);
        string furnitureName = furnitureInfo[1];

        if (furnitures != null) 
        {
            foreach (var furniture in furnitures)
            {
                Furniture furnitureComponent = furniture.GetComponent<Furniture>();
                if (furnitureComponent.GetFurnitureName() == furnitureName)
                {
                    Instantiate(furniture, Vector3.zero, Quaternion.identity);
                    SoundManager.Instance.PlayPressSound();
                    return;
                }
            }
        }

        SoundManager.Instance.PlayErrorSound();
    }

    private GameObject[] GetFurnituresByAnchor(string anchor)
    {
        return anchor switch
        {
            "floor" => floorFurnitures,
            "wall" => wallFurnitures,
            "ceiling" => ceilingFurnitures,
            _ => null
        };
    }
}