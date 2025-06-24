using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorDecorationsOptions : SelectorOptions
{
    protected override void Start()
    {
        List<string> decorationsCategories = FurnitureManager.Instance.GetAllDecorationsCategories();
        SetData(decorationsCategories);
    }

    protected override void UpdateRoomObjectListByCategory()
    {
        foreach (var button in allCurrentRoomObjectsButtons) Destroy(button);
        allCurrentRoomObjectsButtons.Clear();

        if (!string.IsNullOrEmpty(currentCategory) && Enum.TryParse(currentCategory, out FurnitureManager.DecorationCategory parsedCategory))
            allCurrentRoomObjects = FurnitureManager.Instance.GetAllDecorationsByCategory(parsedCategory);
        else allCurrentRoomObjects = FurnitureManager.Instance.GetAllDecorations();

        foreach (var decoration in allCurrentRoomObjects)
        {
            Decoration decorationComponent = decoration.GetComponent<Decoration>();
            GameObject newDecorationButton = Instantiate(defaultSelectorRoomObject, scrollContent);
            SelectorRoomObject SelectorRoomObject = newDecorationButton.GetComponent<SelectorRoomObject>();

            SelectorRoomObject.SetButtonText(decorationComponent.GetName());
            SelectorRoomObject.Init(decorationComponent);

            Button button = SelectorRoomObject.GetButton();
            button.onClick.AddListener(() => FurnitureManager.Instance.AddObject(decorationComponent.GetCodeName(), SelectorRoomObject.GetProfileColor()));

            allCurrentRoomObjectsButtons.Add(newDecorationButton);
            newDecorationButton.SetActive(true);
        }
    }
}
