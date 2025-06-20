using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private GameObject furnitureSelectorCanvasPrefab;
    private GameObject furnitureSelectorCanvas;

    void Start()
    {
        furnitureSelectorCanvas = Instantiate(furnitureSelectorCanvasPrefab, Vector3.zero, Quaternion.identity);
        furnitureSelectorCanvas.SetActive(false);
    }

    public void OnStartScanner()
    {
        SoundManager.Instance.PlayEnterClip();
        GameManager.Instance.ScanScene();
        GameManager.Instance.CloseCurrentCanvas();
    }

    public void OnOpenFurnitureSelector()
    {
        SoundManager.Instance.PlayEnterClip();
        StartCoroutine(GameManager.Instance.OpenCanvasAfterDelay(furnitureSelectorCanvas, 0f, 1.8f));
    }

    public void OnOpenDecorationSelector()
    {
        SoundManager.Instance.PlayEnterClip();
        FurnitureManager.Instance.AddObject("notebook2", "white");
    }
}