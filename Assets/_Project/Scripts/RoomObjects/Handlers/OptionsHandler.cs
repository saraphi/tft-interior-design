using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OptionsHandler : MonoBehaviour
{
    [SerializeField] protected GameObject optionsCanvas;
    protected RoomObject roomObject;

    void Awake()
    {
        roomObject = GetComponent<RoomObject>();    
    }

    public void HideOptionsCanvas()
    {
        optionsCanvas.SetActive(false);
    }

    public void ShowOptionsCanvas()
    {
        SoundManager.Instance.PlayEnterClip();
        GameManager.Instance.PositionCanvas(optionsCanvas, 1f);
        optionsCanvas.SetActive(true);
    }

    public void ToggleOptionsCanvas()
    {
        if (FurnitureManager.Instance.IsUsingObject()) return;
        
        int id = roomObject.GetID();

        if (FurnitureManager.Instance.IsObjectSelected(id))
        {
            FurnitureManager.Instance.DeselectObject();
            SoundManager.Instance.PlayExitClip();
        }
        else
        {
            FurnitureManager.Instance.SelectObject(id);
            ShowOptionsCanvas();
        }
    }

    public abstract void OnOptionSelected(string option);
}
