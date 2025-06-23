using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Lines
{
    public string line;
    public Sprite sprite; 
    public float triggerDelay = 0f;
    public UnityEvent trigger;
}

public class TutorialCanvas : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private BoxCollider meshBoxCollider;
    [SerializeField] private Vector2 canvasDefaultSize;
    [SerializeField] private Vector2 canvasWithImageSize;
    [Header("Content")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text textPro;
    [SerializeField] private Image image;
    [SerializeField] private List<Lines> lines;
    private int currentIndex = 0;

    void Start()
    {
        if (lines.Count > 0) textPro.text = lines[0].line;
        backButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
        ChangeCanvasSize(canvasDefaultSize);
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
        StartCoroutine(DelayTrigger(lines[currentIndex].triggerDelay, lines[currentIndex].trigger));
    }

    private IEnumerator DelayTrigger(float delay, UnityEvent trigger)
    {
        yield return new WaitForSeconds(delay);
        trigger.Invoke();
    }

    private void UpdateButtons()
    {
        if (currentIndex > 0 && currentIndex != lines.Count - 1) backButton.gameObject.SetActive(true);
        else backButton.gameObject.SetActive(false);
        if (currentIndex < lines.Count - 1) nextButton.gameObject.SetActive(true);
        else nextButton.gameObject.SetActive(false);
    }

    private void ChangeCanvasSize(Vector2 size)
    {
        canvasRectTransform.sizeDelta = size;
        meshBoxCollider.size = new Vector3(size.x, size.y, 1);
    }

    public bool HasEnded() => currentIndex == lines.Count - 1;
}