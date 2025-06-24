using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorOption
{
    public string profileName;
    public GameObject color;
}

public class ChangeColorCanvas : MonoBehaviour
{
    [SerializeField] private RoomObject roomObject;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject defaultColor;
    [SerializeField] private Vector2 defaultSize;
    [SerializeField] private Vector2 selectedSize;
    [SerializeField] private float threshold = 0.6f;
    [SerializeField] private int limit = 7;
    [SerializeField] protected float cooldown = 0.1f;

    private List<ColorOption> colors = new List<ColorOption>();

    private int selectedColorProfile;
    protected float lastMoveTime = -Mathf.Infinity;

    void Start()
    {
        defaultColor.SetActive(false);
        List<ColorProfile> colorProfiles = roomObject.GetModel().GetColorProfiles();
        string currentProfileColor = roomObject.GetModel().GetFurnitureColorProfile().profileName;
        int i = 0;
        foreach (ColorProfile colorProfile in colorProfiles)
        {
            if (i > limit) break;
            Gradient gradient = colorProfile.colorIdentifier;
            Sprite sprite = GradientUtils.CreateGradientSprite(gradient);
            GameObject newColor = Instantiate(defaultColor, parent);
            RectTransform rectTransform = newColor.GetComponent<RectTransform>();
            if (currentProfileColor != colorProfile.profileName) rectTransform.sizeDelta = defaultSize;
            else
            {
                rectTransform.sizeDelta = selectedSize;
                selectedColorProfile = i;
            }
            Image image = newColor.GetComponent<Image>();
            image.sprite = sprite;
            newColor.SetActive(true);
            colors.Add(new ColorOption() { profileName = colorProfile.profileName, color = newColor });
            i++;
        }
    }

    public void ChangeColor()
    {
        if (Time.time - lastMoveTime < cooldown) return;

        Vector2 joystickInput = ControllerManager.Instance.GetSecondaryControllerJoystickInput();
        int sum = 0;

        if (Mathf.Abs(joystickInput.x) >= threshold && Mathf.Sign(joystickInput.x) > 0) sum = 1;
        else if (Mathf.Abs(joystickInput.x) >= threshold && Mathf.Sign(joystickInput.x) < 0) sum = -1;

        if (sum != 0 && selectedColorProfile + sum < colors.Count && selectedColorProfile + sum >= 0)
        {
            SoundManager.Instance.PlayPressClip();
            ChangeSelectedColor(selectedColorProfile + sum);
        }

        lastMoveTime = Time.time;
    }

    private void ChangeSelectedColor(int index)
    {
        if (selectedColorProfile == index) return;
        RectTransform oldRectTransform = colors[selectedColorProfile].color.GetComponent<RectTransform>();
        RectTransform newRectTransform = colors[index].color.GetComponent<RectTransform>();
        oldRectTransform.sizeDelta = defaultSize;
        newRectTransform.sizeDelta = selectedSize;
        selectedColorProfile = index;
        ColorOption option = colors[index];
        roomObject.GetModel().ApplyColorProfile(option.profileName);
    }
}