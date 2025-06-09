using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureSelectorCanvas : MonoBehaviour
{
    [SerializeField] private Transform toggleCategories;
    [SerializeField] private Toggle defaultCategoryToggle;

    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject defaultFurnitureOption;
    private Dictionary<string, Toggle> categoryToggles = new Dictionary<string, Toggle>();
    private string currentCategory = null;
    private List<GameObject> allCurrentFurniture = new List<GameObject>();
    private List<GameObject> allCurrentFurnitureButtons = new List<GameObject>();

    void Start()
    {
        List<string> categories = FurnitureManager.Instance.GetCategories();

        foreach (var category in categories)
        {
            string current = category;
            Toggle newToggle = Instantiate(defaultCategoryToggle, toggleCategories);
            TMP_Text buttonText = newToggle.GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = current;
            newToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) SetCategoryFilter(current);
                else if (currentCategory == current) SetCategoryFilter(null);
            });
            newToggle.gameObject.SetActive(true);
            categoryToggles.Add(current, newToggle);
            
            // string current = category;

            // Toggle newButton = Instantiate(defaultCategoryButton, buttonCategories);
            // TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            // if (buttonText != null) buttonText.text = current;
            // newButton.onClick.AddListener(() => SetCategoryFilter(current));
            // newButton.gameObject.SetActive(true);
            // categoryButtons.Add(current, newButton);
        }

        UpdateFurnitureListByCategory();
    }

    public void SetCategoryFilter(string category)
    {
        currentCategory = category;
        SoundManager.Instance.PlayPressClip();
        UpdateFurnitureListByCategory();
    }

    private void UpdateFurnitureListByCategory()
    {
        foreach (var button in allCurrentFurnitureButtons) Destroy(button);
        allCurrentFurnitureButtons.Clear();

        if (!string.IsNullOrEmpty(currentCategory) && Enum.TryParse(currentCategory, out FurnitureManager.FurnitureCategory parsedCategory))
            allCurrentFurniture = FurnitureManager.Instance.GetAllFurnitureByCategory(parsedCategory);
        else allCurrentFurniture = FurnitureManager.Instance.GetAllFurniture();

        foreach (var furniture in allCurrentFurniture)
        {
            Furniture furnitureComponent = furniture.GetComponent<Furniture>();
            GameObject newFurnitureButton = Instantiate(defaultFurnitureOption, scrollContent);
            FurnitureOption furnitureOption = newFurnitureButton.GetComponent<FurnitureOption>();
            furnitureOption.SetButtonText("ADD " + furnitureComponent.GetFurnitureName().ToUpper());
            Button button = furnitureOption.GetButton();
            button.onClick.AddListener(() => FurnitureManager.Instance.AddFurniture(furnitureComponent.GetFurnitureName()));
            allCurrentFurnitureButtons.Add(newFurnitureButton);
            newFurnitureButton.SetActive(true);
        }
    }
}