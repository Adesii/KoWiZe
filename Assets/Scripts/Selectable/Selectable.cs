using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    public bool isSelected = false;

    public abstract void unSelect();
    public abstract void Select();
}
