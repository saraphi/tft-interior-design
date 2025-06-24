using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RoomObject : MonoBehaviour
{
    protected int id;

    [Header("Configuration")]
    [SerializeField] protected string codeName;
    [SerializeField] protected string objectName;
    [SerializeField] protected Sprite image;
    [SerializeField] protected Model model;

    [Header("Interactors")]
    [SerializeField] protected RayInteractor rayInteractor;
    [SerializeField] protected JoystickInteractor joystickInteractor;

    public enum State { Placing, Moving, JoystickMoving, Idle };
    protected State currentState = State.Idle;

    protected Rigidbody rb;
    protected Vector3 backupPosition;
    protected Quaternion backupRotation;
    protected bool hasValidSurface = false;
    protected int collisionMask;
    protected BoxCollider modelCollider;
    protected bool hasGravity = false;
    private OVRSpatialAnchor currentAnchor;
    private bool firstPlacementDone = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collisionMask = ~LayerMask.GetMask("UI", "Gizmo", "Ghost");
        modelCollider = model.GetCollider();
    }

    protected virtual void Update()
    {
        if (!IsIdling())
        {
            if (rayInteractor != null && (currentState == State.Moving || currentState == State.Placing))
                hasValidSurface = rayInteractor.Move();
            else if (joystickInteractor != null && currentState == State.JoystickMoving)
                hasValidSurface = joystickInteractor.Move();

            model.SetModelMaterialsTransparency(!hasValidSurface);

            if (ControllerManager.Instance.OnConfirm()) ConfirmMovement();
            else if (ControllerManager.Instance.OnCancel()) CancelMovement();
        }
        else
        {
            if (FurnitureManager.Instance.IsObjectSelected(id)) model.SetFresnelHighlight();
            else model.SetModelMaterialsTransparency(false);
        }
    }

    public void StartMovement(State state)
    {
        if (state == State.Idle)
        {
            SetLayer(GetDefaultLayer());
            model.SetChildrenCollidersEnabled(true);
            SaveSpatialAnchor();
            return;
        }

        if (currentAnchor != null)
        {
            Destroy(currentAnchor.gameObject);
            currentAnchor = null;
        }

        SetLayer(GetGhostLayer());
        model.SetChildrenCollidersEnabled(false);

        currentState = state;

        backupPosition = transform.position;
        backupRotation = transform.rotation;

        if (currentState != State.JoystickMoving) rb.isKinematic = false;
        rb.useGravity = false;

        SoundManager.Instance.PlayPressClip();
        FurnitureManager.Instance.RegisterObject(this);
    }

    protected virtual void ConfirmMovement()
    {
        if (!hasValidSurface)
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnPrimaryControllerVibration();
            return;
        }

        if (currentState == State.JoystickMoving && joystickInteractor != null)
            joystickInteractor.DeactivateAllGizmos();

        rb.isKinematic = !hasGravity;
        rb.useGravity = hasGravity;
        currentState = State.Idle;

        SetLayer(GetDefaultLayer());
        model.SetChildrenCollidersEnabled(true);

        SaveSpatialAnchor();

        SoundManager.Instance.PlayReleaseClip();
        FurnitureManager.Instance.ClearObject();

        if (!firstPlacementDone && FurnitureManager.Instance.GetFirstAddedObjectID() == id)
        {
            TutorialManager.Instance.LaunchTutorial("select_object");
            firstPlacementDone = true;
        }
    }

    protected void CancelMovement()
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

            if (currentState == State.JoystickMoving && joystickInteractor != null)
                joystickInteractor.DeactivateAllGizmos();

            rb.isKinematic = !hasGravity;
            rb.useGravity = hasGravity;
            currentState = State.Idle;

            SetLayer(GetDefaultLayer());
            model.SetChildrenCollidersEnabled(true);

            SaveSpatialAnchor();

            FurnitureManager.Instance.ClearObject();
            SoundManager.Instance.PlayDeleteClip();
        }
        else if (currentState == State.Placing) Delete();
    }

    public void Duplicate()
    {
        ColorProfile profileColor = model.GetFurnitureColorProfile();
        FurnitureManager.Instance.AddObject(codeName, transform.position, transform.rotation, profileColor.profileName, true);
    }

    public void Delete()
    {
        if (FurnitureManager.Instance.DeleteObject(id))
        {
            SoundManager.Instance.PlayDeleteClip();
            FurnitureManager.Instance.ClearObject();
            Destroy(gameObject);
        }
        else
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnPrimaryControllerVibration();
        }
    }

    public abstract bool WouldCollide(Vector3 targetPosition, Quaternion targetRotation);

    protected void SetLayer(int newLayer)
    {
        gameObject.layer = newLayer;
        SetModelLayerRecursively(model.gameObject, newLayer);

        model.gameObject.layer = GetGhostLayer();
    }

    protected void SetModelLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetModelLayerRecursively(child.gameObject, newLayer);
    }

    protected abstract int GetDefaultLayer();
    protected abstract int GetGhostLayer();

    private async void SaveSpatialAnchor()
    {
        if (currentAnchor != null)
        {
            Destroy(currentAnchor.gameObject);
            currentAnchor = null;
        }

        GameObject anchorObject = new GameObject($"Anchor_{id}");
        anchorObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
        anchorObject.transform.SetParent(transform);

        currentAnchor = anchorObject.AddComponent<OVRSpatialAnchor>();

        await currentAnchor.WhenLocalizedAsync();
        await currentAnchor.SaveAnchorAsync();
    }

    public Model GetModel() => model;
    public string GetCodeName() => codeName;
    public string GetName() => objectName;
    public bool IsIdling() => currentState == State.Idle;
    public void SetID(int newId) => id = newId;
    public int GetID() => id;
    public Sprite GetImageSprite() => image;
    public OVRSpatialAnchor GetSpatialAnchor() => currentAnchor;
}