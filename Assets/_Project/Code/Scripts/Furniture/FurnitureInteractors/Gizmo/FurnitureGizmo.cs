using UnityEngine;

public class FurnitureGizmo : MonoBehaviour
{
    [Header("Gizmo Arrows")]
    [SerializeField] private GameObject directionX;
    [SerializeField] private GameObject directionY;
    [SerializeField] private GameObject directionZ;
    [SerializeField] private GameObject directionNegX;
    [SerializeField] private GameObject directionNegY;
    [SerializeField] private GameObject directionNegZ;

    public enum GizmoDirection { X, Y, Z, NegX, NegY, NegZ };

    public void DeactiveAllDirections()
    {
        directionX.SetActive(false);
        directionY.SetActive(false);
        directionZ.SetActive(false);
        directionNegX.SetActive(false);
        directionNegY.SetActive(false);
        directionNegZ.SetActive(false);
    }

    public void UseDirection(GizmoDirection gizmoDirection)
    {
        switch (gizmoDirection)
        {
            case GizmoDirection.X: directionX.SetActive(true); break;
            case GizmoDirection.Y: directionY.SetActive(true); break;
            case GizmoDirection.Z: directionZ.SetActive(true); break;
            case GizmoDirection.NegX: directionNegX.SetActive(true); break;
            case GizmoDirection.NegY: directionNegY.SetActive(true); break;
            case GizmoDirection.NegZ: directionNegZ.SetActive(true); break;
        }
    }
}