using UnityEngine;

public class FurnitureVisualHandler : MonoBehaviour
{
    private Furniture furniture;
    private Material material;
    private Color originalColor;

    void Awake()
    {
        furniture = GetComponent<Furniture>();
        if (furniture != null)
        {
            MeshRenderer renderer = furniture.GetModelRenderer();
            if (renderer != null)
            {
                material = new Material(renderer.material);
                renderer.material = material;
                originalColor = material.color;
            }
        }
    }

    public void SetAlpha(float alpha, Color? overrideColor = null)
    {
        if (material == null) return;
        Color color = overrideColor ?? originalColor;
        color.a = alpha;
        material.color = color;
    }
}
