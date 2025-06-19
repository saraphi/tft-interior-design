using UnityEngine;

public class Decoration : RoomObject
{
    [Header("Decoration Configuration")]
    [SerializeField] private FurnitureManager.DecorationCategory category;

    public override void Duplicate()
    {
        throw new System.NotImplementedException();
    }

    public override bool WouldCollide(Vector3 targetPosition, Quaternion targetRotation)
    {
        throw new System.NotImplementedException();
    }

    protected override int GetDefaultLayer()
    {
        throw new System.NotImplementedException();
    }

    protected override int GetGhostLayer()
    {
        throw new System.NotImplementedException();
    }

    public FurnitureManager.DecorationCategory GetCategory() => category;
}