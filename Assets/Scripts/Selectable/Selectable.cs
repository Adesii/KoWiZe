using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;
using DG.Tweening;

public abstract class Selectable : BuildableObject, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Selection Settings")]
    public bool isSelected = false;
    private GameObject SelectionPrefab;
    private VisualEffect vsF;
    public float SelectionRadius= 10f;
    public float playSpeed = 2f;
    public Vector3 unselectPunchRotation;
    
    public virtual void unSelect()
    {
        isSelected = false;
        vsF.Stop();
        vsF.playRate *= 2;
        SelectionPrefab.transform.DOBlendableLocalRotateBy(unselectPunchRotation,1f,RotateMode.FastBeyond360);
    }
    public virtual void Select()
    {
        if(SelectionPrefab == null)
        {
            SelectionPrefab = Instantiate(GameController.Instance.SelectionPrefab,transform);
            vsF = SelectionPrefab.GetComponent<VisualEffect>();
            vsF.SetFloat("Radius", SelectionRadius);
        }
        vsF.playRate = playSpeed;
        isSelected = true;
        vsF.Play();


    }

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
    public virtual void RightPointerClicked()
    {

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
        switch (eventData.button)   
        {
            case PointerEventData.InputButton.Left:
                PointerClicked();
                break;
            case PointerEventData.InputButton.Right:
                RightPointerClicked();
                break;
            case PointerEventData.InputButton.Middle:
                break;
            default:
                break;
        }
        
        
    }
}
