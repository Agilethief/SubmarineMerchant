// (c) Copyright Cleverous 2020. All rights reserved.

using Cleverous.VaultSystem;

namespace Cleverous.VaultInventory
{
    public class RootItemStack
    {
        public RootItemStack(RootItem item, int count)
        {
            Source = item;
            StackSize = count;
        }
        public RootItemStack(int vaultId, int count)
        {
            Source = (RootItem) Vault.Get(vaultId);
            StackSize = count;
        }

        public RootItem Source;
        public int StackSize;

        public virtual void Reset()
        {
            Source = null;
            StackSize = 0;
        }

        /// <summary>
        /// Get the total market value of this stack
        /// </summary>
        /// <returns>Market Value of the stack.</returns>
        public virtual int GetTotalValue()
        {
            return Source.Value * StackSize;
        }

        public virtual void Split()
        {

        }
    }
}