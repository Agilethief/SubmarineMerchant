// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;
using Random = System.Random;
using Cleverous.VaultSystem;
using Cleverous.NetworkImposter;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Object;
#endif

namespace Cleverous.VaultInventory.Example
{
    public class VaultExampleTimedItemSpawner : NetworkBehaviour
    {
        [Header("Content for spawning")]
        [AssetDropdown(typeof(WaffleResource))]
        public WaffleResource[] Resources;

        [AssetDropdown(typeof(WaffleConsumable))]
        public WaffleConsumable[] Consumables;

        [AssetDropdown(typeof(WaffleBaseEquipment))]
        public WaffleBaseEquipment[] Equipment;

        [Header("Spawning parameters")]
        public float Frequency = 5;
        public float SpawnRadius = 6;

        // private
        private Random m_rng;
        private float m_timer;

        public override void OnStartServer()
        {
            // This should only be happening on the server so we use OnStartServer().
            // Clients have no control over spawning objects, ever.
            // All clients do is request actions on their inventory and maybe get updates back.

#if FISHNET
            base.OnStartServer();
#endif

            m_rng = new Random();
            m_timer = Frequency;
        }

        private void ChangeFreq(float value)
        {
            Frequency = value;
        }

        public void Update()
        {
            if (!this.IsServer()) return;

            // Every X seconds we will spawn either an Equipment or a batch of Resources/Consumables somewhere in the spawn radius.
            // This code figures out all those details.

            if (m_timer < Frequency)
            {
                m_timer += Time.deltaTime;
                return;
            }

            m_timer = 0;

            int itemType = m_rng.Next(3);
            Vector2 horizontalRng = UnityEngine.Random.insideUnitCircle;
            Vector3 spawnPosition = new Vector3(
                transform.position.x + horizontalRng.x * SpawnRadius,
                transform.position.y,
                transform.position.z + horizontalRng.y * SpawnRadius);

            RootItem sourceItem = null;
            int stackSize = 0;

            switch (itemType)
            {
                case 0:
                    sourceItem = Equipment[m_rng.Next(Equipment.Length)];
                    stackSize = 1;
                    break;
                case 1:
                    sourceItem = Resources[m_rng.Next(Resources.Length)];
                    stackSize = m_rng.Next(1, 30);
                    break;
                case 2:
                    sourceItem = Consumables[m_rng.Next(Consumables.Length)];
                    stackSize = m_rng.Next(1, 6);
                    break;
                default:
                    Debug.Log("Borked");
                    break;
            }

            // But if you know what you want to spawn and how many, this is all you need to do.
            // This method will spawn an item in the world, but it needs a RootItem source, World Position and Stack Size.
            VaultInventory.SpawnWorldItem(sourceItem, spawnPosition, stackSize);
        }
    }
}