using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class simpleUIFader : MonoBehaviour
{
    Vector3 orgPosition;
    public Vector3 FadeDistance = new Vector3();
    public float duration = 0.5f;
    public float delay = 0.5f;
    CanvasGroup cg;
    public bool FadeChildren = false;
    bool fadein = true;

    private void Awake()
    {
        orgPosition = transform.localPosition;
        cg = GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        StartCoroutine(fadeinIE());
    }

    public void disableObject()
    {
        StartCoroutine(fadeout());
    }
    IEnumerator fadeinIE()
    {
        yield return new WaitForSeconds(delay);
        transform.localPosition = orgPosition - FadeDistance;
        transform.DOLocalMove(orgPosition, duration);
        fadein = true;
        childrenFading();
        cg.interactable = true;
        cg.blocksRaycasts = true;
        yield return null;
    }

    IEnumerator fadeout()
    {
        fadein = false;
        transform.DOLocalMove(orgPosition - FadeDistance, duration);
        childrenFading();
        cg.interactable = false;
        cg.blocksRaycasts = false;
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        yield return null;
    }
    private void childrenFading()
    {
        if (FadeChildren)
        {
            if (fadein)
            {
                int counter = 1;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    
                    foreach (var item in child.GetComponentsInChildren<Image>())
                    {
                        item.DOComplete();
                        Color newC = item.color;
                        newC.a = 0;
                        item.color = newC;
                        item.DOFade(1,duration*counter).SetDelay(duration);
                        item.transform.DOBlendableLocalMoveBy(-FadeDistance,0.1f);
                        item.transform.DOBlendableLocalMoveBy(FadeDistance,duration).SetDelay(duration*(counter/2f));
                        
                        TextMeshProUGUI itemChild = item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        if(itemChild != null)
                        {

                        itemChild.alpha = 0;
                        itemChild.DOFade(1, duration * counter).SetDelay(duration);
                        }
                        counter++;
                    }


                }
                cg.DOFade(1, duration);
            }
            else
            {
                cg.DOFade(0, duration);
            }
        }
        else
        {
            if (fadein)
            {
                cg.DOFade(1, duration);
            }
            else
            {
                cg.DOFade(0, duration);
            }
        }
    }
    private void OnDisable()
    {
        

    }
}
