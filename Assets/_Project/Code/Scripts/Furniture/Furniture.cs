using System.Runtime.InteropServices.WindowsRuntime;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    private int id;

    [Header("Configuration")]
    [SerializeField] private string furnitureName;
    [SerializeField] private GameObject model;
    [SerializeField] private MRUKAnchor.SceneLabels sceneLabel;

    [Header("Handlers")]
    [SerializeField] private FurnitureVisualHandler visualHandler;

    [Header("Interactors")]
    [SerializeField] private FurnitureRayInteractor rayInteractor;

    private const string DefaultFurnitureLayer = "Furniture";
    private const string GhostFurnitureLayer = "FurnitureGhost";

    public enum State { Placing, Moving, Idle };
    private State currentState = State.Idle;

    private Rigidbody rb;
    private Vector3 backupPosition;
    private Quaternion backupRotation;
    private bool hasValidSurface = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartMovement(State.Placing);
    }

    void Update()
    {
        if (!IsIdling())
        {
            hasValidSurface = rayInteractor.Move();
            visualHandler.SetAlpha(hasValidSurface ? 1f : 0.2f);

            if (ControllerManager.Instance.OnConfirm()) ConfirmMovement();
            else if (ControllerManager.Instance.OnCancel()) CancelMovement();
        }
    }

    public void StartMovement(State state)
    {
        currentState = state;

        backupPosition = transform.position;
        backupRotation = transform.rotation;

        gameObject.layer = LayerMask.NameToLayer(GhostFurnitureLayer);

        rb.isKinematic = false;
        rb.useGravity = false;

        SoundManager.Instance.PlayPressClip();
        FurnitureManager.Instance.RegisterFurniture(this);
    }

    private void ConfirmMovement()
    {
        if (hasValidSurface)
        {
            rb.isKinematic = true;
            currentState = State.Idle;

            gameObject.layer = LayerMask.NameToLayer(DefaultFurnitureLayer);

            SoundManager.Instance.PlayReleaseClip();
            FurnitureManager.Instance.ClearFurniture();
            return;
        }

        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnControllerVibration();
    }

    private void CancelMovement()
    {
        if (currentState == State.Moving)
        {
            if (transform.position == backupPosition && transform.rotation == backupRotation)
            {
                ControllerManager.Instance.OnControllerVibration();
                SoundManager.Instance.PlayErrorClip();
                return;
            }

            transform.position = backupPosition;
            transform.rotation = backupRotation;

            rb.isKinematic = true;
            currentState = State.Idle;

            gameObject.layer = LayerMask.NameToLayer(DefaultFurnitureLayer); ;

            FurnitureManager.Instance.ClearFurniture();
            SoundManager.Instance.PlayDeleteClip();
        }
        else if (currentState == State.Placing) Delete();
    }

    public void Duplicate()
    {
        Instantiate(gameObject, transform.position, transform.rotation);
    }

    public void Delete()
    {
        if (FurnitureManager.Instance.DeleteFurniture(id))
        {
            SoundManager.Instance.PlayDeleteClip();
            FurnitureManager.Instance.ClearFurniture();
            Destroy(gameObject);
        }
        else
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnControllerVibration();
        }
        
    }

    public MeshRenderer GetModelRenderer() => model.GetComponent<MeshRenderer>();
    public Collider GetModelCollider() => model.GetComponent<Collider>();
    public string GetFurnitureName() => furnitureName;
    public MRUKAnchor.SceneLabels GetSceneLabel() => sceneLabel;
    public bool IsIdling() => currentState == State.Idle;
    public void SetID(int newId) => id = newId;
    public int GetID() => id;
}