using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public static class GameData
    {
        public static string SelectedWorldName;

        public static string newWorldName;
        public static int newWorldSeed;
        public static bool hostingGame;

        public static string ipAddressToConnect;



        public enum GameStartType { Host, Dedicated, Client}
        public static GameStartType startType;

    }
}
