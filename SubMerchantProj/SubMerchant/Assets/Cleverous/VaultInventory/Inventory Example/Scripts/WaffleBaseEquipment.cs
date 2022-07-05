// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Example
{
    public abstract class WaffleBaseEquipment : RootItem
    {
        [Header("[Equipment]")]
        public int Weight;

        protected override void Reset()
        {
            base.Reset();
            Weight = 42;
        }
    }
}