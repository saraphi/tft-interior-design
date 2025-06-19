using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }

    [SerializeField] private List<GameObject> allFurniture;

    private Furniture currentFurniture;
    private int selectedFurniture = -1;
    private Dictionary<int, GameObject> allAddedFurniture = new Dictionary<int, GameObject>();
    private int lastId = -1;

    private Dictionary<string, GameObject> allFurnitureByName = new Dictionary<string, GameObject>();
    private Dictionary<FurnitureCategory, List<GameObject>> allFurnitureByCategory = new Dictionary<FurnitureCategory, List<GameObject>>();

    public enum FurnitureCategory { Chairs, Beds, Sofas, Tables, CeilingLights, Clocks }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        foreach (var category in (FurnitureCategory[])Enum.GetValues(typeof(FurnitureCategory)))
            allFurnitureByCategory.Add(category, new List<GameObject>());

        foreach (var furniture in allFurniture)
        {
            Furniture furnitureComponent = furniture.GetComponent<Furniture>();
            string furnitureCodeName = furnitureComponent.GetCodeName();
            FurnitureCategory furnitureCategory = furnitureComponent.GetCategory();
            allFurnitureByName[furnitureCodeName] = furniture;
            List<GameObject> list = allFurnitureByCategory[furnitureCategory];
            list.Add(furniture);
        }
    }

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
        GameObject previous = allAddedFurniture[selectedFurniture];
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

    public void AddFurniture(string name, string profileColor = null)
    {
        if (!IsUsingFurniture())
        {
            if (allFurnitureByName.ContainsKey(name))
            {
                GameObject furniture = allFurnitureByName[name];
                if (furniture != null)
                {
                    Furniture furnitureComponent = furniture.GetComponent<Furniture>();
                    if (furnitureComponent != null)
                    {
                        InstantiateFurniture(furniture, Vector3.zero, Quaternion.identity, profileColor);
                        SoundManager.Instance.PlayPressClip();
                        return;
                    }
                }
            }
            SoundManager.Instance.PlayErrorClip();
        }

        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnPrimaryControllerVibration();
    }

    public void InstantiateFurniture(GameObject furniture, Vector3 position, Quaternion rotation, string profileColor = null)
    {
        GameObject newObject = Instantiate(furniture, position, rotation);
        Furniture newFurniture = newObject.GetComponent<Furniture>();
        int newId = lastId + 1;
        lastId = newId;
        newFurniture.SetID(newId);
        if (!allAddedFurniture.ContainsKey(newId)) allAddedFurniture.Add(newId, newObject);
        Model newFurnitureModel = newFurniture.GetModel();
        newFurnitureModel.ApplyColorProfile(profileColor);
    }

    public bool DeleteFurniture(int id)
    {
        if (allAddedFurniture.ContainsKey(id))
        {
            allAddedFurniture.Remove(id);
            return true;
        }
        return false;
    }

    public int GetCurrentFurnitureID() => currentFurniture.GetID();

    public List<string> GetCategories() => Enum.GetNames(typeof(FurnitureCategory)).ToList();
    public List<GameObject> GetAllFurniture() => allFurnitureByName.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
    public List<GameObject> GetAllFurnitureByCategory(FurnitureCategory category) => allFurnitureByCategory[category];
}