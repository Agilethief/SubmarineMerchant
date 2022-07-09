using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace CargoGame
{
    public static class SaveSystem
    {
        static string saveLocation = "/.savefile";
        //static string orgSaveLocation = "/.world";    // This is for when we aren't usign cloud saves etc

        static int saveVersionNumber = 1;


        static GameSaveFile _worldSaveFile;
        static public GameSaveFile worldSaveFile
        {
            get
            {
                //Debug.Log("Getting Save File");
                if (_worldSaveFile == null)
                {
                    //Debug.Log("No save file, trying to load");
                    Load("");
                }
                //Debug.Log("Got Save File");
                return _worldSaveFile;
            }
            set
            {
                //Debug.Log("Setting Save File");
                _worldSaveFile = value;
            }
        }


        static void GetSaveLocation(string saveName)
        {

            saveLocation = "/" + saveName + ".savefile";

            //saveLocation = orgSaveLocation;

        }



        public static void Save(string saveName)
        {
            ////Debug.Log("Attempting Save");

            // Convert the world data into something that is serializable
            worldSaveFile = new GameSaveFile();
          
            worldSaveFile.WorldName = saveName;

            // Actually save the stuff
            GetSaveLocation(worldSaveFile.WorldName);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + saveLocation);
            //Debug.Log(Application.persistentDataPath + saveLocation);
            bf.Serialize(file, worldSaveFile);
            file.Close();
        }


        public static void Load(string saveName)
        {

            Debug.Log("Loading Save");
            // Disable saving in demos
            //if (WitchThiefBehaviour.IsDemo == true)
            //	return;

            // For steam cloud saves
            GetSaveLocation(saveName);

            if (File.Exists(Application.persistentDataPath + saveLocation))
            {
                //Debug.Log("Save file found, load starting");
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + saveLocation, FileMode.Open);
                if (file.Length == 0) // A catch for if we open up a blank save file
                {
                    file.Close();
                    ResetSave();
                    return;
                }
                worldSaveFile = (GameSaveFile)bf.Deserialize(file);
                file.Close();
            }
            else
            {
                // There is no save file, so create one.
                Debug.Log("No save file to load, resetting to blank");
                ResetSave();
            }

            // Check if the save file version number is wrong and thus needs to be reset.
            if (worldSaveFile.version != saveVersionNumber)
            {
                Debug.Log("Save file incorrect version, resetting to blank");
                ////Debug.Log("Save File Wrong Version. Wiping Prefs and starting save a new");
                PlayerPrefs.DeleteAll(); // Also wipe all the prefs so we can start anew
                ResetSave();
            }

            // This is where we unpack the save and create a proper world data struct

            //Debug.Log("Save file found and converting to world data");
            

            //Debug.Log("Save file: chunkmap: " + worldData.chunkMap.GetLength(0));


            return;

        }

        public static void ResetSave()
        {
            worldSaveFile = new GameSaveFile();
            Save("default");

        }

    }

    // This is created and set by the save function
    [System.Serializable]
    public class GameSaveFile
    {
        // Version number, test this against the internal number to see if the save needs to be wiped or not.
        public int version = 0;

        // Save Flags
        public int FirstTimeSetup;    // We have now ran the game at least once.

        public string WorldName;
        
    }
}
