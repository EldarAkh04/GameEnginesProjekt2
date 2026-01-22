using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemHandler : MonoBehaviour, IPointerClickHandler
{
    private bool isShopItem;
    public Slot originalInventorySlot;

    public void Initialise(bool shopItem) => isShopItem = shopItem;

    public void OnPointerClick(PointerEventData evenData)
    {
        if(evenData.button == PointerEventData.InputButton.Left || evenData.button == PointerEventData.InputButton.Right)
        {
            if (isShopItem)
            {
                BuyItem();
            }
            else
            {
                SellItem();
            }
        }
    }

    private void BuyItem()
    {
        Item item = GetComponent<Item>();
        ShopSlot slot = GetComponentInParent<ShopSlot>();
        if(!item || !slot)
        {
            return;
        }

        if(MoneyManager.Instance.GetGold() < slot.itemPrice)
        {
            Debug.Log("Not enough gold...");
            return;
        }

        GameObject itemPrefab = FindObjectOfType<ItemDictionary>().GetItemPrefab(item.ID);
        if (InventoryController.Instance.AddItem(itemPrefab))
        {
            MoneyManager.Instance.SpendGold(slot.itemPrice);
            ShopController.Instance.RefreshPlayerInvDisplay();
            ShopController.Instance.RemoveItemFromShop(item.ID, 1);
        }
        else
        {
            Debug.Log("Inventory full");
        }
    }

    private void SellItem()
    {
        Item item = GetComponent<Item>();
        ShopSlot slot = GetComponentInParent<ShopSlot>();
        if(!item || !slot || !originalInventorySlot)
        {
            return;
        }

        Item invItem = originalInventorySlot.currentItem?.GetComponent<Item>();
        if (!invItem)
        {
            return;
        }

        if(invItem.NItem > 1)
        {
            invItem.RemoveToStack(1);
        }
        else
        {
            Destroy(originalInventorySlot.currentItem);
            originalInventorySlot.currentItem = null;
        }

        InventoryController.Instance.RebuildItemCounts();
        MoneyManager.Instance.AddGold(slot.itemPrice);
        ShopController.Instance.RefreshPlayerInvDisplay();
        ShopController.Instance.AddItemToShop(item.ID, 1);
    }
}
