// (c) Copyright Cleverous 2020. All rights reserved.

using Cleverous.VaultSystem;

namespace Cleverous.VaultInventory
{
    /// <summary>
    /// Populates extra dropdown interactions for the UI Context Menu (typically via Right Click)
    /// </summary>
    public abstract class Interaction : DataEntity
    {
        public string InteractLabel;

        /// <summary>
        /// Determine if an Interaction is valid for the context.
        /// </summary>
        /// <param name="interactable">The target Interactable (usually a UI Plug) </param>
        /// <returns>Whether or not the action is valid.</returns>
        public abstract bool IsValid(IInteractableTransform interactable);

        /// <summary>
        /// Perform the designed Interaction.
        /// </summary>  
        /// <param name="interactable">The target Interactable (usually a UI Plug) </param>
        public abstract void DoInteract(IInteractableTransform interactable);
    }
}