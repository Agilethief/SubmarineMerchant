// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;

namespace Cleverous.VaultInventory
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ItemUiPlug : MonoBehaviour, IInteractableTransform, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Transform MyTransform => transform;
        public Interaction[] Interactions
        {
            get
            {
                if (GetReferenceVaultItemData() == null || GetReferenceVaultItemData().ExtraInteractions == null) return new Interaction[0];
                return GetReferenceVaultItemData().ExtraInteractions;
            }
        }

        [Header("Plug References")]
        public GameObject SlotOwnerObject;
        public Image MyTypeImage;
        public Image MyItemImage;
        public Image MyHighlight;
        public GameObject StackSizeBox;
        public Text StackSizeText;

        private Color m_oriBgColor;

        /// <summary>
        /// The UI hosting this Plug.
        /// </summary>
        public InventoryUi Ui { get; set; }
        /// <summary>
        /// A reference to the index of the item in the <see cref="Inventory"/> of the <see cref="InventoryUi"/> hosting this Plug - *not* a reference to the Vault Database index.
        /// </summary>
        public int ReferenceInventoryIndex { get; set; }

        /// <summary>
        /// A reference to the <see cref="RootItem"/> in the database.
        /// </summary>
        public virtual RootItem GetReferenceVaultItemData()
        {
            return Ui?.TargetInventory?.Get(ReferenceInventoryIndex)?.Source;
        }

        protected virtual void Awake()
        {
            m_oriBgColor = MyTypeImage == null ? Color.white : MyTypeImage.color;
            HighlightOff();
        }

        /// <summary>
        /// A Unity call. This is called when a Drag starts. It calls only on the component it is hovering.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            // can't drag the nothingness.
            if (Ui.TargetInventory.Get(ReferenceInventoryIndex) == null || Ui.TargetInventory.Get(ReferenceInventoryIndex).StackSize == 0) return;

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
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (InventoryUi.DragFloater == null) return;
            InventoryUi.DragFloater.transform.position = eventData.position;
        }

        /// <summary>
        /// A Unity interface call. This is NOT called on the same component as the OnDrag() entity. It will be called on whatever component the drag ENDS hovering over.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrop(PointerEventData eventData)
        {
            if (InventoryUi.DragFloater == null) return;
            InventoryUi.DragDestination = this;

            VaultInventory.OnMoveItemEnd?.Invoke(this);
        }

        /// <summary>
        /// A Unity interface call. This is called on the same component OnBeginDrag() was called on. OnDrop() will always be called before this, and always at the end of a Drag.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (InventoryUi.DragFloater == null) return;
            InventoryUi.HandleDragEvent();
        }

        /// <summary>
        /// A Unity interface call. Called when the entity is clicked.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right) Interact();
            else InventoryUi.ClickedItem = this;
        }

        /// <summary>
        /// A Unity interface call.
        /// </summary>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            HighlightOn();
            if (InventoryUi.DragFloater != null) return;
            UiTooltip.Instance.Show(this);
        }

        /// <summary>
        /// A Unity interface call.
        /// </summary>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            HighlightOff();
            UiTooltip.Instance.Hide();
        }

        public virtual void Interact()
        {
#if ENABLE_INPUT_SYSTEM
            UiContextMenu.Instance.ShowContextMenu(Mouse.current.position.ReadValue(), this);
#else
            UiContextMenu.Instance.ShowContextMenu(Input.mousePosition, this);
#endif
        }

        public virtual void UpdateUi(RootItemStack content, SlotRestriction restriction)
        {
            string stack = string.Empty;
            Sprite itemImg = null;

            if (content != null)
            {
                if (content.Source != null) itemImg = content.Source.UiIcon;
                if (StackSizeBox != null) StackSizeBox.SetActive(content.StackSize > 1);
                stack = content.StackSize > 1 ? content.StackSize.ToString() : string.Empty;
            }

            // push the results to the ui
            if (MyItemImage != null)
            {
                SetItemSprite(itemImg);
                MyItemImage.color = itemImg == null ? Color.clear : Color.white;
            }

            if (MyTypeImage != null)
            {
                SetTypeSprite(restriction == null ? null : restriction.UiIcon);
                MyTypeImage.color = restriction == null || itemImg != null ? Color.clear : m_oriBgColor;
            }

            SetStackSizeText(stack);
        }

        public virtual void SetItemSprite(Sprite sprite)
        {
            if (MyItemImage == null) return;

            MyItemImage.sprite = sprite;
            MyItemImage.color = sprite == null ? Color.clear : Color.white;
        }        
        public virtual Sprite GetItemSprite()
        {
            return MyItemImage == null ? null : MyItemImage.sprite;
        }

        public virtual void SetTypeSprite(Sprite sprite)
        {
            if (MyTypeImage != null) MyTypeImage.sprite = sprite;
        }
        public virtual Sprite GetTypeSprite()
        {
            return MyTypeImage == null ? null : MyTypeImage.sprite;
        }

        public virtual void SetStackSizeText(string text)
        {
            if (StackSizeText == null) return;

            if (text == string.Empty) StackSizeBox.SetActive(text != string.Empty);
            StackSizeText.text = text;
        }
        public virtual string GetStackSizeText()
        {
            return StackSizeText == null ? string.Empty : StackSizeText.text;
        }

        public virtual void HighlightOn()
        {
            MyHighlight.gameObject.SetActive(true);
        }
        public virtual void HighlightOff()
        {
            MyHighlight.gameObject.SetActive(false);
        }
    }
}