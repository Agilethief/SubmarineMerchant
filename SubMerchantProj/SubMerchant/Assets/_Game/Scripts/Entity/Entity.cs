using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class Entity : BaseBehaviour
    {
        public string entityName;
        public bool doNotSave; // This will stop the world trying to save this as a world item
        public int itemID; // This should correspond with the id in the spawn list. This will be referenced in the world item struct for spawning and despawning 


    }
}