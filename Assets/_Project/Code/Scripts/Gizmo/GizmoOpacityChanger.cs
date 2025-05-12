using UnityEngine;

public class GizmoOpacityChanger : MonoBehaviour
{
    [SerializeField] private MeshRenderer arrowMeshRenderer;
    [SerializeField] private MeshRenderer coneMeshRenderer;
    private Material material;

    private void Awake()
    {
        if (arrowMeshRenderer != null && arrowMeshRenderer != null)
        {
            material = new Material(arrowMeshRenderer.material);
            arrowMeshRenderer.material = material;
            if (coneMeshRenderer != null) coneMeshRenderer.material = material;
        }
    }

    public void OnHover()
    {
        SetAlpha(1f);
    }

    public void OnUnhover()
    {
        SetAlpha(100f / 255f);
    }

    private void SetAlpha(float alpha)
    {
        if (material == null) return;
        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }
}