using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    private int id;

    [Header("Configuration")]
    [SerializeField] private string furnitureName;
    [SerializeField] private FurnitureModel model;
    [SerializeField] private MRUKAnchor.SceneLabels sceneLabel;
    [SerializeField] private FurnitureManager.FurnitureCategory category;

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
    private int collisionMask;
    private BoxCollider modelCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        modelCollider = model.GetCollider();
        collisionMask = ~LayerMask.GetMask("UI", "Gizmo");
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

        SetFurnitureLayer(LayerMask.NameToLayer(GhostFurnitureLayer));
        model.SetChildrenCollidersEnabled(false);

        if (currentState != State.JoystickMoving) rb.isKinematic = false;
        rb.useGravity = false;        

        SoundManager.Instance.PlayPressClip();
        FurnitureManager.Instance.RegisterFurniture(this);
    }

    private void ConfirmMovement()
    {
        if (hasValidSurface)
        {
            if (currentState == State.JoystickMoving) joystickInteractor.DeactivateAllFurnitureGizmos();

            rb.isKinematic = true;
            currentState = State.Idle;

            SetFurnitureLayer(LayerMask.NameToLayer(DefaultFurnitureLayer));
            model.SetChildrenCollidersEnabled(true);

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

            if (currentState == State.JoystickMoving) joystickInteractor.DeactivateAllFurnitureGizmos();

            rb.isKinematic = true;
            currentState = State.Idle;

            SetFurnitureLayer(LayerMask.NameToLayer(DefaultFurnitureLayer));
            model.SetChildrenCollidersEnabled(true);

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

    private void SetFurnitureLayer(int newLayer)
    {
        gameObject.layer = newLayer;

        SetModelLayerRecursively(model.gameObject, newLayer);

        model.gameObject.layer = LayerMask.NameToLayer(GhostFurnitureLayer);
    }

    private void SetModelLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetModelLayerRecursively(child.gameObject, newLayer);
    }

    public bool WouldCollide(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (modelCollider == null) return false;
        Bounds bounds = modelCollider.bounds;

        Vector3 center = targetPosition + (bounds.center - transform.position);
        Vector3 halfExtents = bounds.extents;

        Collider[] overlaps = Physics.OverlapBox(center, halfExtents, targetRotation, collisionMask);

        foreach (var hit in overlaps)
        {
            if (hit == null || hit.transform.IsChildOf(transform)) continue;

            var anchor = hit.GetComponentInParent<MRUKAnchor>();
            if (anchor != null && anchor.Label.HasFlag(sceneLabel)) continue;

            return true;
        }

        return false;
    }

    public FurnitureModel GetFurnitureModel() => model;
    public string GetFurnitureName() => furnitureName;
    public FurnitureManager.FurnitureCategory GetFurnitureCategory() => category;
    public MRUKAnchor.SceneLabels GetSceneLabel() => sceneLabel;
    public bool IsIdling() => currentState == State.Idle;
    public void SetID(int newId) => id = newId;
    public int GetID() => id;
}