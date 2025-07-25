using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }

    [SerializeField] private List<GameObject> allFurniture;
    [SerializeField] private List<GameObject> allDecorations;

    private RoomObject currentObject;
    private int selectedObject = -1;
    private Dictionary<int, GameObject> allAddedRoomObjects = new Dictionary<int, GameObject>();
    private int lastId = -1;

    private Dictionary<string, GameObject> allRoomObjectsByName = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> allFurnitureByName = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> allDecorationsByName = new Dictionary<string, GameObject>();
    private Dictionary<FurnitureCategory, List<GameObject>> allFurnitureByCategory = new Dictionary<FurnitureCategory, List<GameObject>>();
    private Dictionary<DecorationCategory, List<GameObject>> allDecorationsByCategory = new Dictionary<DecorationCategory, List<GameObject>>();

    public enum FurnitureCategory { Chairs, Beds, Sofas, Tables, CeilingLights, Clocks }
    public enum DecorationCategory { Technology, Desk }

    private bool isChangingColor = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        foreach (var category in (FurnitureCategory[])Enum.GetValues(typeof(FurnitureCategory)))
            allFurnitureByCategory.Add(category, new List<GameObject>());

        foreach (var category in (DecorationCategory[])Enum.GetValues(typeof(DecorationCategory)))
            allDecorationsByCategory.Add(category, new List<GameObject>());

        foreach (var furniture in allFurniture)
        {
            Furniture furnitureComponent = furniture.GetComponent<Furniture>();
            string furnitureCodeName = furnitureComponent.GetCodeName();
            FurnitureCategory furnitureCategory = furnitureComponent.GetCategory();
            allRoomObjectsByName[furnitureCodeName] = furniture;
            allFurnitureByName[furnitureCodeName] = furniture;
            List<GameObject> list = allFurnitureByCategory[furnitureCategory];
            list.Add(furniture);
        }

        foreach (var decoration in allDecorations)
        {
            Decoration decorationComponent = decoration.GetComponent<Decoration>();
            string decorationCodeName = decorationComponent.GetCodeName();
            DecorationCategory decorationCategory = decorationComponent.GetCategory();
            allRoomObjectsByName[decorationCodeName] = decoration;
            allDecorationsByName[decorationCodeName] = decoration;
            List<GameObject> list = allDecorationsByCategory[decorationCategory];
            list.Add(decoration);
        }
    }

    public void RegisterObject(RoomObject roomObject)
    {
        currentObject = roomObject;
    }

    public void ClearObject()
    {
        currentObject = null;
    }

    public void SetChangingColor(bool changing)
    {
        isChangingColor = changing;
    }

    public bool IsChangingColor() => isChangingColor;

    public bool IsUsingObject()
    {
        return isChangingColor || (currentObject != null && !currentObject.IsIdling());
    }

    public void DeselectObject()
    {
        if (selectedObject != -1) DeselectPreviousObject();
        selectedObject = -1;
    }

    public void SelectObject(int id)
    {
        if (selectedObject != -1 && selectedObject != id) DeselectPreviousObject();
        selectedObject = id;
    }

    private void DeselectPreviousObject()
    {
        GameObject previous = allAddedRoomObjects[selectedObject];
        if (previous == null)
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnPrimaryControllerVibration();
            return;
        }

        OptionsHandler previousOptionsHandler = previous.GetComponent<OptionsHandler>();
        if (previousOptionsHandler != null) previousOptionsHandler.HideOptionsCanvas();
        else ControllerManager.Instance.OnPrimaryControllerVibration();
    }

    public bool IsObjectSelected(int id) => selectedObject == id;

    public void AddObject(string codeName, string profileColor = null)
    {
        if (!IsUsingObject())
        {
            if (allRoomObjectsByName.ContainsKey(codeName))
            {
                GameObject roomObject = allRoomObjectsByName[codeName];
                if (roomObject != null)
                {
                    RoomObject roomObjectComponent = roomObject.GetComponent<RoomObject>();
                    if (roomObjectComponent != null)
                    {
                        InstantiateRoomObject(roomObject, Vector3.zero, Quaternion.identity, profileColor, true);
                        SoundManager.Instance.PlayPressClip();
                        return;
                    }
                }
            }
        }
        
        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnPrimaryControllerVibration();
    }

    public void AddObject(string codeName, Vector3 position, Quaternion rotation, string profileColor, bool startMovement)
    {
        if (!IsUsingObject())
        {
            if (allRoomObjectsByName.ContainsKey(codeName))
            {
                GameObject roomObject = allRoomObjectsByName[codeName];
                if (roomObject != null)
                {
                    RoomObject roomObjectComponent = roomObject.GetComponent<RoomObject>();
                    if (roomObjectComponent != null)
                    {
                        InstantiateRoomObject(roomObject, position, rotation, profileColor, startMovement);
                        return;
                    }
                }
            }
        }
        
        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnPrimaryControllerVibration();
    }

    private void InstantiateRoomObject(GameObject roomObject, Vector3 position, Quaternion rotation, string profileColor, bool startMovement)
    {
        GameObject newObject = Instantiate(roomObject, position, rotation);
        RoomObject newRoomObject = newObject.GetComponent<RoomObject>();
        int newId = lastId + 1;
        lastId = newId;
        newRoomObject.SetID(newId);
        if (!allAddedRoomObjects.ContainsKey(newId)) allAddedRoomObjects.Add(newId, newObject);
        Model newModel = newRoomObject.GetModel();
        newModel.ApplyColorProfile(profileColor);
        if (startMovement) newRoomObject.StartMovement(RoomObject.State.Placing);
        else newRoomObject.StartMovement(RoomObject.State.Idle);
    }

    public void DeleteAllObjects()
    {
        if (allAddedRoomObjects.Count == 0) return;
        foreach (GameObject roomObject in allAddedRoomObjects.Values)
            Destroy(roomObject);
        allAddedRoomObjects.Clear();
    }

    public bool DeleteObject(int id)
    {
        if (allAddedRoomObjects.ContainsKey(id))
        {
            allAddedRoomObjects.Remove(id);
            return true;
        }
        return false;
    }

    public int GetCurrentObjectID() => currentObject.GetID();

    public Dictionary<int, GameObject> GetAllAddedRoomObjects() => allAddedRoomObjects;
    public GameObject GetPrefabByCodeName(string codeName) => allRoomObjectsByName[codeName];

    public List<string> GetAllFurnitureCategories() => Enum.GetNames(typeof(FurnitureCategory)).ToList();
    public List<GameObject> GetAllFurniture() => allFurnitureByName.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
    public List<GameObject> GetAllFurnitureByCategory(FurnitureCategory category) => allFurnitureByCategory[category];

    public List<string> GetAllDecorationsCategories() => Enum.GetNames(typeof(DecorationCategory)).ToList();
    public List<GameObject> GetAllDecorations() => allDecorationsByName.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
    public List<GameObject> GetAllDecorationsByCategory(DecorationCategory category) => allDecorationsByCategory[category];
}