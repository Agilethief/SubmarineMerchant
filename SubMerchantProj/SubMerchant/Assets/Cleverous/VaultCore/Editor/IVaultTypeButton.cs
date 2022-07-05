// (c) Copyright Cleverous 2022. All rights reserved.

using Cleverous.VaultSystem;
using UnityEngine.UIElements;

namespace Cleverous.VaultDashboard
{
    public interface IVaultDataGroupButton
    {
        string Title { get; set; }
        IDataGroup DataGroup { get; set; }
        VisualElement MainElement { get; set; }
        VisualElement InternalElement { get; set; }

        void SetAsCurrent();
        void SetIsSelected(bool state);
        void SetIsHighlighted(bool state);
        void SetShowFoldout(bool show);
    }
}