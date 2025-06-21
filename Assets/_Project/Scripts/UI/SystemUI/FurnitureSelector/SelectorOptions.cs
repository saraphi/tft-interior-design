using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SelectorOptions : MonoBehaviour
{
    [Header("Category Toggle UI")]
    [SerializeField] protected Transform toggleCategories;
    [SerializeField] protected Toggle defaultCategoryToggle;

    [Header("Furniture Option UI")]
    [SerializeField] protected Transform scrollContent;
    [SerializeField] protected GameObject defaultSelectorRoomObject;

    protected Dictionary<string, Toggle> categoryToggles = new Dictionary<string, Toggle>();
    protected string currentCategory = null;

    protected List<GameObject> allCurrentRoomObjects = new List<GameObject>();
    protected List<GameObject> allCurrentRoomObjectsButtons = new List<GameObject>();

    protected abstract void Start();

    protected void SetData(List<string> categories)
    {
        foreach (var category in categories)
        {
            string current = category;
            Toggle newToggle = Instantiate(defaultCategoryToggle, toggleCategories);
            TMP_Text buttonText = newToggle.GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = current;

            newToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn && currentCategory != current) SetCategoryFilter(current);
                else if (!isOn && currentCategory == current ) SetCategoryFilter(null);
            });

            newToggle.gameObject.SetActive(true);
            categoryToggles.Add(current, newToggle);
        }

        UpdateRoomObjectListByCategory();
    }

    public void SetCategoryFilter(string category)
    {
        if (currentCategory == category) return;

        currentCategory = category;
        SoundManager.Instance.PlayPressClip();
        UpdateRoomObjectListByCategory();
    }

    protected abstract void UpdateRoomObjectListByCategory();
}
