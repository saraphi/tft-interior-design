using UnityEngine;

public class FurnitureOptionsFollower : MonoBehaviour
{
    [SerializeField] private Furniture target;

    public void SetTarget(Furniture furniture) => target = furniture;

    void Update()
    {
        if (target == null || !gameObject.activeInHierarchy) return;

        Vector3 toCamera = (Camera.main.transform.position - target.transform.position).normalized;
        float offset = target.GetModelRenderer().bounds.extents.magnitude * 1.2f;
        transform.position = target.transform.position + toCamera * offset + target.transform.right * 0.5f;
        transform.rotation = Quaternion.LookRotation(-toCamera);
    }
}
