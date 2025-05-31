using UnityEngine;

public class FurnitureModel : MonoBehaviour
{
    [SerializeField] private MeshCollider meshCollider;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public MeshRenderer GetMeshRenderer() => meshRenderer;
    public Collider GetCollider() => meshCollider;
}