using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class toolTipTrigger : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public string header = "";
    [Multiline()]
    public string content = "";

    public float counter= 0;


    Coroutine coroutine;
    public void OnPointerEnter(PointerEventData eventData)
    {

        coroutine = StartCoroutine(tweenCallBack());
    }

    public IEnumerator tweenCallBack()
    {
        yield return new WaitForSeconds(0.5f);
        ToolTipSystem.Show(content, header);
        yield return null;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(coroutine);
        ToolTipSystem.Hide();
    }
}
