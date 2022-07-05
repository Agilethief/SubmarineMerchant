// (c) Copyright Cleverous 2020. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Cleverous.VaultInventory
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HotbarUiPlug : ItemUiPlug
    {
        [Header("Hotbar Plug References")]
        public HotbarUiPanel HotbarPanel;
        public Image CooldownFillImage;
        public Action<int> OnChanged;
        private int m_dockId;

        // TODO You can dock/undock items with different cooldown times during cooldowns. Niche, but should be fixed.
        public IUseableDataEntity TargetData;
        public bool IsOnCooldown;
        
        private float m_cooldownNormalized;
        private float m_cooldownTimeRemaining;

        public override RootItem GetReferenceVaultItemData()
        {
            // This is always null in the hotbar. docked content could be any DataEntity as long as it implements IUseableDataEntity.
            // hotbar is not restricted to RootItem alone. Abilities, etc, could be in it.
            return null;
        }

        protected override void Awake()
        {
            base.Awake();
            UpdateUi(null, null);
        }
        protected virtual void Update()
        {
            if (m_cooldownTimeRemaining < 0 || TargetData == null)
            {
                IsOnCooldown = false;
                if (CooldownFillImage != null) CooldownFillImage.fillAmount = 0;
                return;
            }

            IsOnCooldown = true;
            m_cooldownTimeRemaining -= Time.deltaTime;
            m_cooldownNormalized = m_cooldownTimeRemaining / TargetData.UseCooldownTime;
            if (CooldownFillImage != null) CooldownFillImage.fillAmount = m_cooldownNormalized;
        }

        /// <summary>
        /// A Unity call. This is called when a Drag starts. It calls only on the component it is hovering.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (HotbarPanel.IsLocked) return;

            // can't drag the nothingness.
            if (TargetData == null) return;

            // can't drag if already dragging.
            if (InventoryUi.DragFloater != null) Destroy(InventoryUi.DragFloater);

            // can't have a tooltip if you're dragging.
            UiTooltip.Instance.Hide();

            InventoryUi.DragFloater = Instantiate(VaultInventory.ItemFloaterTemplate, transform);
            InventoryUi.DragFloater.transform.SetParent(VaultInventory.GameCanvas.transform);
            ItemUiFloater x = InventoryUi.DragFloater.GetComponent<ItemUiFloater>();
            x.Set(GetItemSprite(), GetStackSizeText());
            InventoryUi.DragOrigin = this;

            VaultInventory.OnMoveItemBegin?.Invoke(this);
        }

        /// <summary>
        /// A Unity interface call. This is called on the same component OnBeginDrag() was called on. It is called every frame until the drag finishes.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            if (InventoryUi.DragFloater == null) return;
            InventoryUi.DragFloater.transform.position = eventData.position;
        }

        /// <summary>
        /// A Unity interface call. This is NOT called on the same component as the OnDrag() entity. It will be called on whatever component the drag ENDS hovering over.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrop(PointerEventData eventData)
        {
            if (InventoryUi.DragFloater == null) return;
            InventoryUi.DragDestination = this;

            VaultInventory.OnMoveItemEnd?.Invoke(this);
        }

        /// <summary>
        /// A Unity interface call. This is called on the same component OnBeginDrag() was called on. OnDrop() will always be called before this, and always at the end of a Drag.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (InventoryUi.DragFloater == null) Clear();
            InventoryUi.HandleDragEvent();
        }

        /// <summary>
        /// A Unity interface call. Called when the entity is clicked.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (HotbarPanel.IsLocked) return;
                UpdateUiForHotbarPlug(null);
            }
            else Interact();
        }

        public override void UpdateUi(RootItemStack content, SlotRestriction restriction)
        {
            if (HotbarPanel.IsLocked) return;

            if (content == null || content.Source == null)
            {
                UpdateUiForHotbarPlug(null);
                return;
            }

            if (content.Source.GetType().IsAssignableFrom(typeof(IUseableDataEntity)))
            {
                UpdateUiForHotbarPlug((IUseableDataEntity)content.Source);
            }
        }

        public virtual void UpdateUiForHotbarPlug(IUseableDataEntity data)
        {
            if (HotbarPanel.IsLocked) return;

            TargetData = data;

            if (TargetData == null)
            {
                base.UpdateUi(null, null);
            }
            else
            {
                int stacksize = HotbarPanel.Owner.Inventory.GetCountOfItem(data.GetDbKey());
                SetTypeSprite(null);
                SetItemSprite(data.UiIcon);
                SetStackSizeText(stacksize > 0 ? stacksize.ToString() : "");
            }

            OnChanged?.Invoke(m_dockId);
        }

        public virtual void Clear()
        {
            SetTypeSprite(null);
            SetItemSprite(null);
            SetStackSizeText(string.Empty);
            StackSizeBox.SetActive(false);
            TargetData = null;
        }

        public override void Interact()
        {
            if (IsOnCooldown || TargetData == null) return;

            RootItem item = (RootItem) TargetData;
            if (item != null && !HotbarPanel.Owner.Inventory.Contains(item, 1)) return;

            TargetData.UseBegin(HotbarPanel.Owner);
            m_cooldownTimeRemaining = TargetData.UseCooldownTime;
        }

        public virtual void SetDockId(int id)
        {
            m_dockId = id;
        }
    }
}