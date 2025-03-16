using TMPro;
using UnityEngine;

namespace FPS
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _quickbarKeyText;
        [SerializeField] private GameObject _quickbarSelectionUI;
        [SerializeField] private int _maxQuickbarSlots = 4;

        internal InventoryItem itemInSlot;
        internal int slotIndex;
        internal bool isActiveSlot;

        internal void Initialize(int index)
        {
            _quickbarSelectionUI.SetActive(index < _maxQuickbarSlots);

            slotIndex = index;
            _quickbarKeyText.text = (index + 1).ToString();
        }

        internal void SetSlotActive(bool isActive)
        {
            isActiveSlot = isActive;
            gameObject.SetActive(isActive);
        }
    }
}
