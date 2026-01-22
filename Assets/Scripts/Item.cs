using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerClickHandler
{
    public int ID;
    public int NItem = 1;
    private TMP_Text NItemText;

    public int buyPrice = 10;
    [Range (0, 1)] public float sellPrice = 0.5f;
    public bool itemInInv = false;

    private void Awake()
    {
        NItemText = GetComponentInChildren<TMP_Text>();
        UpdateTextDisplay();
    }

    public int GetSellPrice()
    {
        return Mathf.RoundToInt(buyPrice * sellPrice);
    }

    public void UpdateTextDisplay()
    {
        if (NItemText != null) 
        {
            NItemText.text = NItem > 1 ? NItem.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        NItem += amount;
        UpdateTextDisplay();
    }

    public int RemoveToStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, NItem);
        NItem -= removed;
        UpdateTextDisplay();
        return removed;
    }

    public GameObject clonItem(int newItem)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.NItem = newItem;
        cloneItem.UpdateTextDisplay();
        return clone;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemInInv && InventoryController.Instance != null)
        {
            if(GameManager.instance.currentHealth != GameManager.instance.startingHealth && this.ID ==3){
                InventoryController.Instance.UseItem(this.ID);
                InventoryController.Instance.UsePotionItem(this.ID);
                Debug.Log("Effect");
            }
            else if(this.ID == 1)
            {
                InventoryController.Instance.UseItem(this.ID);
                InventoryController.Instance.UsePowerUpItem(this.ID);
                Debug.Log("Power up");
            }   
        }
        /* if (ID == 1 && ID == 3)
        {
            PerformSpecialAction();
        } */
    }

   /*  private void PerformSpecialAction()
    {
        Debug.Log("Spezialaktion für ID 1 ausgeführt (z.B. Heilungseffekt).");
    } */
}