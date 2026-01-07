using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveController : MonoBehaviour
{
    private string saveLoc;
    private InventoryController inventoryController;
    private ShopNPC[] shops;

    void Start()
    {
        saveLoc = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindObjectOfType<InventoryController>();
        shops = FindObjectsOfType<ShopNPC>();
        LoadGame();
    }

    public void saveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            inventorySaveData = inventoryController.GetInventoryItems(),
            playerGold = MoneyManager.Instance.GetGold(), 
            shopStates = GetShopStates()
        };

        File.WriteAllText(saveLoc, JsonUtility.ToJson(saveData));
    }

    private List<ShopInstanceData> GetShopStates()
    {
        List<ShopInstanceData> shopStatesList = new List<ShopInstanceData>();
        foreach(var shop in shops)
        {
            ShopInstanceData shopData = new ShopInstanceData
            {
                shopID = shop.shopID,
                stock = new List<ShopItemData>()
            };

            foreach (var stockItem in shop.GetCurrentStock())
            {
                shopData.stock.Add(new ShopItemData
                {
                    itemID = stockItem.itemID,
                    NItem = stockItem.NItem
                });
            }
            shopStatesList.Add(shopData);
        }
        return shopStatesList;
    }

    public void LoadGame()
    {
        if (File.Exists(saveLoc))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLoc));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPos;
            inventoryController.SetInventoryItems(saveData.inventorySaveData);

            LoadShopStates(saveData.shopStates);
            MoneyManager.Instance.SetGold(saveData.playerGold);
        }
    }

    private void LoadShopStates(List<ShopInstanceData> shopStates)
    {
        if (shopStates != null)
        {
            foreach(var shop in shops)
            {
                var shopData = shopStates.FirstOrDefault(s => s.shopID == shop.shopID);
                if(shopData != null)
                {
                    List<ShopNPC.ShopStockItem> loadedStock = new List<ShopNPC.ShopStockItem>();
                    foreach (var itemData in shopData.stock)
                    {
                        loadedStock.Add(new ShopNPC.ShopStockItem
                        {
                            itemID = itemData.itemID,
                            NItem = itemData.NItem
                        });
                    }
                    shop.SetStock(loadedStock);
                }
            }
        }
    }
}