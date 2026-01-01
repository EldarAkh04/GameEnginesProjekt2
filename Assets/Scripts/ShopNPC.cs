using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{

    public string shopID = "shop_";
    public string shopkeeperName = "Eldaro";

    public List<ShopStockItem> defaultShopStock = new();
    private List<ShopStockItem> currentShopStock = new();

    private bool isInitialized = false;

    [System.Serializable] 
    public class ShopStockItem
    {
        public int itemID;
        public int NItem;
    }
    void Start()
    {
        InitializeShop();
    }

    private void InitializeShop()
    {
        if (isInitialized)
        {
            return;
        }
        currentShopStock = new List<ShopStockItem>();
        foreach(var item in defaultShopStock)
        {
            currentShopStock.Add(new ShopStockItem
            {
                itemID = item.itemID,
                NItem = item.NItem
            });
        }
        isInitialized = true;
    }
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if(ShopController.Instance == null)
        {
            return;
        }
        if (ShopController.Instance.shopPanel.activeSelf)
        {
            ShopController.Instance.ColesShop();
        }
        else
        {
            ShopController.Instance.OpenShop(this);
        }
    }

    public List<ShopStockItem> GetCurrentStock()
    {
        return currentShopStock;
    }

    public void SetStock(List<ShopStockItem> stock)
    {
        currentShopStock = stock;
    }

    public void AddToStock(int itemID, int NItem)
    {
        ShopStockItem existing = currentShopStock.Find(s => s.itemID == itemID);
        if(existing != null)
        {
            existing.NItem += NItem;
        }
        else
        {
            currentShopStock.Add(new ShopStockItem{itemID = itemID, NItem = NItem});
        }
    }

    public bool RemoveFromShopStock(int itemID, int NItem)
    {
        ShopStockItem existing = currentShopStock.Find(s => s.itemID == itemID);
        if(existing != null && existing.NItem >= NItem)
        {
            existing.NItem -= NItem;
            return true;
        }
        return false;
    }
}
