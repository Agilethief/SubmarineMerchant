// (c) Copyright Cleverous 2022. All rights reserved.

using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Cleverous.VaultDashboard
{
    public abstract class VaultDataGroupColumn : VaultDashboardColumn
    {
        protected bool IsSubscribed;
        protected static List<IVaultDataGroupButton> AllButtonsCache;
        public abstract VaultDataGroupFoldableButton SelectButtonByTitle(string title);
        public abstract void ScrollTo(VisualElement button);
        public abstract void Filter(string f);
    }
}