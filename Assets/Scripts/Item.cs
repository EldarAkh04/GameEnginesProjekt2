using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    public int ID;
    public int NItem = 1;
    private TMP_Text NItemText;

    public int buyPrice = 10;
    [Range (0, 1)] public float sellPrice = 0.5f;

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
        NItemText.text = NItem > 1 ? NItem.ToString() : "";
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
}
