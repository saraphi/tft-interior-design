using UnityEngine;

public class Decoration : RoomObject
{
    [Header("Decoration Configuration")]
    [SerializeField] private FurnitureManager.DecorationCategory category;
    private Collider[] acceptedOverlaps = new Collider[0];

    private const string defaultLayer = "Decoration";
    private const string ghostLayer = "Ghost";

    protected override int GetDefaultLayer() => LayerMask.NameToLayer(defaultLayer);
    protected override int GetGhostLayer() => LayerMask.NameToLayer(ghostLayer);

    protected override void Awake()
    {
        base.Awake();
        hasGravity = true;
    }

    public override bool WouldCollide(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (modelCollider == null) return false;

        Vector3 worldCenter = targetPosition + targetRotation * Vector3.Scale(modelCollider.center, modelCollider.transform.lossyScale);
        Vector3 worldExtents = Vector3.Scale(modelCollider.size * 0.5f, modelCollider.transform.lossyScale);
        // Vector3 worldExtents = Vector3.Scale(modelCollider.bounds.extents, modelCollider.transform.lossyScale);

        Collider[] overlaps = Physics.OverlapBox(worldCenter, worldExtents, targetRotation, collisionMask);

        foreach (var hit in overlaps)
        {
            if (hit == null || hit.transform.IsChildOf(transform)) continue;

            // Si ya se aceptó esta colisión en una colocación anterior, ignórala
            if (System.Array.Exists(acceptedOverlaps, c => c == hit)) continue;

            return true;
        }

        return false;
    }

    protected override void ConfirmMovement()
    {
        base.ConfirmMovement();
        if (hasValidSurface && this is Decoration decor)
        {
            Vector3 worldCenter = transform.position + transform.rotation * Vector3.Scale(modelCollider.center, modelCollider.transform.lossyScale);
            Vector3 worldExtents = Vector3.Scale(modelCollider.size * 0.5f, modelCollider.transform.lossyScale);
            decor.acceptedOverlaps = Physics.OverlapBox(worldCenter, worldExtents, transform.rotation, collisionMask);
        }
    }

    public FurnitureManager.DecorationCategory GetCategory() => category;
}