// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Cleverous.VaultInventory
{
    public class MerchantUiSellSlot : ItemUiPlug
    {
        public override void OnDrop(PointerEventData eventData)
        {
            base.OnDrop(eventData);

            // If we're not dropping into the hotbar, try to sell the item.
            if (!(InventoryUi.DragOrigin is HotbarUiPlug))
            {
                MerchantUi.Instance.ClientSell(InventoryUi.DragOrigin.ReferenceInventoryIndex);
            }

            if (InventoryUi.DragFloater != null) Destroy(InventoryUi.DragFloater);
            InventoryUi.DragOrigin = null;
            InventoryUi.DragDestination = null;
        }
    }
}