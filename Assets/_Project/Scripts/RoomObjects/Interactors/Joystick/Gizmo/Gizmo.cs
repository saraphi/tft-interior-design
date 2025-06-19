using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [SerializeField] private RoomObject roomObject;
    [SerializeField] private GizmoType gizmoType;
    [SerializeField] private float margin = 0.05f;

    [Header("Gizmo Arrows")]
    [SerializeField] private GameObject directionX;
    [SerializeField] private GameObject directionY;
    [SerializeField] private GameObject directionZ;
    [SerializeField] private GameObject directionNegX;
    [SerializeField] private GameObject directionNegY;
    [SerializeField] private GameObject directionNegZ;

    public enum GizmoDirection { X, Y, Z, NegX, NegY, NegZ };
    public enum GizmoType { Movement, Rotation };
    private float threshold = 0.9f;

    private Dictionary<GizmoDirection, GameObject> arrows = new Dictionary<GizmoDirection, GameObject>();
    private Dictionary<GizmoDirection, Vector3> arrowsDirections = new Dictionary<GizmoDirection, Vector3>();

    void Start()
    {
        arrows.Add(GizmoDirection.X, directionX);
        arrows.Add(GizmoDirection.Y, directionY);
        arrows.Add(GizmoDirection.Z, directionZ);
        arrows.Add(GizmoDirection.NegX, directionNegX);
        arrows.Add(GizmoDirection.NegY, directionNegY);
        arrows.Add(GizmoDirection.NegZ, directionNegZ);

        Model objectModel = roomObject.GetModel();
        BoxCollider collider = objectModel.GetCollider();
        if (collider == null) return;

        Bounds bounds = collider.bounds;

        if (gizmoType == GizmoType.Rotation) SetRotationArrowsDirections();
        else SetMovementArrowsDirections();

        foreach (KeyValuePair<GizmoDirection, GameObject> arrow in arrows)
        {
            Vector3 direction = arrowsDirections[arrow.Key];
            if (direction == null) continue;
            Vector3 offset = direction * (Vector3.Scale(bounds.extents, direction).magnitude + margin);
            arrow.Value.transform.position = bounds.center + offset;
        }
    }

    public void DeactiveAllDirections()
    {
        directionX.SetActive(false);
        directionY.SetActive(false);
        directionZ.SetActive(false);
        directionNegX.SetActive(false);
        directionNegY.SetActive(false);
        directionNegZ.SetActive(false);
    }

    public void UseDirection(Vector3 direction)
    {
        GizmoDirection gizmoDirection = GetGizmoDirection(direction);
        GameObject arrow = arrows[gizmoDirection];
        if (arrow == null) return;
        arrow.SetActive(true);
    }

    private GizmoDirection GetGizmoDirection(Vector3 direction)
    {
        if (Vector3.Dot(direction.normalized, Vector3.right) >= threshold) return GizmoDirection.X;
        else if (Vector3.Dot(direction.normalized, Vector3.left) >= threshold) return GizmoDirection.NegX;
        else if (Vector3.Dot(direction.normalized, Vector3.up) >= threshold) return GizmoDirection.Y;
        else if (Vector3.Dot(direction.normalized, Vector3.down) >= threshold) return GizmoDirection.NegY;
        else if (Vector3.Dot(direction.normalized, Vector3.forward) >= threshold) return GizmoDirection.Z;
        else if (Vector3.Dot(direction.normalized, Vector3.back) >= threshold) return GizmoDirection.NegZ;
        else return GizmoDirection.X;
    }

    private void SetMovementArrowsDirections()
    {
        arrowsDirections.Add(GizmoDirection.X, roomObject.transform.TransformDirection(Vector3.right).normalized);
        arrowsDirections.Add(GizmoDirection.Y, roomObject.transform.TransformDirection(Vector3.up).normalized);
        arrowsDirections.Add(GizmoDirection.Z, roomObject.transform.TransformDirection(Vector3.forward).normalized);
        arrowsDirections.Add(GizmoDirection.NegX, roomObject.transform.TransformDirection(Vector3.left).normalized);
        arrowsDirections.Add(GizmoDirection.NegY, roomObject.transform.TransformDirection(Vector3.down).normalized);
        arrowsDirections.Add(GizmoDirection.NegZ, roomObject.transform.TransformDirection(Vector3.back).normalized);
    }

    private void SetRotationArrowsDirections()
    {
        arrowsDirections.Add(GizmoDirection.X, roomObject.transform.TransformDirection(Vector3.down).normalized);
        arrowsDirections.Add(GizmoDirection.Y, roomObject.transform.TransformDirection(Vector3.back).normalized);
        arrowsDirections.Add(GizmoDirection.Z, roomObject.transform.TransformDirection(Vector3.down).normalized);
        arrowsDirections.Add(GizmoDirection.NegX, roomObject.transform.TransformDirection(Vector3.up).normalized);
        arrowsDirections.Add(GizmoDirection.NegY, roomObject.transform.TransformDirection(Vector3.forward).normalized);
        arrowsDirections.Add(GizmoDirection.NegZ, roomObject.transform.TransformDirection(Vector3.up).normalized);
    }
}