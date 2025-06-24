using UnityEngine;
using UnityEngine.UI;

public class SelectorCanvas : MonoBehaviour
{
    [Header("Furniture")]
    [SerializeField] private SelectorFurnitureOptions furnitureOptions;
    [SerializeField] private Toggle furnitureToggle;

    [Header("Decorations")]
    [SerializeField] private SelectorDecorationsOptions decorationsOptions;
    [SerializeField] private Toggle decorationsToggle;

    [Header("Confirmation Dialog")]
    [SerializeField] private Button removeAllButton;
    [SerializeField] private GameObject removeAllCanvas;

    void Start()
    {
        furnitureToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn && !furnitureOptions.gameObject.activeInHierarchy) OnCurrentOptionsChange("furniture");
        });

        decorationsToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn && !decorationsOptions.gameObject.activeInHierarchy) OnCurrentOptionsChange("decorations");
        });
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            int addedRoomObjectsCount = FurnitureManager.Instance.GetAllAddedRoomObjects().Count;
            if (addedRoomObjectsCount != 0 && !removeAllButton.interactable) removeAllButton.interactable = true;
            else if (addedRoomObjectsCount == 0 && removeAllButton.interactable) removeAllButton.interactable = false;
        }
    }

    private void OnCurrentOptionsChange(string option)
    {
        SoundManager.Instance.PlayPressClip();
        switch (option)
        {
            case "furniture":
                decorationsOptions.gameObject.SetActive(false);
                furnitureOptions.gameObject.SetActive(true);
                break;
            case "decorations":
                furnitureOptions.gameObject.SetActive(false);
                decorationsOptions.gameObject.SetActive(true);
                break;
        }
    }

    public void OnRemoveAll()
    {
        if (!removeAllCanvas.activeInHierarchy)
        {
            SoundManager.Instance.PlayPressClip();
            removeAllCanvas.SetActive(true);
        }
    }

    public void OnRemoveAllCancel()
    {
        if (removeAllCanvas.activeInHierarchy)
        {
            SoundManager.Instance.PlayExitClip();
            removeAllCanvas.SetActive(false);
        }
    }

    public void OnRemoveAllConfirmation()
    {
        if (removeAllCanvas.activeInHierarchy)
        {
            SoundManager.Instance.PlayPressClip();
            FurnitureManager.Instance.DeleteAllObjects();
            removeAllCanvas.SetActive(false);
        }
    }
}