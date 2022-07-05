// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory
{
    public interface IUseInventory
    {
        Inventory Inventory { get; set; }
        Transform MyTransform { get; }
    }
}