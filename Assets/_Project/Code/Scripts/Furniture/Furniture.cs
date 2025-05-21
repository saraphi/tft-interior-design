using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string furnitureName;
    [SerializeField] private GameObject model;
    [SerializeField] private MRUKAnchor.SceneLabels sceneLabel;

    [Header("Handlers")]
    [SerializeField] private FurnitureVisualHandler visualHandler;

    [Header("Interactors")]
    [SerializeField] private FurnitureRayInteractor rayInteractor;

    public enum State { Placing, Moving, Idle };
    private State currentState;

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
            hasValidSurface = rayInteractor.Move(sceneLabel);
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
        SoundManager.Instance.PlayDeleteClip();
        FurnitureManager.Instance.ClearFurniture();
        Destroy(gameObject);
    }

    public MeshRenderer GetModelRenderer() => model.GetComponent<MeshRenderer>();
    public string GetFurnitureName() => furnitureName;
    public MRUKAnchor.SceneLabels GetSceneLabel() => sceneLabel;
    public bool IsIdling() => currentState == State.Idle;
}