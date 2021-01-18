using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_City_Hover_prefab_Store : MonoBehaviour
{
    
    public TextMeshProUGUI CityName;
    public GameObject ResourceInstancing;
    public Transform ResourceParent;

    public citySystem ownCity;
    public Canvas sortingStuff;

    public float UIscale;

    
    private void OnEnable()
    {
        transform.DOScale(UIscale, 0.2f);
    }
    public void disableObject()
    {
        transform.DOScale(0, 0.2f).OnComplete(() => { gameObject.SetActive(false); });
    }

   
}
