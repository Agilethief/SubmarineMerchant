// (c) Copyright Cleverous 2020. All rights reserved.

using Cleverous.NetworkImposter;
using UnityEngine;

namespace Cleverous.VaultInventory.Example
{
    public class WaffleConsumable : UseableItem
    {
        [Header("[Consumable]")]
        public AudioClip ConsumeSound;

        public override void UseBegin(IUseInventory user)
        {
            AudioSource.PlayClipAtPoint(ConsumeSound, user.MyTransform.position, 0.7f);

            if (NetworkPipeline.StaticIsServer())
            {
                // user.health++ or whatever
                // take item
                user.Inventory.DoTake(this, 1);
            }
        }

        public override void UseFinish(IUseInventory user)
        {
            // TODO for more advanced things, like abilities tap once to aim (begin) and again to use (finish).
            // We don't implement this by default because it's out of scope, but we want to provide the methods to support doing it.
            throw new System.NotImplementedException();
        }

        public override void UseCancel(IUseInventory user)
        {
            // TODO for when using the finish method.
            throw new System.NotImplementedException();
        }
    }
}