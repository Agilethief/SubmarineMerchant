// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Example
{
    public class WaffleResource : RootItem
    {
        [Header("[Resource]")]
        public int SomeCraftingValue;

        protected override void Reset()
        {
            base.Reset();
            MaxStackSize = 500;
            SomeCraftingValue = 123;
        }
    }
}