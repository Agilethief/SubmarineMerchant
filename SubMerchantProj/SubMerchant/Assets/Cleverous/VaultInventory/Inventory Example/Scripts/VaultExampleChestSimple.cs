// (c) Copyright Cleverous 2020. All rights reserved.

using Cleverous.VaultSystem;
using Cleverous.NetworkImposter;
using UnityEngine;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Object;
#endif
 
namespace Cleverous.VaultInventory.Example
{
    public class VaultExampleChestSimple : NetworkBehaviour
    {
        [Header("References")]
        public GameObject Lid;
        [AssetDropdown(typeof(LootTable))]
        public LootTable Loot;
        public AudioSource AudioSource;
        public AudioClip SoundOpen;
        public AudioClip SoundClose;

        [Header("Configuration")]
        public int ItemCountMin;
        public int ItemCountMax;
        public int CanOpenMaxTimes;

        private int m_openedCount;

#if MIRROR && !FISHNET || !MIRROR && !FISHNET
        public virtual void Reset()
        {
#elif FISHNET
        protected override void Reset()
        {
            base.Reset();
#endif
            Lid = null;
            Loot = null;
            AudioSource = null;
            SoundOpen = null;
            SoundClose = null;

            ItemCountMin = 1;
            ItemCountMax = 4;
            CanOpenMaxTimes = 1;
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            if (m_openedCount < CanOpenMaxTimes) Interact();
        }

        public virtual void OnTriggerExit(Collider col)
        {
            if (m_openedCount < CanOpenMaxTimes) InteractEnd();
        }

        public virtual void Interact()
        {
            AudioSource.clip = SoundOpen;
            AudioSource.Play();
            Lid.transform.localRotation = Quaternion.Euler(90, 0, 0);

            if (this.IsServer())
            {
                m_openedCount++;
                SvrSpawnItems();
            }
        }
        public virtual void InteractEnd()
        {
            AudioSource.clip = SoundClose;
            AudioSource.Play();
            Lid.transform.localRotation = Quaternion.Euler(0, 0, 0);
            UiContextMenu.Instance.HideContextMenu();
        }

        [Server]
        protected virtual void SvrSpawnItems()
        {
            int count = Random.Range(ItemCountMin, ItemCountMax);
            for (int i = 0; i < count; i++)
            {
                RootItemStack item = Loot.GetLoot();
                if (item.Source != null) VaultInventory.SpawnWorldItem(item.Source, transform.position, item.StackSize);
            }
        }
    }
}