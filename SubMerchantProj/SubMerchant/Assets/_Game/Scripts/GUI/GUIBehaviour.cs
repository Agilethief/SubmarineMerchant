using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class GUIBehaviour : BaseClientOnlyBehaviour
    {
       public RectTransform rt
        {
            get { return transform.GetComponent<RectTransform>(); }
        }

        CanvasGroup _cg;
        [HideInInspector]
        public CanvasGroup cg
        {
            get
            {
                if(_cg == null)
                {
                    _cg = transform.GetComponent<CanvasGroup>();
                }
                return _cg;
            }
            
        }


        public static PlayerUI _playerUI;
        public static PlayerUI GetPlayerUI
        {
            get
            {
                if (_playerUI == null) _playerUI = FindObjectOfType<PlayerUI>();

                return _playerUI;
            }
        }

        public virtual void CloseGUI() { Destroy(this.gameObject); }


        protected IEnumerator LerpScale(RectTransform targetObject, float newScale, float lerpSpeed)
		{
			Vector3 newScaleVec = Vector3.one * newScale;
			float distance = Vector3.Distance(targetObject.localScale, newScaleVec);

			while (distance > 0.05f)
			{
				distance = Vector3.Distance(targetObject.localScale, newScaleVec);

				targetObject.localScale = Vector3.MoveTowards(targetObject.localScale, newScaleVec, Time.deltaTime * lerpSpeed);
				targetObject.localScale = Vector3.Lerp(targetObject.localScale, newScaleVec, Time.deltaTime * lerpSpeed);
				yield return null;
			}

			targetObject.localScale = newScaleVec;

			yield return null;
		}

        protected IEnumerator LerpAnchoredPosition(RectTransform targetObject, Vector3 targetPosition, float lerpSpeed)
		{
			float distance = Vector3.Distance(targetObject.anchoredPosition, targetPosition);

			while (distance > 0.5f) // Remember this is gui space where 1 px is 1 unit.
			{
				distance = Vector3.Distance(targetObject.anchoredPosition, targetPosition);

				targetObject.anchoredPosition = Vector3.MoveTowards(targetObject.anchoredPosition, targetPosition, Time.deltaTime * lerpSpeed);
				targetObject.anchoredPosition = Vector3.Lerp(targetObject.anchoredPosition, targetPosition, Time.deltaTime * lerpSpeed);
				yield return null;
			}

			targetObject.localPosition = targetPosition;

			yield return null;
		}

        protected IEnumerator FadeCG(CanvasGroup targetCG, float targetValue, float duration)
        {
            float startVal = targetCG.alpha;
            float timer = 0;
            float normTime;
            while(timer <= duration)
            {
                timer += Time.deltaTime;
                normTime = Mathf.InverseLerp(0,duration, timer);

                targetCG.alpha = Mathf.Lerp(startVal,targetValue, normTime);
                
                yield return null;
            }

            targetCG.alpha = targetValue; // Force it to the desired target.

            yield return null;
        }
    }
}
