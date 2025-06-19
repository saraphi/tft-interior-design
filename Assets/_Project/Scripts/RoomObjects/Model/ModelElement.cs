using UnityEngine;

public class ModelElement : MonoBehaviour
{
    [SerializeField] private string elementID;
    [SerializeField] private Collider elementCollider;

    [SerializeField] private MeshRenderer meshRenderer;
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

    public void SetOpaque() => meshRenderer.material = opaque;
    public void SetTransparent() => meshRenderer.material = transparent;
    public void SetFresnel() => meshRenderer.material = fresnel;
}