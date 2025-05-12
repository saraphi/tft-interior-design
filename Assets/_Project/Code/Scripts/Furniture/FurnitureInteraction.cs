using UnityEngine;

public class FurnitureInteraction : MonoBehaviour
{
    private Furniture furniture;
    private Rigidbody rb;

    private Vector3 backupPosition;
    private Quaternion backupRotation;

    private IContinuousFurnitureInteractor continuousInteractor;
    public bool isMoving { get; private set; } = false;

    private void Awake()
    {
        furniture = GetComponent<Furniture>();
        rb = GetComponent<Rigidbody>();
    }

    public void HandleInteractionInput()
    {
        if (!isMoving || continuousInteractor == null) return;

        bool hasValidPosition = continuousInteractor.Move(furniture.GetSceneLabels());

        if (ControllerManager.Instance.OnConfirm()) ConfirmMove();
        else if (ControllerManager.Instance.OnCancel()) CancelMove();
    }

    public void StartContinuousMovement(IContinuousFurnitureInteractor interactor)
    {
        continuousInteractor = interactor;
        isMoving = true;

        backupPosition = transform.position;
        backupRotation = transform.rotation;

        rb.isKinematic = false;
        rb.useGravity = false;

        SoundManager.Instance.PlayPressSound();
        PlacementManager.Instance.RegisterPlacingFurniture(furniture);
    }

    private void ConfirmMove()
    {
        isMoving = false;
        continuousInteractor = null;

        rb.isKinematic = true;

        SoundManager.Instance.PlayReleaseSound();
        PlacementManager.Instance.ClearFurniture();
    }

    private void CancelMove()
    {
        isMoving = false;
        continuousInteractor = null;

        transform.position = backupPosition;
        transform.rotation = backupRotation;
        
        rb.isKinematic = true;
        
        SoundManager.Instance.PlayDeleteSound();
        PlacementManager.Instance.ClearFurniture();
    }

    public void Duplicate()
    {
        Instantiate(gameObject, transform.position + Vector3.right * 0.3f, transform.rotation);
    }

    public void Delete()
    {
        SoundManager.Instance.PlayDeleteSound();
        PlacementManager.Instance.ClearFurniture();
        Destroy(gameObject);
    }
}