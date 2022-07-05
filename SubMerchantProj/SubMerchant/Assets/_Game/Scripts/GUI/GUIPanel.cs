using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CargoGame
{
    public class GUIPanel : GUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {

        public virtual void SetupPanel(Vector2 screenPos)
        {
            rt.anchoredPosition = screenPos;
        }
        public virtual void SetupPanel(float x, float y)
        {
            rt.anchoredPosition = new Vector2(x,y);
        }

        // Mouse over stuff. May not be working silky
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (debugThisObject) Debug.Log("Pointer entered: " + gameObject.name);
        }


        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (debugThisObject) Debug.Log("Pointer exited: " + gameObject.name);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (debugThisObject) Debug.Log("Pointer down: " + gameObject.name);
        }


        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (debugThisObject) Debug.Log("Pointer up: " + gameObject.name);
        }

    }
}
