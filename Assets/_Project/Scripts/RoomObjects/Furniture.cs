using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Furniture : RoomObject
{
    [Header("Furniture Configuration")]

    [SerializeField] private MRUKAnchor.SceneLabels sceneLabel;
    [SerializeField] private FurnitureManager.FurnitureCategory category;
    private Vector3 insertionDirection;

    private const string defaultLayer = "Furniture";
    private const string ghostLayer = "Ghost";

    protected override int GetDefaultLayer() => LayerMask.NameToLayer(defaultLayer);
    protected override int GetGhostLayer() => LayerMask.NameToLayer(ghostLayer);

    protected override void Start()
    {
        SetSceneLabelAndDirection();
        base.Start();
    }

    public override bool WouldCollide(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (modelCollider == null) return false;

        Vector3 worldCenter = targetPosition + targetRotation * Vector3.Scale(modelCollider.center, modelCollider.transform.lossyScale);
        Vector3 worldExtents = Vector3.Scale(modelCollider.size * 0.5f, modelCollider.transform.lossyScale);

        Collider[] overlaps = Physics.OverlapBox(worldCenter, worldExtents, targetRotation, collisionMask);

        foreach (var hit in overlaps)
        {
            if (hit == null || hit.transform.IsChildOf(transform)) continue;

            var anchor = hit.GetComponentInParent<MRUKAnchor>();
            if (anchor != null && anchor.Label.HasFlag(sceneLabel)) continue;

            return true;
        }

        return false;
    }

    private void SetSceneLabelAndDirection()
    {
        insertionDirection = sceneLabel switch
        {
            MRUKAnchor.SceneLabels.FLOOR => Vector3.down,
            MRUKAnchor.SceneLabels.CEILING => Vector3.up,
            MRUKAnchor.SceneLabels.WALL_FACE => transform.forward,
            _ => Vector3.down
        };
    }

    public Vector3 GetInsertionPoint()
    {
        Bounds bounds = modelCollider.bounds;

        Vector3 direction = insertionDirection.normalized;

        Vector3 extents = bounds.extents;
        Vector3 offset = new Vector3(
            direction.x * extents.x,
            direction.y * extents.y,
            direction.z * extents.z
        );

        return bounds.center + offset;
    }

    public FurnitureManager.FurnitureCategory GetCategory() => category;
    public MRUKAnchor.SceneLabels GetSceneLabel() => sceneLabel;
}