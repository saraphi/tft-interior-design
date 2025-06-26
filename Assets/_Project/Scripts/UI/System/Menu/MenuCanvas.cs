using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button furnitureButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject selectorCanvasPrefab;
    [SerializeField] private GameObject rescanRoomCanvasPrefab;
    private GameObject selectorCanvas;
    private GameObject rescanRoomCanvas;

    void Start()
    {
        selectorCanvas = Instantiate(selectorCanvasPrefab, Vector3.zero, Quaternion.identity);
        rescanRoomCanvas = Instantiate(rescanRoomCanvasPrefab, Vector3.zero, Quaternion.identity);
        selectorCanvas.SetActive(false);
        rescanRoomCanvas.SetActive(false);
    }

    public void OnScanRoom()
    {
        SoundManager.Instance.PlayEnterClip();
        StartCoroutine(GameManager.Instance.OpenCanvasAfterDelay(rescanRoomCanvas, 0f, 1.2f));
    }

    public void OnOpenRoomObjectSelector()
    {
        SoundManager.Instance.PlayEnterClip();
        StartCoroutine(GameManager.Instance.OpenCanvasAfterDelay(selectorCanvas, 0f, 1.8f));
        TutorialManager.Instance.LaunchTutorial("ray_movement");
    }

    public void OnSaveData()
    {
        SoundManager.Instance.PlayEnterClip();
        GameManager.Instance.SaveData();
    }

    public void OnLoadData()
    {
        SoundManager.Instance.PlayEnterClip();
        GameManager.Instance.LoadData();
    }

    public void OnDeleteData()
    {
        SoundManager.Instance.PlayEnterClip();
        GameManager.Instance.DeleteData();
    }

    public void SetSaveButtonInteractable(bool interactable)
    {
        saveButton.interactable = interactable;
    }

    public void SetLoadButtonInteractable(bool interactable)
    {
        loadButton.interactable = interactable;
    }

    public void SetDeleteButtonInteractable(bool interactable)
    {
        deleteButton.interactable = interactable;
    }

    public void SetFurnitureButtonInteractable(bool interactable)
    {
        furnitureButton.interactable = interactable;
    }
}