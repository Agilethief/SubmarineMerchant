using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace CargoGame
{
    public class GUI_OpenWorldPage : GUI_MenuPage
    {
        
        public Transform scrollContainer;

        public GUI_WorldEntry worldEntryTemplate;
        public Toggle onlyServerToggle;

        List<GUI_WorldEntry> worldEntries = new List<GUI_WorldEntry>();

        void ClearWorlds()
        {
            if(worldEntries.Count <= 0) return;

            for(int i = 0; i < worldEntries.Count; i++)
            {
                Destroy(worldEntries[i]);
            }

            worldEntries.Clear();
        }

        public void GetAllSavedWorlds()
        {
            ClearWorlds();

            string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.savefile");
            string worldVisibleName;

            foreach(string saveFile in filePaths)
            {
                worldVisibleName = saveFile.Replace(Application.persistentDataPath + @"\", ""); // Remove the path
                worldVisibleName = worldVisibleName.Replace(".savefile", ""); // remove the extension

                GUI_WorldEntry newEntry = Instantiate(worldEntryTemplate, scrollContainer);
                newEntry.name = "World: " + worldVisibleName;
                newEntry.worldName = worldVisibleName;
                newEntry.worldNameVisible = "World: " + worldVisibleName;
                newEntry.gameObject.SetActive(true);
                worldEntries.Add(newEntry);
            }

        }


    }
}
