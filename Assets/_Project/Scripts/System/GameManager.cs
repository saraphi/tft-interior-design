using System.Collections;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EffectMesh effectMeshPrefab;
    [SerializeField] private GameObject welcomeCanvasPrefab;
    [SerializeField] private GameObject menuCanvasPrefab;
    [SerializeField] private LayerMask collisionMask;

    public static GameManager Instance { get; private set; }

    private bool tutorialStarted = false;
    private GameObject menuCanvas;
    private GameObject welcomeCanvas;
    private GameObject currentCanvas;

    private EffectMesh effectMesh;

    private WelcomeCanvas welcomeCanvasScript;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        effectMesh = Instantiate(effectMeshPrefab);
        effectMesh.HideMesh = true;
        welcomeCanvas = Instantiate(welcomeCanvasPrefab, Vector3.one, Quaternion.identity);
        menuCanvas = Instantiate(menuCanvasPrefab, Vector3.one, Quaternion.identity);
        welcomeCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        welcomeCanvasScript = welcomeCanvas.GetComponent<WelcomeCanvas>();
    }

    void Update()
    {
        if (effectMesh != null) SetEffectMeshHideMesh(!FurnitureManager.Instance.IsUsingObject());

        if (!tutorialStarted)
        {
            tutorialStarted = true;
            StartCoroutine(OpenCanvasAfterDelay(welcomeCanvas, 1f));
        }

        if (ControllerManager.Instance.OnMenu() && welcomeCanvasScript != null && welcomeCanvasScript.HasEnded())
        {
            if (!menuCanvas.activeInHierarchy)
            {
                SoundManager.Instance.PlayEnterClip();
                StartCoroutine(OpenCanvasAfterDelay(menuCanvas, 0f, 1f));
            }
            else
            {
                SoundManager.Instance.PlayExitClip();
                CloseCurrentCanvas();
            }
        }
    }

    public IEnumerator OpenCanvasAfterDelay(GameObject canvas, float delay = 0f, float distance = 1.5f, bool markAsCurrent = true)
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

    public void SetEffectMeshHideMesh(bool hide)
    {
        effectMesh.HideMesh = hide;
    }

    public void ScanScene()
    {
        OVRScene.RequestSpaceSetup().ContinueWith(_ =>
        {
            MRUK.Instance.ClearScene();
            MRUK.Instance.LoadSceneFromDevice();

            if (effectMesh != null) Destroy(effectMesh.gameObject);
            effectMesh = Instantiate(effectMeshPrefab);
            ControllerManager.Instance.OnPrimaryControllerVibration();
        });
    }

    public bool IsRoomLoaded()
    {
        return MRUK.Instance != null && MRUK.Instance.GetCurrentRoom() != null;
    }
}