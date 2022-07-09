using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CargoGame
{
    public class GUI_MenuPage : GUIBehaviour
    {
        public Button bClose;
        public GUI_MenuPage previousPage;

        IEnumerator animRoutine;

        public virtual void Start()
        {
            if(bClose != null)
                bClose.onClick.AddListener(ExitPage);
        }

        public void ExitPage()
        {
            ClosePage();

            if (previousPage != null)
                previousPage.OpenPage();
        }

        public void ClosePage()
        {
            if(animRoutine != null)
                StopCoroutine(animRoutine);
            animRoutine = CloseRoutine();
            StartCoroutine(animRoutine);
        }

        IEnumerator CloseRoutine()
        {
            cg.blocksRaycasts = false;
            cg.interactable = false;
            yield return StartCoroutine(FadeCG(cg, 0, 0.4f));

            //gameObject.SetActive(false);

            yield return null;
        }

        public virtual void OpenPage()
        {
            if(animRoutine != null)
                StopCoroutine(animRoutine);
            animRoutine = OpenRoutine();
            StartCoroutine(animRoutine);
        }

         IEnumerator OpenRoutine()
        {
            //gameObject.SetActive(true);
            cg.blocksRaycasts = true;
            cg.interactable = true;
            yield return StartCoroutine(FadeCG(cg, 1, 0.4f));

            yield return null;
        }
    }
}
