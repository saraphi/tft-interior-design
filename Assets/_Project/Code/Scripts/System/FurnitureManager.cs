using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
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
        return currentFurniture != null && !currentFurniture.IsIdling();
    }

    public void AddFurniture(string type)
    {
        if (!IsUsingFurniture())
        {
            string[] data = type.Split(",");
            GameObject[] furnitureList = GetFurnitureByAnchor(data[0]);
            string furnitureName = data[1];

            if (furnitureList != null)
            {
                foreach (var furniture in furnitureList)
                {
                    Furniture furnitureComponent = furniture.GetComponent<Furniture>();
                    if (furnitureComponent.GetFurnitureName() == furnitureName)
                    {
                        Instantiate(furniture, Vector3.zero, Quaternion.identity);
                        SoundManager.Instance.PlayPressClip();
                        return;
                    }
                }
            }
        }

        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnControllerVibration();
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