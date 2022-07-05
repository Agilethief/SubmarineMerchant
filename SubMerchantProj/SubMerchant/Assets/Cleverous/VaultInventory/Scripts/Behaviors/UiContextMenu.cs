// (c) Copyright Cleverous 2020. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Cleverous.VaultSystem;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;

namespace Cleverous.VaultInventory
{
    /// <summary>
    /// The UI Context Menu is a popup for contextual actions on some entity. It requires a GridLayoutGroup loaded with enough UI Buttons to accomodate all Interactions.
    /// TODO - Controller support
    /// </summary>
    public class UiContextMenu : MonoBehaviour
    {
        public static UiContextMenu Instance;

        [AssetDropdown(typeof(ContextMenuConfig))]
        public ContextMenuConfig Configuration;
        public GameObject PanelWrapper;
        public GridLayoutGroup Grid;
        protected Button[] UiButtons;
        protected Text[] UiButtonLabels;
        protected Interaction[] InteractCache;
        protected IInteractableTransform CurrentTargetCache;
        private RectTransform m_rect;
        private List<Interaction> m_defaultInteractions;
        public bool MenuIsOpen { get; protected set; }

        public static Action OnOpened;
        public static Action OnClosed;


        public virtual void Awake()
        {
            Instance = this;
            PanelWrapper.gameObject.SetActive(false);
            UiButtons = Grid.GetComponentsInChildren<Button>();
            UiButtonLabels = Grid.GetComponentsInChildren<Text>();
            m_rect = GetComponent<RectTransform>();
            m_defaultInteractions = new List<Interaction>
            {
                Configuration.UseInteraction,
                Configuration.SplitInteraction,
                Configuration.DropInteraction
            };
        }

        protected virtual void Update()
        {

            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
#if ENABLE_INPUT_SYSTEM
                position = Mouse.current.position.ReadValue()
#else
                position = Input.mousePosition
#endif
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            bool isHoveringContextMenu = false;
            foreach (RaycastResult x in results)
            {
                if (UiButtons.Any(o => o.gameObject == x.gameObject))
                {
                    isHoveringContextMenu = true;
                }
            }
            if (!isHoveringContextMenu) 
            {
                HideContextMenu();
            }
        }

        /// <summary>
        /// Show the context menu on screen. If there is only one possible interaction, it is fired.
        /// </summary>
        /// <param name="position">Where to put the menu.</param>
        /// <param name="target">The thing to interact with - what the dropdown content is based on.</param>
        public virtual void ShowContextMenu(Vector3 position, IInteractableTransform target)
        {
            if (target.MyTransform == null) return;

            m_rect.SetAsLastSibling();
            MenuIsOpen = true;
            
            InteractCache = new Interaction[m_defaultInteractions.Count + target.Interactions.Length];
            m_defaultInteractions.CopyTo(InteractCache);
            target.Interactions.CopyTo(InteractCache, m_defaultInteractions.Count);

            PanelWrapper.transform.position = position;
            CurrentTargetCache = target;

            if (InteractCache.Length == 0) return;

            // If there is only one thing to do, then do it.
            if (InteractCache.Length == 1 && InteractCache[0] != null && InteractCache[0].IsValid(target))
            {
                InteractCache[0].DoInteract(target);
                return;
            }

            // If there are multiple things to do, populate the list of buttons.
            for (int i = 0; i < UiButtons.Length; i++)
            {
                if (i < InteractCache.Length)
                {
                    if (InteractCache[i] == null) continue;
                    if (!InteractCache[i].IsValid(target))
                    {
                        UiButtons[i].gameObject.SetActive(false);
                        continue;
                    }

                    UiButtons[i].gameObject.SetActive(true);
                    UiButtonLabels[i].text = InteractCache[i].InteractLabel;
                }
                else
                {
                    UiButtons[i].gameObject.SetActive(false);
                }
            }

            PanelWrapper.gameObject.SetActive(true);
            OnOpened?.Invoke();
        }

        /// <summary>
        /// Used implicitly by the <see cref="Button"/>s under the Grid on the Context Menu.
        /// </summary>
        /// <param name="id">The index of the button.</param>
        public virtual void ClickedInteractionUiButton(int id)
        {
            HideContextMenu();
            InteractCache[id].DoInteract(CurrentTargetCache);
        }

        /// <summary>
        /// Hide the context menu.
        /// </summary>
        public virtual void HideContextMenu()
        {
            MenuIsOpen = false;
            PanelWrapper.gameObject.SetActive(false);
            OnClosed?.Invoke();
        }
    }
}