using System;
using UnityEngine;

namespace FPS
{
    [CreateAssetMenu(fileName = "InventoryItemData", menuName = "FPS/InventoryItemData", order = 1)]
    public class InventoryItemDataSCO : ScriptableObject
    {
        [SerializeField] private InventoryItemData[] _inventoryItemData;

        public InventoryItemData GetInventoryItemData(InventoryItemType item)
        {
            return Array.Find(_inventoryItemData, x => x.item == item);
        }
    }

    [Serializable]
    public class InventoryItemData
    {
        public InventoryItemType item;
        public int maxStackAmount;

        public string itemName;
        public string itemDescription;
        //add fields as needed
    }
}
