using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    
    void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
    }
    //Test
    
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
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
