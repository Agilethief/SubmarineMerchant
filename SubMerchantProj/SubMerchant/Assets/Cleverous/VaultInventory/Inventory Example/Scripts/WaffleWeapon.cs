// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Example
{
    public class WaffleWeapon : WaffleBaseEquipment
    {
        [Header("[Weapon]")]
        public int AttackValue;

        protected override void Reset()
        {
            base.Reset();
            AttackValue = 5;
        }
    }
}