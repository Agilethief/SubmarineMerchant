using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

namespace CargoGame
{
    public class SteamDebugScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if(!SteamManager.Initialized) return;
            
            string steamName = SteamFriends.GetPersonaName();
            Debug.Log("Steam name is: " + steamName);


        }

    }
}
