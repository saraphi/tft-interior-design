using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureOption : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Transform colorToggleContainer;
    [SerializeField] private Toggle defaultColorToggle;

    private TMP_Text buttonText;
    private FurnitureModel model;
    private string currentProfileColor;

    public void Init(Furniture furniture)
    {
        model = furniture.GetFurnitureModel();

        List<FurnitureColorProfile> profiles = model.GetColorProfiles();

        foreach (var profile in profiles)
        {
            Toggle newToggle = Instantiate(defaultColorToggle, colorToggleContainer);
            newToggle.group = colorToggleContainer.GetComponent<ToggleGroup>();

            var bgImage = newToggle.transform.Find("Background").GetComponent<Image>();
            bgImage.sprite = GradientUtils.CreateGradientSprite(profile.colorIdentifier);
            bgImage.color = Color.white;

            newToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn && currentProfileColor != profile.profileName) SetProfileColor(profile.profileName);
            });

            newToggle.gameObject.SetActive(true);
        }

        if (profiles.Count != 0) currentProfileColor = profiles[0].profileName;
        defaultColorToggle.gameObject.SetActive(false);
    }

    public void SetProfileColor(string profileColor)
    {
        if (currentProfileColor == profileColor) return;
        currentProfileColor = profileColor;
        SoundManager.Instance.PlayPressClip();
    }

    public string GetProfileColor() => currentProfileColor;

    public Button GetButton() => button;
    public void SetButtonText(string text)
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TMP_Text>();
        buttonText.text = text;
    }
}
