// (c) Copyright Cleverous 2022. All rights reserved.

using Cleverous.VaultSystem;

namespace Cleverous.VaultInventory
{
    public class ContextMenuConfig : DataEntity
    {
        [AssetDropdown(typeof(Interaction))]
        public Interaction UseInteraction;
        [AssetDropdown(typeof(Interaction))]
        public Interaction SplitInteraction;
        [AssetDropdown(typeof(Interaction))]
        public Interaction DropInteraction;
    }
}