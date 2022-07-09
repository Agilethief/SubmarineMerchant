using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CargoGame
{
    public class MainMenuManager : BaseClientOnlyBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

        public bool menuActive = true; // Will be used to disable the menu during transitions, loads etc;

        public GUI_MainMenuPage mmPage;
        public GUI_OptionsPage optionsPage;
        public GUI_JoinGamePage joinPage;
        public GUI_FindGamesPage findPage;
        public GUI_HostGamePage hostPage;
        public GUI_NewWorldPage newWorldPage;
        public GUI_OpenWorldPage openWorldPage;

        public CanvasGroup curtain;

        //public WorldBuilderSequencer worldBuilder;

        IEnumerator sceneLoadingRoutine;

        private void Start()
        {
            Instance = this; 

            mmPage.gameObject.SetActive(true);
            mmPage.cg.alpha = 1;

            optionsPage.cg.alpha = 0;
            joinPage.cg.alpha = 0;
            hostPage.cg.alpha = 0;
            newWorldPage.cg.alpha = 0;
            openWorldPage.cg.alpha = 0;
            findPage.cg.alpha = 0;

            optionsPage.cg.blocksRaycasts = false;
            optionsPage.cg.interactable = false;
            joinPage.cg.blocksRaycasts = false;
            joinPage.cg.interactable = false;
            hostPage.cg.blocksRaycasts = false;
            hostPage.cg.interactable = false;
            newWorldPage.cg.blocksRaycasts = false;
            newWorldPage.cg.interactable = false;
            openWorldPage.cg.blocksRaycasts = false;
            openWorldPage.cg.interactable = false;
            findPage.cg.blocksRaycasts = false;
            findPage.cg.interactable = false;

            optionsPage.gameObject.SetActive(true);
            joinPage.gameObject.SetActive(true);
            hostPage.gameObject.SetActive(true);
            newWorldPage.gameObject.SetActive(true);
            openWorldPage.gameObject.SetActive(true);
            findPage.gameObject.SetActive(true);
        }

        public void CreateNewGame(string saveName)
        {
            SaveSystem.Save(saveName);

            WorldGenerationComplete();
        }

        public void WorldGenerationComplete()
        {
            openWorldPage.GetAllSavedWorlds();
            openWorldPage.OpenPage();
        }

        public void OpenScene(string sceneString)
        {
            if (sceneLoadingRoutine != null)
                StopCoroutine(sceneLoadingRoutine);

            sceneLoadingRoutine = LoadSceneRoutine(sceneString);
            StartCoroutine(sceneLoadingRoutine);

        }

        IEnumerator LoadSceneRoutine(string sceneString)
        {
            
            yield return new WaitForSeconds(1f);

            SceneManager.LoadScene(sceneString);

            yield return null;
        }

    }
}
