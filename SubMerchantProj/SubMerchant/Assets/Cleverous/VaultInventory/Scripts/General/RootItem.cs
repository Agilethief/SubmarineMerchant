// (c) Copyright Cleverous 2020. All rights reserved.

using Cleverous.VaultSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cleverous.VaultInventory
{
    public abstract class RootItem : DataEntity
    {
        [Header("[Item Properties]")]
        public ItemRarity Rarity;
        [AssetDropdown(typeof(SlotRestriction))]
        public SlotRestriction Restriction;
        
        [SpritePreview]
        [SerializeField]
        [FormerlySerializedAs("UiIcon")]
        private Sprite m_uiIcon;
        public Sprite UiIcon { get => m_uiIcon; set => m_uiIcon = value; }

        public GameObject ArtPrefab;
        public int MaxStackSize;
        public int Value;

        [AssetDropdown(typeof(Interaction))] 
        public Interaction[] ExtraInteractions;


        protected override void Reset()
        {
            base.Reset();
            Description = "Upon further examination, you find the cryptic words 'Lorum ipsum'.";

            Rarity = ItemRarity.Common;
            Restriction = null;
            m_uiIcon = null;
            ArtPrefab = null;
            MaxStackSize = 99;
            Value = 100;
        }

        public virtual Color GetRarityColor()
        {
            return ItemRarityColors.RarityColors[(int) Rarity];
        }
        public virtual string GetUiTitle()
        {
            string x = ColorUtility.ToHtmlStringRGB(GetRarityColor());
            return $"<color=#{x}>{Title}</color>";
        }
        public virtual string GetDescriptionSimple()
        {
            return Description;
        }
        public virtual string GetDescriptionComplex()
        {
            string restriction = Restriction == null ? "<color=white>Generic Item</color>" : $"<color=white>{Restriction.Title}</color>";
            return $"{GetDescriptionSimple()}\n" +
                   $"{restriction}\n" +
                   $"<color=white> ${Value}</color>";
        }
        protected override Sprite GetDataIconInternal()
        {
            return UiIcon;
        }
    }
}