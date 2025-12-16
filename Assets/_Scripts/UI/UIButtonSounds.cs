using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayMouseOver();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayClick();
    }
}
