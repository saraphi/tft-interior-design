using UnityEngine;

public class FurnitureUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject optionsCanvas;

    [Header("Offsets")]
    [SerializeField] private float sizeMultiplier = 1.2f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float localRightOffset = 0.5f;

    public void ToggleOptionsCanvas()
    {
        if (optionsCanvas.activeInHierarchy)
        {
            SoundManager.Instance.PlayExitSound();
            optionsCanvas.SetActive(false);
        }
        else
        {
            Transform cameraTransform = Camera.main.transform;
            Transform furnitureTransform = transform;

            Vector3 toCamera = (cameraTransform.position - furnitureTransform.position).normalized;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Bounds combinedBounds = new Bounds(furnitureTransform.position, Vector3.zero);

            foreach (var rend in renderers) combinedBounds.Encapsulate(rend.bounds);

            float size = combinedBounds.extents.magnitude;

            Vector3 targetPosition =
                furnitureTransform.position
                + toCamera * (size * sizeMultiplier)
                + furnitureTransform.right * localRightOffset;

            optionsCanvas.transform.position = targetPosition;
            optionsCanvas.transform.rotation = Quaternion.LookRotation(-toCamera);

            optionsCanvas.SetActive(true);
            SoundManager.Instance.PlayEnterSound();
        }
    }
}