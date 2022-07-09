using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CargoGame
{
    public class GUI_WorldGenerationPage : GUI_MenuPage
    {
        public TMP_Text worldTitle, statusText;
        public Image progressSlider;

        float targetFill;
        public RawImage displayTexture;


        public void UpdateStatus(string newStatus)
        {
            statusText.text = newStatus;
        }

        public void SetProgressSlider(float progress)
        {
            targetFill = progress;
        }

        private void Update()
        {
            progressSlider.fillAmount = Mathf.Lerp(progressSlider.fillAmount, targetFill, Time.deltaTime * 2f);
        }

        public void SetDisplayTexture(Texture2D incomingTex)
        {
            displayTexture.texture = incomingTex;
        }

    }
}
