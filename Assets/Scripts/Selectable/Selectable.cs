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

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isBuilding)
            GameController.Instance.localSettings.localPlayer.AddToSelection(this);
    }
}
