using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CargoGame
{
    public class PostFXVolume : BaseClientOnlyBehaviour
    {
        
        [SerializeField] private Volume fxVolume;


        public void CreateNewVolume(VolumeProfile profile, float duration)
        {
            fxVolume.weight = 0;
            fxVolume.profile = profile;

            // Start fade routine
            StopAllCoroutines();
            StartCoroutine(FadeVolume(1, duration));
        }

        public void FadeUpVolume(float duration)
        {
             // Start fade routine
            StopAllCoroutines();
            StartCoroutine(FadeVolume(1, duration));
        }

        public void ClearVolume(float clearDuration, bool destroyThis)
        {
            // Start fade routine
            StopAllCoroutines();
            StartCoroutine(FadeVolume(0, clearDuration, destroyThis));
        }

        IEnumerator FadeVolume(float targetVal, float duration, bool destroyOnFinish = false)
        {
            float startingVal = fxVolume.weight;
            float timer = 0;
            float normalisedTime = 0;

            while(timer <= duration)
            {
                timer+= Time.deltaTime;

                normalisedTime = Mathf.InverseLerp(startingVal, targetVal, timer);

                fxVolume.weight = normalisedTime;

                yield return null;
            }

             fxVolume.weight = targetVal;

            if(destroyOnFinish)
                Destroy(this.gameObject);

            yield return null;
        }

    }
}
