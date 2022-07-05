// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Cleverous.VaultInventory
{
    public class UiWindowDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public RectTransform TargetPanel;

        public void OnBeginDrag(PointerEventData eventData)
        {
            TargetPanel.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 moveDelta = eventData.delta / VaultInventory.GameCanvas.scaleFactor;
            float xMin = VaultInventory.GameCanvas.pixelRect.xMin / VaultInventory.GameCanvas.scaleFactor;
            float xMax = VaultInventory.GameCanvas.pixelRect.xMax / VaultInventory.GameCanvas.scaleFactor;
            float yMin = VaultInventory.GameCanvas.pixelRect.yMin / VaultInventory.GameCanvas.scaleFactor;
            float yMax = VaultInventory.GameCanvas.pixelRect.yMax / VaultInventory.GameCanvas.scaleFactor;

            // updates for pivot-agnostic panels were contributed by Discord community user 'Plop'! Thanks!
            Vector2 pivot = new Vector2(TargetPanel.pivot.x, TargetPanel.pivot.y);
            Vector2 factor = new Vector2((TargetPanel.anchorMax.x + TargetPanel.anchorMin.x) / 2, (TargetPanel.anchorMax.y + TargetPanel.anchorMin.y) / 2);
            Vector2 result = new Vector2(
                Mathf.Clamp(
                    TargetPanel.anchoredPosition.x + moveDelta.x,
                    (xMin - xMax * factor[0] + TargetPanel.sizeDelta.x / 2 + TargetPanel.sizeDelta.x * (pivot[0] - 0.5f)),
                    (xMax - xMax * factor[0] - TargetPanel.sizeDelta.x / 2 + TargetPanel.sizeDelta.x * (pivot[0] - 0.5f))),
                Mathf.Clamp(
                    TargetPanel.anchoredPosition.y + moveDelta.y,
                    (yMin - yMax * factor[1] + TargetPanel.sizeDelta.y / 2 + TargetPanel.sizeDelta.y * (pivot[1] - 0.5f)),
                    (yMax - yMax * factor[1] - TargetPanel.sizeDelta.y / 2 + TargetPanel.sizeDelta.y * (pivot[1] - 0.5f))));

            TargetPanel.anchoredPosition = result;
        }
    }
}