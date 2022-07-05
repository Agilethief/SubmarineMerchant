using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CargoGame
{

	/// <summary>
	/// A generic script for buttons in the game
	/// </summary>
	public class GUI_GenericButton : GUIBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public float mouseOverScaleBoost = 1.1f;
        public bool scaleUp = true;

		Selectable mySelectable;

		private void Start()
		{
            mySelectable = transform.GetComponent<Selectable>();
		}

		// Mouse over stuff. May not be working silky
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (mySelectable.interactable) // Ensure the button is interactable
			{
                if (scaleUp)
                {
                    StopAllCoroutines();
                    StartCoroutine(LerpScale(GetComponent<RectTransform>(), mouseOverScaleBoost, 3));
                }
				//audioController.uiMenuElementChange.Play();

				//uiCursorHighlight.gameObject.SetActive(true);

				//selection = this.gameObject;
			}
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			if (mySelectable.interactable)
			{
                if (scaleUp)
                {
                    StopAllCoroutines();
                    StartCoroutine(LerpScale(GetComponent<RectTransform>(), 1f, 1.5f));
                }
			}
		}
	}
}