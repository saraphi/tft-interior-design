using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    private int id;

    [Header("Configuration")]
    [SerializeField] private string furnitureName;
    [SerializeField] private FurnitureModel model;
    [SerializeField] private MRUKAnchor.SceneLabels sceneLabel;

    [Header("Interactors")]
    [SerializeField] private FurnitureRayInteractor rayInteractor;
    [SerializeField] private FurnitureJoystickInteractor joystickInteractor;

    private const string DefaultFurnitureLayer = "Furniture";
    private const string GhostFurnitureLayer = "FurnitureGhost";

    public enum State { Placing, Moving, JoystickMoving, Idle };
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
            if (currentState == State.Moving || currentState == State.Placing) hasValidSurface = rayInteractor.Move();
            else if (currentState == State.JoystickMoving) hasValidSurface = joystickInteractor.Move();

            model.SetModelMaterialsTransparency(!hasValidSurface);

            if (ControllerManager.Instance.OnConfirm()) ConfirmMovement();
            else if (ControllerManager.Instance.OnCancel()) CancelMovement();
        }
        else
        {
            if (FurnitureManager.Instance.IsFurnitureSelected(id)) model.SetFresnelHighlight();
            else model.SetModelMaterialsTransparency(false);
        }
    }

    public void StartMovement(State state)
    {
        currentState = state;

        backupPosition = transform.position;
        backupRotation = transform.rotation;

        gameObject.layer = LayerMask.NameToLayer(GhostFurnitureLayer);
        SetModelLayerRecursively(model.gameObject, LayerMask.NameToLayer(GhostFurnitureLayer));

        if (currentState != State.JoystickMoving) rb.isKinematic = false;
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
            SetModelLayerRecursively(model.gameObject, LayerMask.NameToLayer(DefaultFurnitureLayer));

            SoundManager.Instance.PlayReleaseClip();
            FurnitureManager.Instance.ClearFurniture();
            return;
        }

        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnPrimaryControllerVibration();
    }

    private void CancelMovement()
    {
        if (currentState == State.Moving)
        {
            if (transform.position == backupPosition && transform.rotation == backupRotation)
            {
                ControllerManager.Instance.OnPrimaryControllerVibration();
                SoundManager.Instance.PlayErrorClip();
                return;
            }

            transform.position = backupPosition;
            transform.rotation = backupRotation;

            rb.isKinematic = true;
            currentState = State.Idle;

            gameObject.layer = LayerMask.NameToLayer(DefaultFurnitureLayer);
            SetModelLayerRecursively(model.gameObject, LayerMask.NameToLayer(DefaultFurnitureLayer));

            FurnitureManager.Instance.ClearFurniture();
            SoundManager.Instance.PlayDeleteClip();
        }
        else if (currentState == State.Placing) Delete();
    }

    public void Duplicate()
    {
        FurnitureManager.Instance.InstantiateFurniture(gameObject, transform.position, transform.rotation);
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
            ControllerManager.Instance.OnPrimaryControllerVibration();
        }
    }

    private void SetModelLayerRecursively(GameObject obj, int newLayer)
    {
        gameObject.layer = newLayer;
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetModelLayerRecursively(child.gameObject, newLayer);
    }

    public FurnitureModel GetFurnitureModel() => model;
    public string GetFurnitureName() => furnitureName;
    public MRUKAnchor.SceneLabels GetSceneLabel() => sceneLabel;
    public bool IsIdling() => currentState == State.Idle;
    public void SetID(int newId) => id = newId;
    public int GetID() => id;
}