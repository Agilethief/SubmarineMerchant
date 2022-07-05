// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory
{
    /// <summary>
    /// <para>An Interactable UI Item. Could also be used for other interactables, like structures in the world, but mainly for UI plugs.</para>
    /// </summary>
    public interface IInteractableTransform
    {
        Transform MyTransform { get; }
        Interaction[] Interactions { get; }
        void Interact();
    }
}