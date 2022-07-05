// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;
using UnityEngine.UI;

namespace Cleverous.VaultInventory
{
    public class ItemUiFloater : MonoBehaviour
    {
        public Text StackSizeText;
        public Image MyImage;

        public virtual void Set(Sprite sprite, string text)
        {
            MyImage.sprite = sprite;
            StackSizeText.text = text;
        }
    }
}