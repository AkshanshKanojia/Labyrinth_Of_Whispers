using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPS
{
    public class PlayerInventory : Singleton<PlayerInventory>
    {
        [Header("Inventory Data")]
        private int _defaultInventorySize = 6;

        internal InventoryData activeInventoryData;

        private const string KEY_INVENTORY = "Inventory";
        private const string INVETORY_DATA_PATH = "InventoryData";

        private InventoryItemDataSCO _inventoryItemData;

        #region Initialization

        private void Start()
        {
            Initialize();//todo: remove later once game flow is finalized
        }
        internal void Initialize()
        {
            LoadInventoryItemData();
            LoadInventory();//load default inventory if any
        }

        private void LoadInventoryItemData()
        {
            _inventoryItemData = Resources.Load<InventoryItemDataSCO>(INVETORY_DATA_PATH);
        }

        #endregion

        #region Inventory Methods

        internal void SaveInventory()
        {
            string inventoryData = JsonUtility.ToJson(activeInventoryData);
            PlayerPrefs.SetString(KEY_INVENTORY, inventoryData);
        }

        internal void LoadInventory()
        {
            string inventoryData = PlayerPrefs.GetString(KEY_INVENTORY);

            if (!string.IsNullOrEmpty(inventoryData))
            {
                InventoryData inventorySaveData = JsonUtility.FromJson<InventoryData>(inventoryData);
                activeInventoryData = inventorySaveData;
            }
            else
            {
                //generate default inventory
                activeInventoryData = new InventoryData
                {
                    currentInventorySize = _defaultInventorySize,
                    itemsInInventory = new List<InventoryItem>()
                };
            }
        }

        internal bool CheckInventorySpace(InventoryItemType itemType, int amountToAdd)
        {
            int expectedStacks = 0;

            if (activeInventoryData.itemsInInventory.Count < activeInventoryData.currentInventorySize)
            {
                return true;
            }

            //if all slots are preoccupied check for stack limit

            InventoryItemData inventoryItemData = _inventoryItemData.GetInventoryItemData(itemType);

            expectedStacks = GetExpectedStacks(itemType, amountToAdd);

            if (activeInventoryData.itemsInInventory.Any(i => i.item == itemType))
            {
                //first check if adding item in current stack is less than max stack limit if not get count of stacks that will be generated after adding and compare with inventory size

                InventoryItem inventoryItem = activeInventoryData.itemsInInventory.FirstOrDefault(x => x.item == itemType);

                int totalAmount = inventoryItem.amount + amountToAdd;

                if (totalAmount <= inventoryItemData.maxStackAmount)
                {
                    return true;
                }

                if (expectedStacks + activeInventoryData.itemsInInventory.Count <= activeInventoryData.currentInventorySize)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else
            {
                //inventory item not present, check for stacks that will be generated after adding and compare with inventory size
                if (expectedStacks + activeInventoryData.itemsInInventory.Count <= activeInventoryData.currentInventorySize)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private int GetExpectedStacks(InventoryItemType itemType, int ammount)
        {
            InventoryItemData inventoryItemData = _inventoryItemData.GetInventoryItemData(itemType);
            if (inventoryItemData != null)
            {
                return Mathf.CeilToInt((float)ammount / inventoryItemData.maxStackAmount);
            }
            else
            {
                Debug.LogError("Inventory Item Data not found for item: " + itemType);
                return 0;
            }
        }

        internal bool AddItem(InventoryItemType item, int amount, bool saveAfterModification = true)
        {
            if (!CheckInventorySpace(item, amount))
            {
                Debug.LogWarning("Inventory Full, should not be adding this item!");
                return false;
            }

            InventoryItemData itemData = _inventoryItemData.GetInventoryItemData(item);
            if (itemData == null)
            {
                Debug.LogError("Inventory Item Data not found for item: " + item);
                return false;
            }

            InventoryItem existingStack = activeInventoryData.itemsInInventory.FirstOrDefault(x => x.item == item);

            if (existingStack != null)
            {
                int spaceLeft = itemData.maxStackAmount - existingStack.amount;
                int toAdd = Mathf.Min(spaceLeft, amount);
                existingStack.amount += toAdd;
                amount -= toAdd; // Reduce the remaining amount
            }

            // If there's still more to add, create new stacks
            while (amount > 0 && activeInventoryData.itemsInInventory.Count < activeInventoryData.currentInventorySize)
            {
                int toAdd = Mathf.Min(amount, itemData.maxStackAmount);
                activeInventoryData.itemsInInventory.Add(new InventoryItem { item = item, amount = toAdd });
                amount -= toAdd;
            }

            if (saveAfterModification)
            {
                SaveInventory();
            }

            return amount <= 0; // If `amount` is still >0, inventory was full
        }

        internal InventoryItem GetItem(InventoryItemType item)
        {
            InventoryItem tempItenm = activeInventoryData.itemsInInventory.FirstOrDefault(x => x.item == item);

            return tempItenm;
        }

        internal void AdjustItemQuantity(InventoryItemType item, int amountToAdd, bool saveAfterModification = true)
        {
            InventoryItem tempItem = activeInventoryData.itemsInInventory.FirstOrDefault(x => x.item == item);
            if (tempItem != null)
            {
                tempItem.amount += amountToAdd;
            }
            else
            {
                Debug.LogError("Item not found in inventory: " + item);
            }

            if (saveAfterModification)
            {
                SaveInventory();
            }
        }
        #endregion
    }

    [Serializable]
    public class InventoryItem
    {
        public InventoryItemType item;
        public int amount;
    }

    [Serializable]
    public class InventoryData
    {
        public List<InventoryItem> itemsInInventory = new List<InventoryItem>();
        public int currentInventorySize;
    }

    public enum InventoryItemType
    {
        //add items as per game requirements
        
        //weapons/ammos
        PistolAmmo,
        Pistol,
       
        //crafting items
        Syringe,
        NailBomb,
        NoiseMaker,
        SmokeBomb,

        //consumables
        GlowStick,
        FlashlightBattery,

        //special/puzzle items
        Fuse,
    }
}
