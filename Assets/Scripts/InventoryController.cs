using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public static InventoryController Instance {get; private set;}
    Dictionary<int, int> itemsCountCash = new();
    public event Action OnInventoryChenged;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        RebuildItemCounts();
    }
    
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }
    public void RebuildItemCounts()
    {
        itemsCountCash.Clear();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if(item != null)
                {
                    itemsCountCash[item.ID] = itemsCountCash.GetValueOrDefault(item.ID, 0) + item.NItem;
                }
            }
        }

        OnInventoryChenged?.Invoke();
    }

    public Dictionary<int, int> GetItemCounts() => itemsCountCash;

    public  bool AddItem(GameObject itemPrefab)
    {

        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if (itemToAdd == null)
        {
            return false;
        }
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if(slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.AddToStack();
                    RebuildItemCounts();
                    return true;
                }
            }
        }
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                GameObject newItem =  Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                RebuildItemCounts();
                return true;
            }
        }
        Debug.Log("Inv is full");
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invDate = new List<InventorySaveData>();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invDate.Add(new InventorySaveData { 
                    itemID = item.ID, 
                    slotIndex = slotTransform.GetSiblingIndex(),
                    NItem = item.NItem
                });
            }
        }
        return invDate;
    }
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        foreach(Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0; i< slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
        foreach(InventorySaveData data in inventorySaveData)
        {
            if(data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if(itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                Item itemComponent = item.GetComponent<Item>();
                if(itemComponent != null && data.NItem > 1)
                {
                    itemComponent.NItem = data.NItem;
                    itemComponent.UpdateTextDisplay();
                }

                    slot.currentItem = item;
                }
            }
        }
        RebuildItemCounts();
    }
}
