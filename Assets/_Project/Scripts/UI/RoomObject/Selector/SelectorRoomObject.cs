using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorRoomObject : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text roomObjectTitle;
    [SerializeField] private Transform colorToggleContainer;
    [SerializeField] private Toggle defaultColorToggle;
    private Model model;
    private string currentProfileColor;

    public void Init(RoomObject roomObject)
    {
        model = roomObject.GetModel();

        List<ColorProfile> profiles = model.GetColorProfiles();

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

        Sprite sprite = roomObject.GetImageSprite();
        Image image = button.GetComponent<Image>();
        image.sprite = sprite;
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
        if (roomObjectTitle == null) roomObjectTitle = GetComponentInChildren<TMP_Text>();
        roomObjectTitle.text = text;
    }
}