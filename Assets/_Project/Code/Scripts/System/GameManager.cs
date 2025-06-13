using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject welcomeCanvasPrefab;
    [SerializeField] private GameObject menuCanvasPrefab;
    [SerializeField] private LayerMask collisionMask;

    public static GameManager Instance { get; private set; }

    private bool tutorialStarted = false;
    private GameObject menuCanvas;
    private GameObject welcomeCanvas;
    private GameObject currentCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        welcomeCanvas = Instantiate(welcomeCanvasPrefab, Vector3.one, Quaternion.identity);
        menuCanvas = Instantiate(menuCanvasPrefab, Vector3.one, Quaternion.identity);
        welcomeCanvas.SetActive(false);
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (!tutorialStarted)
        {
            tutorialStarted = true;
            StartCoroutine(OpenCanvasAfterDelay(welcomeCanvas, 1f));
        }

        if (ControllerManager.Instance.OnMenu())
        {
            if (!menuCanvas.activeInHierarchy)
            {
                SoundManager.Instance.PlayEnterClip();
                StartCoroutine(OpenCanvasAfterDelay(menuCanvas));
            }
            else
            {
                SoundManager.Instance.PlayExitClip();
                CloseCurrentCanvas();
            }
        }
    }

    public IEnumerator OpenCanvasAfterDelay(GameObject canvas, float delay = 0f, float distance = 1f, bool markAsCurrent = true)
    {
        yield return new WaitForSeconds(delay);
        if (markAsCurrent)
        {
            CloseCurrentCanvas();
            currentCanvas = canvas;
        }
        PositionCanvas(canvas, distance);
        canvas.SetActive(true);
    }

    public void CloseCurrentCanvas()
    {
        if (currentCanvas != null) currentCanvas.SetActive(false);
    }

    public void PositionCanvas(GameObject canvas, float distance)
    {
        Vector3 eyeLevelPos = Camera.main.transform.position + Camera.main.transform.forward * distance;
        eyeLevelPos.y = Camera.main.transform.position.y;

        Quaternion rotation = Quaternion.LookRotation(eyeLevelPos - Camera.main.transform.position);

        Vector3 finalPos = eyeLevelPos;
        float radius = 0.5f;
        int attempts = 10;
        float step = 0.2f;

        while (Physics.CheckSphere(finalPos, radius, collisionMask) && attempts > 0)
        {
            finalPos += Camera.main.transform.right * step;
            attempts--;
        }

        canvas.transform.SetPositionAndRotation(finalPos, Quaternion.Euler(0, rotation.eulerAngles.y, 0));
    }
}