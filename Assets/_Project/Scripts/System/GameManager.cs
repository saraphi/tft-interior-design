using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EffectMesh effectMeshPrefab;
    [SerializeField] private GameObject welcomeCanvasPrefab;
    [SerializeField] private GameObject menuCanvasPrefab;
    [SerializeField] private LayerMask collisionMask;

    [Header("Game Data")]
    [SerializeField] private DataLoader dataLoader;

    public static GameManager Instance { get; private set; }

    private bool tutorialStarted = false;
    private bool startTutorial = false;
    private GameObject menuCanvas;
    private GameObject welcomeCanvas;
    private GameObject currentCanvas;

    private EffectMesh effectMesh;

    private TutorialCanvas welcomeCanvasScript;
    private MenuCanvas menuCanvasScript;

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
        welcomeCanvasScript = welcomeCanvas.GetComponent<TutorialCanvas>();
        menuCanvasScript = menuCanvas.GetComponent<MenuCanvas>();

        bool hasSavedData = dataLoader.HasSavedData();

        startTutorial = !hasSavedData;
        menuCanvasScript.SetLoadButtonInteractable(hasSavedData);
        menuCanvasScript.SetDeleteButtonInteractable(hasSavedData);
        menuCanvasScript.SetSaveButtonInteractable(false);
    }

    void Update()
    {
        if (effectMesh != null) SetEffectMeshHideMesh(!FurnitureManager.Instance.IsUsingObject());
        CheckIfAddedRoomObjects();

        if (startTutorial && !tutorialStarted)
        {
            tutorialStarted = true;
            StartCoroutine(OpenCanvasAfterDelay(welcomeCanvas, 1f));
        }

        if (ControllerManager.Instance.OnMenu())
        {
            if (startTutorial && tutorialStarted && !welcomeCanvasScript.HasEnded()) return;
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

    private void CheckIfAddedRoomObjects()
    {
        Dictionary<int, GameObject> allRoomObjects = FurnitureManager.Instance.GetAllAddedRoomObjects();
        menuCanvasScript.SetSaveButtonInteractable(allRoomObjects.Count > 0);
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

    public void SaveData()
    {
        bool saved = dataLoader.SaveData();
        if (saved) menuCanvasScript.SetDeleteButtonInteractable(true);
    }

    public async void LoadData()
    {
        bool success = await dataLoader.LoadData();
        if (!success)
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnPrimaryControllerVibration();
            ControllerManager.Instance.OnSecondaryControllerVibration();
        }
        if (success) SoundManager.Instance.PlayReleaseClip();
    }

    public void DeleteData()
    {
        bool deleted = dataLoader.DeleteData();
        if (deleted)
        {
            menuCanvasScript.SetLoadButtonInteractable(false);
            menuCanvasScript.SetDeleteButtonInteractable(false);
        }
    }

    public bool IsRoomLoaded()
    {
        return MRUK.Instance != null && MRUK.Instance.GetCurrentRoom() != null;
    }
}