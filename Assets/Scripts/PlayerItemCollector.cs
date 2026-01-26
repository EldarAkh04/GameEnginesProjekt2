using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    private SaveController saveController;
    
    void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if(item != null)
            {
                bool itemAdd = inventoryController.AddItem(collision.gameObject);
                if (itemAdd)
                {
                    if (saveController != null)
                    {
                        saveController.saveGame();
                        Debug.Log("Spielstand nach Item-Aufnahme aktualisiert.");
                    }
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
