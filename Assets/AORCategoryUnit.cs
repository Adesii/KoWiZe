using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class AORCategoryUnit : MonoBehaviour, IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    public Image UnitPicture;
    public TextMeshProUGUI UnitName;

    public BuildPanelMenu b;

    public Color DefaultColor;
    public Color HighlightColor;
    public Color UnavailableColor;
    public Color SelectedColor;


    public BaseUnit ownUnit;

    private bool isAvailable = true;

    public Image Overlay;

    public void SetAvailability(bool available)
    {
        if (available && !isAvailable)
        {
            Overlay.DOColor(DefaultColor,0.2f);
            isAvailable = true;
        }
        else
        {
            Overlay.DOColor(UnavailableColor,0.2f);
            isAvailable = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isAvailable && b.selectedUnit != ownUnit) return;
        b.selectedUnit = ownUnit;
        Overlay.DOColor(SelectedColor, 0.2f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAvailable) return;
        Overlay.DOColor(HighlightColor, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAvailable) return;
        Overlay.DOColor(DefaultColor, 0.2f);
    }
}
