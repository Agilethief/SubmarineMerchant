using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CargoGame
{
    public class GUI_MainMenuPage : GUI_MenuPage
    {
        public Button bContinue, bHost, bJoin, bOptions, bExit;

        public override void Start()
        {
            bContinue.onClick.AddListener(ContinuePress);
            bHost.onClick.AddListener(HostPress);
            bJoin.onClick.AddListener(JoinPress);
            bOptions.onClick.AddListener(OptionsPress);
            bExit.onClick.AddListener(ExitPress);

        }


        void ContinuePress()
        {

        }

         void HostPress()
        {
            ClosePage();
            MainMenuManager.Instance.hostPage.OpenPage();
        }

         void JoinPress()
        {
            ClosePage();
            MainMenuManager.Instance.findPage.OpenPage();
        }

         void OptionsPress()
        {
            ClosePage();
            MainMenuManager.Instance.optionsPage.OpenPage();
        }

         void ExitPress()
        {
            StopAllCoroutines();
            StartCoroutine(ExitRoutine());
        }

        IEnumerator ExitRoutine()
        {
            // Do any pre quit stuff here.

            yield return null;
            Application.Quit();
        }

    }
}
