// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory
{
    public abstract class UseableItem : RootItem, IUseableDataEntity
    {
        [SerializeField]
        private float m_useCooldown;
        public float UseCooldownTime { get => m_useCooldown; set => m_useCooldown = value; }

        public abstract void UseBegin(IUseInventory user);
        public abstract void UseFinish(IUseInventory user);
        public abstract void UseCancel(IUseInventory user);
    }
}