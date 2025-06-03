using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FurnitureModel : MonoBehaviour
{
    [SerializeField] private List<GameObject> children;
    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material fresnelMaterial;

    private List<MeshRenderer> meshRenderers = new();

    void Awake()
    {
        foreach (var child in children)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = opaqueMaterial;
                meshRenderers.Add(meshRenderer);
            }
        }
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
    }

    public void SetModelMaterialsTransparency(bool isTransparent)
    {
        Material targetMaterial = isTransparent ? transparentMaterial : opaqueMaterial;

        foreach (var renderer in meshRenderers)
            renderer.material = targetMaterial;
    }

    public void EnableFresnelHighlight()
    {
        foreach (var renderer in meshRenderers)
            renderer.material = fresnelMaterial;
    }

    public void DisableFresnelHighlight()
    {
        foreach (var renderer in meshRenderers)
            renderer.material = opaqueMaterial;
    }

    public void SetColliderEnabled(bool enabled)
    {
        boxCollider.enabled = enabled;
    }

    public List<MeshRenderer> GetMeshRenderers() => meshRenderers;
    public BoxCollider GetCollider() => boxCollider;
}