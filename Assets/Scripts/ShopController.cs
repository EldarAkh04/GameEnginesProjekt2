using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("UI")]
    public GameObject shopPanel;
    public Transform shopInvGrid, playerInvGrid;
    public GameObject shopSlotPref;
    public TMP_Text playerMoneyText, shopTitleText;

    private ItemDictionary itemDictionary;
    private ShopNPC currentShop;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        shopPanel.SetActive(false);
        if(MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnGoldChanged += UpdateMoneyDisplay;
            UpdateMoneyDisplay(MoneyManager.Instance.GetGold());
        }
    }

    private void UpdateMoneyDisplay(int amount)
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = amount.ToString();
        }
    }

    public void OpenShop(ShopNPC shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        if(shopTitleText != null)
        {
            shopTitleText.text = shop.shopkeeperName + "'s Shop";
        }
        RefreshShopDisplay();
        RefreshPlayerInvDisplay();
        PauseController.SetPause(true);
    }

    public void ColesShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
        PauseController.SetPause(false);
    }

    public void RefreshShopDisplay()
    {
        if (currentShop == null)
        {
            return;
        }
        
        foreach (Transform child in shopInvGrid)
        {
            Destroy(child.gameObject);
        }
        foreach (var stockItem in currentShop.GetCurrentStock()) 
        {
            if (stockItem.NItem <= 0)
            {
                continue;
            }
            CreatShopSlot(shopInvGrid, stockItem.itemID, stockItem.NItem, true);
        }
    }

    public void RefreshPlayerInvDisplay()
    {
        if(InventoryController.Instance == null)
        {
            return;
        }
        foreach(Transform child in playerInvGrid)
        {
            Destroy(child.gameObject);
            Debug.Log("Test");
        }
        foreach(Transform slotTransfrom in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot invSlot = slotTransfrom.GetComponent<Slot>();
            if(invSlot?.currentItem != null)
            {
                Item originalItem = invSlot.currentItem.GetComponent<Item>();
                CreatShopSlot(playerInvGrid, originalItem.ID, originalItem.NItem, false, invSlot);

            }
        }
    }

    private void CreatShopSlot(Transform grid, int itemID, int NItem, bool isShop, Slot originalSlot = null)
    {
        GameObject slotObj = Instantiate(shopSlotPref, grid);
        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);
        
        if (itemPrefab == null) return;

        GameObject itemInstance = Instantiate(itemPrefab, slotObj.transform);

        itemInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
        Item item = itemInstance.GetComponent<Item>();
        item.NItem = NItem;
        item.UpdateTextDisplay();

        int price = isShop ? item.buyPrice : item.GetSellPrice();

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();
        if (slot != null)
        {
            slot.isShopSlot = isShop;
            slot.SetItem(itemInstance, price);
        }
    }
}
