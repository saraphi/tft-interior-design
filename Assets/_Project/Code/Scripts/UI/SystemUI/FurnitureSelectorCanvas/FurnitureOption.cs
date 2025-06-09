using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureOption : MonoBehaviour
{
    [SerializeField] private Button button;
    private TMP_Text buttonText;

    public Button GetButton() => button;
    public void SetButtonText(string text)
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TMP_Text>();
        buttonText.text = text;
    }
}
