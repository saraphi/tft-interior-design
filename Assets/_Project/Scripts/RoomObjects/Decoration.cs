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

    public enum DecorationRotationAxis { X, Y, Z }
    private DecorationRotationAxis currentAxis = DecorationRotationAxis.Y;

    private bool rotationTriggerDown = false;

    protected override void Awake()
    {
        base.Awake();
        hasGravity = true;
    }

    protected override void Update()
    {
        base.Update();

        if (IsIdling()) GameManager.Instance.OnHideDecorationAxisCanvas();
        else GameManager.Instance.OnShowDecorationAxisCanvas();

        if (!IsIdling() && ControllerManager.Instance.OnSecondaryIndexTrigger())
        {
            if (!rotationTriggerDown)
            {
                currentAxis = currentAxis switch
                {
                    DecorationRotationAxis.X => DecorationRotationAxis.Y,
                    DecorationRotationAxis.Y => DecorationRotationAxis.Z,
                    DecorationRotationAxis.Z => DecorationRotationAxis.X,
                    _ => DecorationRotationAxis.Y
                };
                SoundManager.Instance.PlayPressClip();
                rotationTriggerDown = true;
                GameManager.Instance.SetDecorationRotationAxisToCanvas(currentAxis);
            }
        }
        else rotationTriggerDown = false;
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
    public DecorationRotationAxis GetRotationAxis() => currentAxis;
}