using UnityEngine;

public class FurnitureVisualHandler : MonoBehaviour
{
    private Furniture furniture;
    private Material material;

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
            }
        }
    }

    public void SetAlpha(float alpha)
    {
        if (material == null) return;

        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }
}
