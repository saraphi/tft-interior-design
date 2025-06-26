using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorFurnitureOptions : SelectorOptions
{
    protected override void Start()
    {
        List<string> furnitureCategories = FurnitureManager.Instance.GetAllFurnitureCategories();
        SetData(furnitureCategories);
    }

    protected override void UpdateRoomObjectListByCategory()
    {
        foreach (var button in allCurrentRoomObjectsButtons) Destroy(button);
        allCurrentRoomObjectsButtons.Clear();

        if (!string.IsNullOrEmpty(currentCategory) && Enum.TryParse(currentCategory, out FurnitureManager.FurnitureCategory parsedCategory))
            allCurrentRoomObjects = FurnitureManager.Instance.GetAllFurnitureByCategory(parsedCategory);
        else allCurrentRoomObjects = FurnitureManager.Instance.GetAllFurniture();

        foreach (var furniture in allCurrentRoomObjects)
        {
            Furniture furnitureComponent = furniture.GetComponent<Furniture>();
            GameObject newFurnitureButton = Instantiate(defaultSelectorRoomObject, scrollContent);
            SelectorRoomObject SelectorRoomObject = newFurnitureButton.GetComponent<SelectorRoomObject>();

            SelectorRoomObject.SetButtonText(furnitureComponent.GetName());
            SelectorRoomObject.Init(furnitureComponent);

            Button button = SelectorRoomObject.GetButton();
            button.onClick.AddListener(() => FurnitureManager.Instance.AddObject(furnitureComponent.GetCodeName(), SelectorRoomObject.GetProfileColor()));

            allCurrentRoomObjectsButtons.Add(newFurnitureButton);
            newFurnitureButton.SetActive(true);
        }
    }
}
