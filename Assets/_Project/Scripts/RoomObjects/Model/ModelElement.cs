using UnityEngine;

public class ModelElement : MonoBehaviour
{
    [SerializeField] private string elementID;
    [SerializeField] private Collider elementCollider;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int materialsNumber = 1;
    private Material opaque;
    private Material transparent;
    private Material fresnel;
    private float transparentAlpha;

    public void SetMaterialsTemplate(Material opaqueTemplate, Material transparentTemplate, Material fresnelTemplate)
    {
        opaque = new Material(opaqueTemplate);
        transparent = new Material(transparentTemplate);
        transparentAlpha = transparent.color.a;
        fresnel = new Material(fresnelTemplate);
    }

    public void SetColor(Color color)
    {
        if (opaque != null) opaque.color = color;
        if (transparent != null)
        {
            Color newColor = color;
            newColor.a = transparentAlpha;
            transparent.color = newColor;
        }
        if (fresnel != null) fresnel.SetColor("_BaseColor", color);
    }

    public void SetColliderEnabled(bool enabled)
    {
        if (elementCollider != null) elementCollider.enabled = enabled;
    }

    public string GetElementID() => elementID;

    public void SetOpaque()
    {
        if (materialsNumber == 1) meshRenderer.material = opaque;
        else if (materialsNumber > 1)
        {
            Material[] materials = new Material[materialsNumber];
            for (int i = 0; i < materialsNumber; i++) materials[i] = opaque;
            meshRenderer.materials = materials;
        }
    }

    public void SetTransparent()
    {
        if (materialsNumber == 1) meshRenderer.material = transparent;
        else if (materialsNumber > 1)
        {
            Material[] materials = new Material[materialsNumber];
            for (int i = 0; i < materialsNumber; i++) materials[i] = transparent;
            meshRenderer.materials = materials;
        }
    }

    public void SetFresnel()
    {
        if (materialsNumber == 1) meshRenderer.material = fresnel;
        else if (materialsNumber > 1)
        {
            Material[] materials = new Material[materialsNumber];
            for (int i = 0; i < materialsNumber; i++) materials[i] = fresnel;
            meshRenderer.materials = materials;
        }
    }
}