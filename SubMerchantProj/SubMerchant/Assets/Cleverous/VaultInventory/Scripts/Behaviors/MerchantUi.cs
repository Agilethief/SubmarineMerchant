// (c) Copyright Cleverous 2020. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cleverous.VaultInventory
{
    /// <summary>
    /// Will manage actions between shop ui components and the merchant.
    /// </summary>
    public class MerchantUi : MonoBehaviour
    {
        public static MerchantUi Instance;
        public static bool IsOpen { get; protected set; }
        public static Action OnOpened;
        public static Action OnClosed;

        [Header("References")]
        public RectTransform ContentContainer;
        public Text NameText;
        public GameObject ListItemPrefab;

        private Merchant m_merchant;
        private List<GameObject> m_uiItems;

        public void Awake()
        {
            m_uiItems = new List<GameObject>();
            Instance = this;
            gameObject.SetActive(false);
        }

        public virtual void Open(Merchant merchant)
        {
            IsOpen = true;
            m_merchant = merchant;
            if (NameText != null) NameText.text = merchant.MerchantName;
            gameObject.SetActive(true);

            for (int i = 0; i < merchant.ItemsForSale.Length; i++)
            {
                GameObject go = Instantiate(ListItemPrefab, ContentContainer);
                m_uiItems.Add(go);

                MerchantListItem row = go.GetComponent<MerchantListItem>();
                row.Setup(m_merchant, this, i);
            }

            OnOpened?.Invoke();
        }
        public virtual void Close()
        {
            IsOpen = false;
            m_merchant = null;
            foreach (GameObject x in m_uiItems)
            {
                Destroy(x);
            }
            gameObject.SetActive(false);

            OnClosed?.Invoke();
        }

        public virtual void ClientBuy(int index, int count)
        {
            if (m_merchant == null) return;
            m_merchant.ClientBuy(index, count);
        }
        public virtual void ClientSell(int index)
        {
            m_merchant.ClientSell(index);
        }

        public virtual void OnDisable()
        {
            IsOpen = false;
        }
    }
}