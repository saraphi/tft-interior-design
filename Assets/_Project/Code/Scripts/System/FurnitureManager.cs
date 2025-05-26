using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    [SerializeField] private List<GameObject> floorFurniture;
    [SerializeField] private List<GameObject> wallFurniture;
    [SerializeField] private List<GameObject> ceilingFurniture;
    private Furniture currentFurniture;
    private Dictionary<int, GameObject> allFurnitures = new Dictionary<int, GameObject>();
    private int lastId = -1;

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
            List<GameObject> furnitureList = GetFurnitureByAnchor(data[0]);
            string furnitureName = data[1];

            if (furnitureList != null)
            {
                foreach (var furniture in furnitureList)
                {
                    Furniture furnitureComponent = furniture.GetComponent<Furniture>();
                    if (furnitureComponent.GetFurnitureName() == furnitureName)
                    {
                        GameObject newObject = Instantiate(furniture, Vector3.zero, Quaternion.identity);
                        Furniture newFurniture = newObject.GetComponent<Furniture>();
                        int newId = lastId + 1;
                        lastId = newId;
                        newFurniture.SetID(newId);
                        allFurnitures.Add(newId, newObject);
                        SoundManager.Instance.PlayPressClip();
                        return;
                    }
                }
            }
        }

        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnControllerVibration();
    }

    public bool DeleteFurniture(int id)
    {
        if (allFurnitures.ContainsKey(id))
        {
            allFurnitures.Remove(id);
            return true;
        }
        return false;
    }

    private List<GameObject> GetFurnitureByAnchor(string anchor)
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