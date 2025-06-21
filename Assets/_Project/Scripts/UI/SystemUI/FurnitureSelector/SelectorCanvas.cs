using System;
using System.Collections.Generic;
using TMPro;
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
}