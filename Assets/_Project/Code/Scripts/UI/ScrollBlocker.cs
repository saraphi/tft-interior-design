using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ScrollRect scrollRect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        scrollRect.enabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        scrollRect.enabled = true;
    }
}