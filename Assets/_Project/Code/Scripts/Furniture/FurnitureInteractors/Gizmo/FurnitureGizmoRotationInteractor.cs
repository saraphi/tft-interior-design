using Meta.XR.MRUtilityKit;
using UnityEngine;

public class FurnitureGizmoRotationInteractor : FurnitureGizmoInteractor
{
    [SerializeField] private float rotationDegrees = 15f;

    [Header("Rotation Arcs")]
    [SerializeField] private GameObject arcX;
    [SerializeField] private GameObject arcY;
    [SerializeField] private GameObject arcZ;

    public override void ActivateGizmo()
    {
        base.ActivateGizmo();
        UpdateAvailableRotations();
    }

    private void UpdateAvailableRotations()
    {
        MRUKAnchor.SceneLabels sceneLabel = furniture.GetSceneLabel();

        SetAllArcsActive(true);

        if (sceneLabel == MRUKAnchor.SceneLabels.FLOOR || sceneLabel == MRUKAnchor.SceneLabels.CEILING)
        {
            arcX?.SetActive(false);      
            arcZ?.SetActive(false);   
        }
        else if (sceneLabel == MRUKAnchor.SceneLabels.WALL_FACE)
        {
            arcX?.SetActive(false);
            arcY?.SetActive(false);
        }
    }

    private void SetAllArcsActive(bool state)
    {
        arcX?.SetActive(state);
        arcY?.SetActive(state);
        arcZ?.SetActive(state);
    }

    public void Rotate(string axis)
    {
        SoundManager.Instance.PlayPressClip();

        Vector3 localAxis = GetDirection(axis);
        Vector3 worldAxis = furniture.transform.TransformDirection(localAxis);
        Quaternion proposedRotation = Quaternion.AngleAxis(rotationDegrees, worldAxis) * furniture.transform.rotation;

        if (WouldCollideAfterRotation(proposedRotation))
        {
            SoundManager.Instance.PlayErrorClip();
            ControllerManager.Instance.OnPrimaryControllerVibration();
            return;
        }

        furniture.transform.rotation = proposedRotation;
    }

    private bool WouldCollideAfterRotation(Quaternion proposedRotation)
    {
        Collider furnitureCollider = furniture.GetComponent<Collider>();
        if (furnitureCollider == null) return false;

        Bounds bounds = furnitureCollider.bounds;
        Vector3 halfExtents = bounds.extents;

        Vector3 center = furniture.transform.position + (bounds.center - furniture.transform.position);

        Collider[] futureHits = Physics.OverlapBox(center, halfExtents, proposedRotation, ~0);
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

    private Vector3 GetDirection(string axis)
    {
        return axis switch
        {
            "x" => Vector3.left,
            "y" => Vector3.down,
            "z" => Vector3.back,
            _ => Vector3.zero
        };
    }
}