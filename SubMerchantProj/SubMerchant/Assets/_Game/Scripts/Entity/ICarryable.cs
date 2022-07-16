using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public interface ICarryable 
    {
        public void Pickup(Transform pickingUpObject);

        public void CMDDrop(float throwStr);
    }
}
