using UnityEngine;

public class Decoration : RoomObject
{
    [Header("Decoration Configuration")]
    [SerializeField] private FurnitureManager.DecorationCategory category;

    private const string defaultLayer = "Decoration";
    private const string ghostLayer = "Ghost";

    protected override int GetDefaultLayer() => LayerMask.NameToLayer(defaultLayer);
    protected override int GetGhostLayer() => LayerMask.NameToLayer(ghostLayer);

    public FurnitureManager.DecorationCategory GetCategory() => category;
}