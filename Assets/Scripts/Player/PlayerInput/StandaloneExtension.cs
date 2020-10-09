using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StandaloneExtension : StandaloneInputModule
{
    public PointerEventData GetPointerData() {
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        //bool pressed, released;
        //Debug.Log(GetTouchPointerEventData(Input.GetTouch(0), out pressed, out released).pointerCurrentRaycast.gameObject.name);
        //Debug.Log(m_PointerData[kMouseLeftId].pointerCurrentRaycast.gameObject.name);

        return null;
    }

}
