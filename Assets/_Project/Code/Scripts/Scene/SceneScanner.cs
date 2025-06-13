
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SceneScanner : MonoBehaviour
{
    [SerializeField] private MRUK mruk;

    public void ScanOrRescan()
    {
        OVRScene.RequestSpaceSetup();
    }

    public bool IsRoomLoaded()
    {
        return MRUK.Instance != null && MRUK.Instance.GetCurrentRoom() != null;
    }
}