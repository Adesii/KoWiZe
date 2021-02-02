using UnityEngine;
using UnityEngine.EventSystems;

internal class StandaloneInputModuleV2 : StandaloneInputModule
{
    public GameObject GameObjectUnderPointer(int pointerId)
    {
        var lastPointer = GetLastPointerEventData(pointerId);
        if (lastPointer != null)
            return lastPointer.pointerCurrentRaycast.gameObject;
        return null;
    }

    public GameObject GameObjectUnderPointer()
    {
        return GameObjectUnderPointer(PointerInputModule.kMouseLeftId);
    }
}