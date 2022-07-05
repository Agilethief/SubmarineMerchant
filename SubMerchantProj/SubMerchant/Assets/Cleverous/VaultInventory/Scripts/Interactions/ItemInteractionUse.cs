// (c) Copyright Cleverous 2020. All rights reserved.

using System;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Object;
using NetworkIdentity = FishNet.Object.NetworkObject;
#endif

namespace Cleverous.VaultInventory
{
    public class ItemInteractionUse : Interaction
    {
        protected override void Reset()
        {
            base.Reset();
            Title = "Interact Use";
            Description = "Use the item immediately.";
            InteractLabel = "USE";
        }

        public override bool IsValid(IInteractableTransform target)
        {
            ItemUiPlug plug = target.MyTransform.GetComponent<ItemUiPlug>();
            if (plug == null) return false;
            return plug.GetReferenceVaultItemData() is UseableItem;
        }

        public override void DoInteract(IInteractableTransform target)
        {
            ItemUiPlug plug = target.MyTransform.GetComponent<ItemUiPlug>();
            if (plug == null) return;

            // Only works for UseableItem classes.
            ((UseableItem)plug.GetReferenceVaultItemData()).UseBegin(plug.Ui.TargetInventory.InventoryOwner);
        }
    }
}