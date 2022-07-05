// (c) Copyright Cleverous 2020. All rights reserved.

namespace Cleverous.VaultInventory
{
    public class ItemInteractionSplit : Interaction
    {
        protected override void Reset()
        {
            base.Reset();
            Title = "Interact Split";
            Description = "Split a stack of inventory items in half.";
            InteractLabel = "SPLIT";
        }

        public override bool IsValid(IInteractableTransform target)
        {
            ItemUiPlug plug = target.MyTransform.GetComponent<ItemUiPlug>();
            if (plug == null) return false;

            return plug.GetReferenceVaultItemData().MaxStackSize > 1 && plug.Ui.TargetInventory.Get(plug.ReferenceInventoryIndex).StackSize > 1;
        }

        public override void DoInteract(IInteractableTransform target)
        {
            ItemUiPlug plug = target.MyTransform.GetComponent<ItemUiPlug>();
            if (plug == null) return;

            plug.Ui.TargetInventory.CmdRequestSplit(plug.ReferenceInventoryIndex);
        }
    }
}