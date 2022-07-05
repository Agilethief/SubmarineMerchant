// (c) Copyright Cleverous 2020. All rights reserved.

namespace Cleverous.VaultInventory
{
    public class ItemInteractionDrop : Interaction
    {
        protected override void Reset()
        {
            base.Reset();
            Title = "Interact Drop";
            Description = "Drop the item immediately.";
            InteractLabel = "Drop";
        }

        public override bool IsValid(IInteractableTransform target)
        {
            ItemUiPlug plug = target.MyTransform.GetComponent<ItemUiPlug>();
            if (plug == null) return false;

            // TODO check here to prevent this from being valid if it's not the local player's inventory.
            return true;
        }

        public override void DoInteract(IInteractableTransform target)
        {
            ItemUiPlug plug = target.MyTransform.GetComponent<ItemUiPlug>();
            if (plug == null) return;

            plug.Ui.TargetInventory.CmdRequestDrop(plug.ReferenceInventoryIndex);
        }
    }
}