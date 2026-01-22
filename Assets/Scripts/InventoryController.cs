using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public static InventoryController Instance {get; private set;}
    Dictionary<int, int> itemsCountCash = new();
    public event Action OnInventoryChenged;

    [SerializeField] public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    private List<InventorySaveData> persistentData = new List<InventorySaveData>();

    /* private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject); 
        }
        else {
            Destroy(gameObject);
        }
    } */

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject foundPanel = GameObject.Find("InventoryPage");
        if (foundPanel != null)
        {
            inventoryPanel = foundPanel;
            if (persistentData != null && persistentData.Count > 0)
            {
                SetInventoryItems(persistentData);
            }
        }
    }

    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        RebuildItemCounts();
    }

    void Awake()
    {
        // Das gesamte Menü-System als Singleton
        if (Instance == null)
        {
            
            Instance = this;
            DontDestroyOnLoad(gameObject); // Das Objekt wird nicht gelöscht
        }
        else
        {
            Destroy(gameObject); // Verhindert doppelte Menüs
        }
    }

    public void RebuildItemCounts()
    {
        itemsCountCash.Clear();
        if (inventoryPanel == null) return;

        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
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

    public void SaveInventoryToData()
    {
        persistentData = GetInventoryItems();
    }

    public Dictionary<int, int> GetItemCounts() => itemsCountCash;

    public bool AddItem(GameObject itemPrefab)
    {
        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if (itemToAdd == null) return false;
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if(slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.itemInInv = true;
                    slotItem.AddToStack();
                    RebuildItemCounts();
                    SaveInventoryToData();
                    return true;
                }
            }
        }

        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                Item newItemScript = newItem.GetComponent<Item>();
                if(newItemScript != null) 
                {
                    newItemScript.itemInInv = true;
                }

                RectTransform rect = newItem.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(0, 0, 0); 
                rect.localScale = Vector3.one;
                rect.anchoredPosition = Vector2.zero;
                
                slot.currentItem = newItem;
                RebuildItemCounts();
                SaveInventoryToData();
                return true;
            }
        }
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invDate = new List<InventorySaveData>();
        if (inventoryPanel == null) return invDate;

        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
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
        if (inventoryPanel == null)
        {
            inventoryPanel = GameObject.Find("InventoryPage");
            if (inventoryPanel == null) return;
        }

        if (itemDictionary == null)
        {
            itemDictionary = FindObjectOfType<ItemDictionary>();
        }

        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<Slot> newSlots = new List<Slot>();
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, inventoryPanel.transform);
            newSlots.Add(slotObj.GetComponent<Slot>());
        }

        if (inventorySaveData != null)
        {
            foreach (InventorySaveData data in inventorySaveData)
            {
                if (data.slotIndex < newSlots.Count)
                {
                    Slot slot = newSlots[data.slotIndex];
                    GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                    
                    if (itemPrefab != null)
                    {
                        GameObject item = Instantiate(itemPrefab, slot.transform);
                        RectTransform rect = item.GetComponent<RectTransform>();
                        
                        rect.localScale = Vector3.one;
                        rect.localPosition = new Vector3(0, 0, 0); 
                        rect.anchoredPosition = Vector2.zero;

                        Item itemComponent = item.GetComponent<Item>();
                        if (itemComponent != null)
                        {
                            itemComponent.itemInInv = true;
                            itemComponent.NItem = data.NItem;
                            itemComponent.UpdateTextDisplay();
                        }
                        slot.currentItem = item;
                    }
                }
            }
        }
        persistentData = inventorySaveData;
        RebuildItemCounts();
    }

    public void UseItem(int id)
    {
        if (id != 1 && id != 3) 
        {
            Debug.Log("Nix passiert");
            return;
        }
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null && item.ID == id)
                {
                    item.RemoveToStack(1);
                    if (item.NItem <= 0)
                    {
                        Destroy(slot.currentItem);
                        slot.currentItem = null;
                    }
                    RebuildItemCounts();
                    SaveInventoryToData();
                    return;
                }
            }
        }
    }
    public void UsePotionItem(int id) 
    {
        if(id == 3)
        {
            if(GameManager.instance != null)
            {
                GameManager.instance.HealPlayer();
                Debug.Log("Heilung durchgeführt!");
            }
        }
    }

    public void UsePowerUpItem(int id)
    {
        if(id == 1)
        {
            if(GameManager.instance != null)
            {
                GameManager.instance.PoweUpPlayer();
            }
        }
    }
}