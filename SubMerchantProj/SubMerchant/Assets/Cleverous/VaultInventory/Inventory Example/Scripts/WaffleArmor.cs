// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Example
{
    public class WaffleArmor : WaffleBaseEquipment
    {
        [Header("[Armor]")]
        public int DefenseValue;

        protected override void Reset()
        {
            base.Reset();
            DefenseValue = 5;
        }
    }
}