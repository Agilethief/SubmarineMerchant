using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CargoGame
{
    public class GUI_NewWorldPage : GUI_MenuPage
    {
        
        public TMP_InputField worldNameInput;

        public Button createWorldButton;


        public override void Start()
        {
            base.Start();

            createWorldButton.onClick.AddListener(CreateWorld);
        }


        void CreateWorld()
        {
            if(worldNameInput.text.Length < 1)
                worldNameInput.text = "DefaultWorldName";

            GameData.newWorldName = worldNameInput.text;
            
            ClosePage();
            MainMenuManager.Instance.CreateNewGame(GameData.newWorldName);

        }
    }
}
