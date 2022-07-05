// (c) Copyright Cleverous 2022. All rights reserved.

using UnityEngine.UIElements;

namespace Cleverous.VaultDashboard
{
    public abstract class VaultDashboardColumn : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<VisualElement, UxmlTraits> { }
        public abstract void VaultPanelReload();
    }
}