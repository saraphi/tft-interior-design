using System.Collections.Generic;
using System.Linq;
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
    private Dictionary<int, GameObject> allAddedFurnitures = new Dictionary<int, GameObject>();
    private int lastId = -1;

    private Dictionary<string, GameObject> allFurnitures = new Dictionary<string, GameObject>();

    void Start()
    {
        List<GameObject> allFurnitureList = floorFurniture.Concat(wallFurniture).Concat(ceilingFurniture).ToList();
        allFurnitures = allFurnitureList.Distinct().ToDictionary(x => x.GetComponent<Furniture>().GetFurnitureName(), x => x);
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
        GameObject previous = allAddedFurnitures[selectedFurniture];
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

    public void AddFurniture(string name)
    {
        if (!IsUsingFurniture())
        {
            if (allFurnitures.ContainsKey(name))
            {
                GameObject furniture = allFurnitures[name];
                if (furniture != null)
                {
                    Furniture furnitureComponent = furniture.GetComponent<Furniture>();
                    if (furnitureComponent != null)
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
        if (!allAddedFurnitures.ContainsKey(newId)) allAddedFurnitures.Add(newId, newObject);
    }

    public bool DeleteFurniture(int id)
    {
        if (allAddedFurnitures.ContainsKey(id))
        {
            allAddedFurnitures.Remove(id);
            return true;
        }
        return false;
    }

    public int GetCurrentFurnitureID()
    {
        return currentFurniture.GetID();
    }
}