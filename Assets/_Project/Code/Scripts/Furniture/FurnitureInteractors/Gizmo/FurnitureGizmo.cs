using UnityEngine;

public class FurnitureGizmo : MonoBehaviour
{
    [SerializeField] private Furniture furniture;

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

    public void UseDirection(GizmoDirection gizmoDirection, Vector3 arrowDirection)
    {
        GameObject arrow = null;

        switch (gizmoDirection)
        {
            case GizmoDirection.X: arrow = directionX; break;
            case GizmoDirection.Y: arrow = directionY; break;
            case GizmoDirection.Z: arrow = directionZ; break;
            case GizmoDirection.NegX: arrow = directionNegX; break;
            case GizmoDirection.NegY: arrow = directionNegY; break;
            case GizmoDirection.NegZ: arrow = directionNegZ; break;
        }

        if (arrow == null) return;

        arrow.SetActive(true);
        PositionArrow(arrow, arrowDirection);
    }

    private void PositionArrow(GameObject arrow, Vector3 arrowDirection)
    {
        if (furniture == null) return;

        Collider collider = furniture.GetModelCollider();
        if (collider == null) return;

        Bounds bounds = collider.bounds;
        Vector3 worldDirection = furniture.transform.TransformDirection(arrowDirection).normalized;

        float extent = Vector3.Scale(bounds.extents, worldDirection).magnitude;

        Vector3 offset = worldDirection * (extent + 0.05f);
        arrow.transform.position = bounds.center + offset;
    }
}