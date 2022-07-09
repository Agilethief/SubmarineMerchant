using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CargoGame
{
    public class GUI_HostGamePage : GUI_MenuPage
    {
       
        public Button newWorldButton, openWorldButton;

        public override void Start()
        {
            base.Start();

            newWorldButton.onClick.AddListener(GoToNewWorldPage);
            openWorldButton.onClick.AddListener(GoToOpenWorldPage);
        }

        public void GoToNewWorldPage()
        {
            ClosePage();
            MainMenuManager.Instance.newWorldPage.OpenPage();
        }

         public void GoToOpenWorldPage()
        {
            MainMenuManager.Instance.openWorldPage.GetAllSavedWorlds();

            ClosePage();
            MainMenuManager.Instance.openWorldPage.OpenPage();
        }

    }
}
