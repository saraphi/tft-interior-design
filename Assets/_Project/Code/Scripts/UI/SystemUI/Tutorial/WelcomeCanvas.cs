using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeCanvas : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text textPro;

    [SerializeField] private List<string> lines;
    private int currentIndex = 0;

    void Start()
    {
        if (lines.Count > 0) textPro.text = lines[0];
        backButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
    }

    public void OnNext()
    {
        SoundManager.Instance.PlayPressClip();
        currentIndex++;
        if (lines.Count > 0 && currentIndex < lines.Count) textPro.text = lines[currentIndex];
        UpdateButtons();
    }

    public void OnBack()
    {
        SoundManager.Instance.PlayExitClip();
        currentIndex--;
        if (lines.Count > 0 && currentIndex >= 0) textPro.text = lines[currentIndex];
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        if (currentIndex > 0) backButton.gameObject.SetActive(true);
        else backButton.gameObject.SetActive(false);
        if (currentIndex < lines.Count - 1) nextButton.gameObject.SetActive(true);
        else nextButton.gameObject.SetActive(false);
    }
}