using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
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
}