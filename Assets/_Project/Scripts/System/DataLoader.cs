using System;
using System.IO;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

[Serializable]
public class RoomObjectSaveData
{
    public string id;
    public string codeName;
    public string profileColor;
    public string anchorUUID;
}

[Serializable]
public class RoomSaveData
{
    public string roomUUID;
    public List<RoomObjectSaveData> objects = new();
}

public class DataLoader : MonoBehaviour
{
    private const string saveFilePath = "game_data.json";

    public bool SaveData()
    {
        if (!GameManager.Instance.IsRoomLoaded()) return false;

        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        string roomUUID = currentRoom.Anchor.Uuid.ToString();

        RoomSaveData data = new RoomSaveData() { roomUUID = roomUUID };

        foreach (var pair in FurnitureManager.Instance.GetAllAddedRoomObjects())
        {
            GameObject roomObject = pair.Value;
            if (roomObject == null) continue;

            RoomObject roomObjectComponent = roomObject.GetComponent<RoomObject>();
            if (roomObjectComponent == null) continue;

            OVRSpatialAnchor anchor = roomObjectComponent.GetSpatialAnchor();

            if (anchor != null && anchor.Uuid != Guid.Empty)
            {
                data.objects.Add(new RoomObjectSaveData
                {
                    id = roomObjectComponent.GetID().ToString(),
                    codeName = roomObjectComponent.GetCodeName(),
                    profileColor = roomObjectComponent.GetModel().GetFurnitureColorProfile().profileName,
                    anchorUUID = anchor.Uuid.ToString()
                });
            }
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, saveFilePath);
        File.WriteAllText(path, json);
        return true;
    }

    public async Task<bool> LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFilePath);
        if (!File.Exists(path)) return false;

        string json = File.ReadAllText(path);
        RoomSaveData data = JsonUtility.FromJson<RoomSaveData>(json);

        if (!GameManager.Instance.IsRoomLoaded()) return false;

        string currentUUID = MRUK.Instance.GetCurrentRoom().Anchor.Uuid.ToString();
        if (data.roomUUID != currentUUID) return false;

        List<Guid> anchorGuids = data.objects
            .Select(obj => Guid.TryParse(obj.anchorUUID, out Guid g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();

        List<OVRSpatialAnchor.UnboundAnchor> anchors = new();
        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(anchorGuids, anchors);

        if (!result.Success) return false;

        for (int i = 0; i < anchors.Count; i++)
        {
            var unbound = anchors[i];
            var savedObject = data.objects.FirstOrDefault(o => o.anchorUUID == unbound.Uuid.ToString());
            if (savedObject == null) continue;

            GameObject temp = new GameObject($"TempAnchor_{unbound.Uuid}");
            var anchor = temp.AddComponent<OVRSpatialAnchor>();

            unbound.BindTo(anchor);
            await anchor.WhenLocalizedAsync();

            FurnitureManager.Instance.AddObject(
                savedObject.codeName,
                anchor.transform.position,
                anchor.transform.rotation,
                savedObject.profileColor,
                false
            );

            Destroy(anchor.gameObject);
        }

        return true;
    }

    public bool DeleteData()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFilePath);
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }

    public bool HasSavedData()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFilePath);
        if (!File.Exists(path)) return false;

        string json = File.ReadAllText(path);
        return !string.IsNullOrEmpty(json) && json.Length > 10;
    } 
}