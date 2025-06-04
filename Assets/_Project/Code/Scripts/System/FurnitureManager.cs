using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Oculus.Interaction.Body.Input;
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
    private int selectedFurniture = -1;
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

    public void DeselectFurniture()
    {
        if (selectedFurniture != -1) DeselectPreviousFurniture();
        selectedFurniture = -1;
    }

    public void SelectFurniture(int id)
    {
        if (selectedFurniture != -1 && selectedFurniture != id) DeselectPreviousFurniture();
        selectedFurniture = id;
    }

    private void DeselectPreviousFurniture()
    {
        GameObject previous = allFurnitures[selectedFurniture];
        if (previous == null)
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnPrimaryControllerVibration();
            return;
        }

        FurnitureOptionsHandler previousOptionsHandler = previous.GetComponent<FurnitureOptionsHandler>();
        if (previousOptionsHandler != null) previousOptionsHandler.HideOptionsCanvas();
        else ControllerManager.Instance.OnPrimaryControllerVibration();
    }

    public bool IsFurnitureSelected(int id) => selectedFurniture == id;

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
                        InstantiateFurniture(furniture, Vector3.zero, Quaternion.identity);
                        SoundManager.Instance.PlayPressClip();
                        return;
                    }
                }
            }
        }

        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnPrimaryControllerVibration();
    }

    public void InstantiateFurniture(GameObject furniture, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = Instantiate(furniture, position, rotation);
        Furniture newFurniture = newObject.GetComponent<Furniture>();
        int newId = lastId + 1;
        lastId = newId;
        newFurniture.SetID(newId);
        if (!allFurnitures.ContainsKey(newId)) allFurnitures.Add(newId, newObject);
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

    public int GetCurrentFurnitureID()
    {
        return currentFurniture.GetID();
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