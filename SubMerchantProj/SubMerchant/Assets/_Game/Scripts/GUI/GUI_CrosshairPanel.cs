using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CargoGame
{
    public class GUI_CrosshairPanel : GUIBehaviour
    {
        
        public Image crosshairImage;
        public Image crosshairCompletionCircle;
        public Sprite defaultCrosshair;

        public void SetCrossHairSprite(Sprite crossHairSprite, Color spriteColour)
        {
            crosshairImage.sprite = crossHairSprite;
            crosshairImage.color = spriteColour;
        }

        public void RevealCrosshair()
        {
            crosshairImage.gameObject.SetActive(true);
            crosshairCompletionCircle.gameObject.SetActive(false);
            crosshairImage.rectTransform.rotation = Quaternion.identity;

        }
        public void RevealCompletionCircle()
        {
            crosshairImage.gameObject.SetActive(false);
            crosshairCompletionCircle.gameObject.SetActive(true);
        }

        public void ClearCrosshair()
        {
            crosshairImage.sprite = defaultCrosshair;
            crosshairImage.color = Color.white;
            crosshairImage.rectTransform.rotation = Quaternion.identity;

            crosshairImage.gameObject.SetActive(true);
            crosshairCompletionCircle.gameObject.SetActive(false);
            crosshairCompletionCircle.rectTransform.rotation = Quaternion.identity;
        }

         public void HideCrosshair()
        {
            crosshairImage.sprite = null;
            crosshairImage.color = Color.clear;
            crosshairImage.rectTransform.rotation = Quaternion.identity;

            crosshairImage.gameObject.SetActive(true);
            crosshairCompletionCircle.gameObject.SetActive(false);
            crosshairCompletionCircle.rectTransform.rotation = Quaternion.identity;
        }

        public void RotateCrosshair(float rotateAmount)
        {
            crosshairImage.transform.Rotate(0,0,rotateAmount);
        }

    }
}
