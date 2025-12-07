using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInventory : MonoBehaviour
{
    public GameObject inventory;
    public bool isActive;

    void Start()
    {
        inventory.SetActive(false);
        isActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isActive)
            {
                hideInventory();
            }
            else
            {
                showInventory();
            }
        }
    }

    public void showInventory()
    {
        inventory.SetActive(true);
        Time.timeScale = 0f;
        isActive = true;
    }

    public void hideInventory()
    {
        inventory.SetActive(false);
        Time.timeScale = 1f;
        isActive = false;
    }
}
