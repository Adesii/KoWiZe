using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class sideBarNotification : MonoBehaviour
{
    Sequence sUI;

    bool hasClicked = false;
    // Start is called before the first frame update
    void Start()
    {
        sUI= DOTween.Sequence();
        transform.localPosition = new Vector3(100, 0);
        transform.localScale = new Vector3(0, 0, 0);
        
        sUI.Append(transform.DOLocalMoveX(0, 0.4f)).Join(transform.DOScale(1,0.6f)).PrependInterval(1).SetAutoKill(false);
        sUI.Play();
    }

    public void kill()
    {
        print("killing");
        if (!hasClicked)
        {
            hasClicked = true;
        UIEventManagerAndNotifier.moveDown(gameObject.transform.parent.gameObject);
        TweenCallback sc = new TweenCallback(Killyoursef);
        sUI.OnPause(Killyoursef);
        sUI.PlayBackwards();
        }

    }
    public void Killyoursef()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
