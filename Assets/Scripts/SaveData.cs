using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public List<InventorySaveData> inventorySaveData;
    public int playerGold;
    public List<ShopInstanceData> shopStates = new();
    public float currentHealth;
    public float startingHealth;
}

    [System.Serializable]

    public class ShopInstanceData
    {
        public string shopID;
        public List<ShopItemData> stock = new();
    }

    [System.Serializable]

    public class ShopItemData
    {
        public int itemID; 
        public int NItem;
    }
