using UnityEngine;

public class FurnitureOptionsFollower : MonoBehaviour
{
    [SerializeField] private Furniture furniture;
    [SerializeField] private float offsetExtra = 0.1f;
    [SerializeField] private float canvasHeightInWorld = 0.56f;

    public void PositionCanvas()
    {
        if (furniture == null) return;

        FurnitureModel furnitureModel = furniture.GetFurnitureModel();
        BoxCollider modelCollider = furnitureModel.GetCollider();
        if (modelCollider == null) return;

        Bounds bounds = modelCollider.bounds;
        Vector3 objectCenter = bounds.center;

        Vector3 toCamera = (Camera.main.transform.position - objectCenter).normalized;
        Vector3 toCameraFlat = new Vector3(toCamera.x, 0f, toCamera.z).normalized;

        float extentInCameraDir = Mathf.Abs(Vector3.Dot(bounds.extents, toCameraFlat));
        float totalOffset = extentInCameraDir + canvasHeightInWorld / 2f + offsetExtra;

        Vector3 candidatePos = objectCenter + toCameraFlat * totalOffset;
        candidatePos.y = Camera.main.transform.position.y;

        transform.localPosition = furniture.transform.InverseTransformPoint(candidatePos);
        transform.rotation = Quaternion.LookRotation(-toCameraFlat);
    }
}