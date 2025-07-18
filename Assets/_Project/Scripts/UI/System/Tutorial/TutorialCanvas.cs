using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Line
{
    public string line;
    public Sprite sprite; 
    public float triggerDelay = 0f;
}

public class TutorialCanvas : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private BoxCollider meshBoxCollider;
    [SerializeField] private Vector2 canvasDefaultSize;
    [SerializeField] private Vector2 canvasWithImageSize;
    [Header("Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button oKButton;
    [SerializeField] private bool showOKButton = false;

    [Header("Lines")]
    [SerializeField] private TMP_Text textPro;
    [SerializeField] private Image image;
    [SerializeField] private Transform dotList;
    [SerializeField] private Image defaultDot;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color indexColor;
    [SerializeField] private List<Line> lines;
    private int currentIndex = 0;
    private Image[] dots;

    void Start()
    {
        if (lines.Count <= 0) return;
        UpdateButtons();
        dots = new Image[lines.Count];
        for (int i = 0; i < dots.Length; i++)
        {
            Image newDot = Instantiate(defaultDot, dotList);
            newDot.color = defaultColor;
            newDot.gameObject.SetActive(true);
            dots[i] = newDot;
        }
        UpdateLine();
    }

    public void OnOK()
    {
        SoundManager.Instance.PlayPressClip();
        TutorialManager.Instance.CloseCurrentTutorialCanvas();
    }

    public void OnNext()
    {
        SoundManager.Instance.PlayPressClip();
        currentIndex++;
        if (lines.Count > 0 && currentIndex < lines.Count) UpdateLine();
        UpdateButtons();
    }

    public void OnBack()
    {
        SoundManager.Instance.PlayExitClip();
        currentIndex--;
        if (lines.Count > 0 && currentIndex >= 0) UpdateLine();
        UpdateButtons();
    }

    private void UpdateLine()
    {
        textPro.text = lines[currentIndex].line;
        UpdateDots();
        dots[currentIndex].color = indexColor;
        if (lines[currentIndex].sprite != null)
        {
            image.sprite = lines[currentIndex].sprite;
            ChangeCanvasSize(canvasWithImageSize);
            image.gameObject.SetActive(true);
        }
        else
        {
            ChangeCanvasSize(canvasDefaultSize);
            image.gameObject.SetActive(false);
        }
    }

    private void UpdateDots()
    {
        foreach (var dot in dots) dot.color = defaultColor;
    }

    private void UpdateButtons()
    {
        if (currentIndex > 0 && currentIndex != lines.Count - 1) backButton.gameObject.SetActive(true);
        else backButton.gameObject.SetActive(false);
        if (currentIndex < lines.Count - 1)
        {
            oKButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            if (showOKButton) oKButton.gameObject.SetActive(true);
        }
    }

    private void ChangeCanvasSize(Vector2 size)
    {
        canvasRectTransform.sizeDelta = size;
        meshBoxCollider.size = new Vector3(size.x, size.y, 1);
    }

    public bool HasEnded() => (currentIndex == lines.Count - 1 && !showOKButton) || !gameObject.activeInHierarchy;
}