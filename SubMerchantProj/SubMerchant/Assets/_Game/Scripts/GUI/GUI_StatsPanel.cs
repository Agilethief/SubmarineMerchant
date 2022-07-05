using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CargoGame
{
    public class GUI_StatsPanel : GUIBehaviour
    {
        
        [SerializeField] private Image healthbarImage, waterbarImage, foodbarImage;

        float normalisedHealth, normalisedWater, normalisedFood;


        public void SetWater(float waterValue, float waterMax)
        {
            normalisedWater = Mathf.InverseLerp(0, waterMax, waterValue);

            waterbarImage.fillAmount = normalisedWater;
        }
        public void SetHealth(float healthValue, float healthMax)
        {
            normalisedHealth = Mathf.InverseLerp(0, healthMax, healthValue);

            healthbarImage.fillAmount = normalisedHealth;
        }
        public void SetFood(float foodValue, float foodMax)
        {
            normalisedFood = Mathf.InverseLerp(0, foodMax, foodValue);

            foodbarImage.fillAmount = normalisedFood;
        }

    }
}
