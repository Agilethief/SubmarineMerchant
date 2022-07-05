// (c) Copyright Cleverous 2020. All rights reserved.

using Cleverous.VaultSystem;

namespace Cleverous.VaultInventory
{
    public class InventoryConfig : DataEntity
    {
        [AssetDropdown(typeof(SlotRestriction))]
        public SlotRestriction[] SlotRestrictions;

        public InventoryConfig()
        {
            SlotRestrictions = new SlotRestriction[10];
        }

        protected override void Reset()
        {
            base.Reset();
            SlotRestrictions = new SlotRestriction[10];
        }
    }
}