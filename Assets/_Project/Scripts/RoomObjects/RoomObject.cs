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

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collisionMask = ~LayerMask.GetMask("UI", "Gizmo", "FurnitureGhost");
        modelCollider = model.GetCollider();
    }

    protected virtual void Start()
    {
        StartMovement(State.Placing);
    }

    protected void Update()
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
            if (FurnitureManager.Instance.IsObjectSelected(id)) model.SetFresnelHighlight();
            else model.SetModelMaterialsTransparency(false);
        }
    }

    public void StartMovement(State state)
    {
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

    protected void ConfirmMovement()
    {
        if (hasValidSurface)
        {
            if (currentState == State.JoystickMoving) joystickInteractor.DeactivateAllGizmos();

            rb.isKinematic = !hasGravity;
            rb.useGravity = hasGravity;
            currentState = State.Idle;

            SetLayer(GetDefaultLayer());
            model.SetChildrenCollidersEnabled(true);

            SoundManager.Instance.PlayReleaseClip();
            FurnitureManager.Instance.ClearObject();
            return;
        }

        SoundManager.Instance.PlayErrorClip();
        ControllerManager.Instance.OnPrimaryControllerVibration();
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

            if (currentState == State.JoystickMoving) joystickInteractor.DeactivateAllGizmos();

            rb.isKinematic = !hasGravity;
            rb.useGravity = hasGravity;
            currentState = State.Idle;

            SetLayer(GetDefaultLayer());
            model.SetChildrenCollidersEnabled(true);

            FurnitureManager.Instance.ClearObject();
            SoundManager.Instance.PlayDeleteClip();
        }
        else if (currentState == State.Placing) Delete();
    }

    public void Duplicate()
    {
        ColorProfile profileColor = model.GetFurnitureColorProfile();
        FurnitureManager.Instance.InstantiateObject(gameObject, transform.position, transform.rotation, profileColor.profileName);
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

    public Model GetModel() => model;
    public string GetCodeName() => codeName;
    public string GetName() => objectName;
    public bool IsIdling() => currentState == State.Idle;
    public void SetID(int newId) => id = newId;
    public int GetID() => id;
    public Sprite GetImageSprite() => image;
}