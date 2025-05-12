using Meta.XR.MRUtilityKit;
using UnityEngine;

public class FurnitureGizmoMovementInteractor : FurnitureGizmoInteractor
{
    [SerializeField] private float movementOffset = 0.05f;

    [Header("Arrow References")]
    [SerializeField] private GameObject arrowX;
    [SerializeField] private GameObject arrowY;
    [SerializeField] private GameObject arrowZ;
    [SerializeField] private GameObject arrowNegX;
    [SerializeField] private GameObject arrowNegY;

    [SerializeField] private GameObject arrowNegZ;
    

    public override void ActivateGizmo()
    {
        base.ActivateGizmo();
        UpdateAvailableDirections();
    }

    private void UpdateAvailableDirections()
    {
        MRUKAnchor.SceneLabels sceneLabels = furniture.GetSceneLabels();

        SetAllArrowsActive(true);

        if (sceneLabels == MRUKAnchor.SceneLabels.FLOOR || sceneLabels == MRUKAnchor.SceneLabels.CEILING)
        {
            arrowY?.SetActive(false);      
            arrowNegY?.SetActive(false);   
        }
        else if (sceneLabels == MRUKAnchor.SceneLabels.WALL_FACE)
        {
            arrowZ?.SetActive(false);
            arrowNegZ?.SetActive(false);
        }
    }

    private void SetAllArrowsActive(bool state)
    {
        arrowX?.SetActive(state);
        arrowNegX?.SetActive(state);
        arrowY?.SetActive(state);
        arrowNegY?.SetActive(state);
        arrowZ?.SetActive(state);
        arrowNegZ?.SetActive(state);
    }

    public void MoveInDirection(string direction)
    {
        SoundManager.Instance.PlayPressSound();

        Vector3 localDirection = GetDirection(direction);
        Vector3 worldDirection = furniture.transform.TransformDirection(localDirection);
        Vector3 targetPosition = furniture.transform.position + worldDirection * movementOffset;

        if (WouldCollideAtPosition(targetPosition))
        {
            SoundManager.Instance.PlayErrorSound();
            ControllerManager.Instance.OnControllerVibration();
            return;
        }

        furniture.transform.position = targetPosition;
    }

    private bool WouldCollideAtPosition(Vector3 targetPosition)
    {
        Collider furnitureCollider = furniture.GetComponent<Collider>();
        if (furnitureCollider == null) return false;

        Bounds bounds = furnitureCollider.bounds;
        Vector3 halfExtents = bounds.extents;

        Vector3 newCenter = targetPosition + (bounds.center - furniture.transform.position);

        Collider[] futureHits = Physics.OverlapBox(newCenter, halfExtents, furniture.transform.rotation, ~0);
        Collider[] currentHits = Physics.OverlapBox(bounds.center, halfExtents, furniture.transform.rotation, ~0);

        foreach (Collider hit in futureHits)
        {
            if (hit.transform == furniture.transform) continue;

            bool alreadyOverlapping = false;

            foreach (Collider current in currentHits)
            {
                if (current == hit)
                {
                    alreadyOverlapping = true;
                    break;
                }
            }

            if (!alreadyOverlapping) return true;
        }

        return false;
    }

    private Vector3 GetDirection(string directionString)
    {
        return directionString switch
        {
            "x" => Vector3.right,
            "y" => Vector3.up,
            "z" => Vector3.forward,
            "neg_x" => Vector3.left,
            "neg_y" => Vector3.down,
            "neg_z" => Vector3.back,
            _ => Vector3.zero
        };
    }
}