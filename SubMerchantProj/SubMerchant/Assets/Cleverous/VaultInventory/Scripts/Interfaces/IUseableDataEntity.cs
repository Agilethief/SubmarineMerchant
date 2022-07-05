// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory
{
    public interface IUseableDataEntity
    {
        int GetDbKey();
        string Title { get; set; }
        string Description { get; set; }
        Sprite UiIcon { get; set; }
        float UseCooldownTime { get; set; }
        void UseBegin(IUseInventory user);
        void UseFinish(IUseInventory user);
        void UseCancel(IUseInventory user);
    }
}