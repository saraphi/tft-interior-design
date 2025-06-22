using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject selectorCanvasPrefab;
    private GameObject SelectorCanvas;

    void Start()
    {
        SelectorCanvas = Instantiate(selectorCanvasPrefab, Vector3.zero, Quaternion.identity);
        SelectorCanvas.SetActive(false);
    }

    public void OnStartScanner()
    {
        SoundManager.Instance.PlayEnterClip();
        GameManager.Instance.ScanScene();
        GameManager.Instance.CloseCurrentCanvas();
    }

    public void OnOpenRoomObjectSelector()
    {
        SoundManager.Instance.PlayEnterClip();
        StartCoroutine(GameManager.Instance.OpenCanvasAfterDelay(SelectorCanvas, 0f, 1.8f));
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
}