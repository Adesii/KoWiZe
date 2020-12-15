using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Selectable : BuildableObject, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool isSelected = false;

    public abstract void unSelect();
    public abstract void Select();

    public virtual void PointerEntered()
    {

    }
    public virtual void PointerExited()
    {

    }
    public virtual void PointerClicked()
    {
        if (!isBuilding)
            GameController.Instance.localSettings.localPlayer.AddToSelection(this);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEntered();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExited();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClicked();
        
    }
}
