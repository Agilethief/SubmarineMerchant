using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class EntityPlayer : Entity, IDamagable
    {
        [Header("Player Attributes")]
        [SerializeField] protected float baseMoveSpeed;
        [SerializeField] protected float dizzyMoveSpeed;

        [SerializeField, SyncVar (hook = nameof(RefreshUIHealth))]
        private int healthCurrent, healthMax;

        [SerializeField, SyncVar (hook = nameof(RefreshUIWater))]
        private int waterCurrent, waterMax;
        [SerializeField]
        private float waterDizzyThreshold = 0.2f;
        bool dizzy;
        float normalisedWater;
        [SerializeField, SyncVar]
        private int waterBaseDrain, waterSunDrain;

        [SerializeField, SyncVar (hook = nameof(RefreshUIFood))]
        private int foodCurrent, foodMax;


        [SerializeField, SyncVar]
        private bool inSun; // When this is true, the player is in the sunlight and thus extra water damage occurs
        [SerializeField]
        LayerMask sunBlockingLayers;

        SimplePlayer _myPlayer;
         SimplePlayer myPlayer { get{
                if(_myPlayer == null) _myPlayer = transform.GetComponent<SimplePlayer>();
                return _myPlayer;} 
            }

        public void Die()
        {
            myPlayer.camRig.SetDeadVolume(true);
            // Disable input

            // Lower Cam to ground

            // Set state machine
            
        }

        public void HealDamage(int healAmount)
        {
            healthCurrent += healAmount;
            if(healthCurrent > healthMax) healthCurrent = healthMax;
        }

        public void TakeDamage(int damageAmount, IDamagable.DamageType damageType)
        {
            healthCurrent -= damageAmount;
            if(healthCurrent < 0)
                Die();
        }

        public void DrinkWater(int waterAmount = 200)
        {
            //Debug.Log("Water being drunk by player object");
            waterCurrent += waterAmount;
        }

        // This is periodically called to reduce the amount of water the player has.
        public void WaterTick()
        {
            //Temp
            inSun = CheckIfInSun(Vector3.up * 9000);

            waterCurrent -= waterBaseDrain;
            if(inSun)
            {
                waterCurrent -= waterSunDrain;
            }
            if(waterCurrent < 0) waterCurrent = 0;

            normalisedWater = Mathf.InverseLerp(0, waterMax, waterCurrent);

            DizzyCheck();
        }

        public bool CheckIfInSun(Vector3 sunWorldPos)
        {
            if(Physics.Linecast(sunWorldPos, pos, sunBlockingLayers))
            {
                return false;
            }
            return true;
        }

        void DizzyCheck()
        {
            if (normalisedWater < waterDizzyThreshold)
            {
                if (dizzy) return;

                myPlayer.camRig.SetDizzyWater(true);
                dizzy = true;
                myPlayer.movementStateMachine.moveSpeed = dizzyMoveSpeed;
            }
            else
            {
                if (!dizzy) return;

                myPlayer.camRig.SetDizzyWater(false);
                dizzy = false;
                myPlayer.movementStateMachine.moveSpeed = baseMoveSpeed;
            }
        }

        void RefreshUIWater(int oldVal, int newVal)
        {
            myPlayer.playerUI.statsPanel.SetWater(waterCurrent, waterMax);
        }
         void RefreshUIFood(int oldVal, int newVal)
        {
            myPlayer.playerUI.statsPanel.SetFood(foodCurrent, foodMax);
        }
         void RefreshUIHealth(int oldVal, int newVal)
        {
            myPlayer.playerUI.statsPanel.SetHealth(healthCurrent, healthMax);
        }
    }


}
