// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;
using UnityEngine.UI;

namespace Cleverous.VaultInventory
{
    /// <summary>
    /// Used to communicate with the Merchant class.
    /// </summary>
    public class MerchantListItem : MonoBehaviour
    {
        public Image ItemIcon;
        public Image CurrencyIcon;

        public Text ItemName;
        public Text ItemPrice;
        public Button BuyButton;

        protected RootItem ReferenceItem;
        protected MerchantUi Controller;

        public virtual void Setup(Merchant merchant, MerchantUi controller, int index)
        {
            Controller = controller;
            ReferenceItem = merchant.ItemsForSale[index];

            if (ReferenceItem == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (ItemIcon != null) ItemIcon.sprite = ReferenceItem.UiIcon;
            if (ItemName != null) ItemName.text = ReferenceItem.Title;
            const char cent = '\u00A2';
            if (ItemPrice != null) ItemPrice.text = $"{cent}{ReferenceItem.Value}";
            if (CurrencyIcon != null) CurrencyIcon.sprite = merchant.AcceptedCurrency.UiIcon;
            if (BuyButton != null) BuyButton.onClick.AddListener(() => Buy(index, 1));
        }

        public virtual void Buy(int index, int count)
        {
            Controller.ClientBuy(index, count);
        }
    }
}