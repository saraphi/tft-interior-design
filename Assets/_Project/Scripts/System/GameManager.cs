using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EffectMesh effectMeshPrefab;
    [SerializeField] private GameObject menuCanvasPrefab;
    [SerializeField] private LayerMask collisionMask;

    [Header("Game Data")]
    [SerializeField] private DataLoader dataLoader;

    public static GameManager Instance { get; private set; }

    private GameObject menuCanvas;
    private GameObject currentCanvas;

    private EffectMesh effectMesh;

    private MenuCanvas menuCanvasScript;
    private bool hasSavedData;
    private bool firstPlacementDone = false;
    private bool firstJoystickMovementDone = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        effectMesh = Instantiate(effectMeshPrefab);
        effectMesh.HideMesh = true;

        hasSavedData = dataLoader.HasSavedData();

        menuCanvas = Instantiate(menuCanvasPrefab, Vector3.one, Quaternion.identity);
        menuCanvas.SetActive(false);
        menuCanvasScript = menuCanvas.GetComponent<MenuCanvas>();
        menuCanvasScript.SetLoadButtonInteractable(hasSavedData);
        menuCanvasScript.SetDeleteButtonInteractable(hasSavedData);
        menuCanvasScript.SetSaveButtonInteractable(false);

        TutorialManager.Instance.SetStartTutorial(!hasSavedData);
        if (!hasSavedData) TutorialManager.Instance.LaunchTutorial("welcome");
    }

    void Update()
    {
        if (effectMesh != null) SetEffectMeshHideMesh(!FurnitureManager.Instance.IsUsingObject());
        CheckIfAddedRoomObjects();

        if (ControllerManager.Instance.OnMenu())
        {
            if (!hasSavedData && !TutorialManager.Instance.HasCurrentTutorialEnded()) return;
            if (!menuCanvas.activeInHierarchy)
            {
                SoundManager.Instance.PlayEnterClip();
                if (!hasSavedData && TutorialManager.Instance.IsCurrentTutorialActive())
                    TutorialManager.Instance.CloseCurrentTutorialCanvas();
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
        int attempts = 5;
        float step = 0.1f;

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

    public void SetPlacementDone()
    {
        if (!firstPlacementDone)
        {
            firstPlacementDone = true;
            TutorialManager.Instance.LaunchTutorial("select_object");
        }
    }

    public void SetJoystickMovementDone()
    {
        if (!firstJoystickMovementDone)
        {
            firstJoystickMovementDone = true;
            TutorialManager.Instance.LaunchTutorial("joystick_movement");
        }
    }

    public bool IsFirstPlacementDone() => firstPlacementDone;
    public bool IsFirstJoystickMovementDone() => firstJoystickMovementDone;
}