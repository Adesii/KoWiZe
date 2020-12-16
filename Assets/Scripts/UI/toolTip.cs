using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class toolTip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contetnField;
    public LayoutElement layoutElement;
    public RectTransform rectTransform;

    public Vector2 offset;

    public int characterWrapLimit;
    public Canvas can;
    Vector2 position;
    Vector2 pivot = new Vector2();

    float pivotX;
    float pivotY;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        position = Input.mousePosition;
    }

    private void LateUpdate()
    {
        if (Application.isEditor)
        {
            int headerLenght = headerField.text.Length;
            int contentLenght = contetnField.text.Length;

            layoutElement.enabled = (headerLenght > characterWrapLimit || contentLenght > characterWrapLimit);
        }


        
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        position = Input.mousePosition;

        bool noneOutside = true;
        foreach (Vector3 item in corners)
        {
            
            if (!can.pixelRect.Contains(item))
            {
                noneOutside = false;
            }
        }
        if (!noneOutside)
        {
            float pivotx = position.x / can.pixelRect.width;
            float pivoty = position.y / can.pixelRect.height;
            pivot = Vector2.Lerp(pivot, new Vector2(pivotx, pivoty), Time.deltaTime * 2);
            position = new Vector2(Mathf.Clamp(position.x, 0, can.pixelRect.width), Mathf.Clamp(position.y, 0, can.pixelRect.height));
            //    pivotX = Mathf.Lerp(pivotX, 0, Time.deltaTime);
            //  pivotY = Mathf.Lerp(pivotY, 0, Time.deltaTime);
        }
        rectTransform.pivot = pivot;

        transform.position = position;


    }
    public void SetText(string Content,string Header = "")
    {
        if (string.IsNullOrEmpty(Header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = Header;
        }
        contetnField.text = Content;

        int headerLenght = headerField.text.Length;
        int contentLenght = contetnField.text.Length;

        layoutElement.enabled = (headerLenght > characterWrapLimit || contentLenght > characterWrapLimit);
    }
}
