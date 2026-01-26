using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public static bool IsMenuOpen = false;

    public static MenuController Instance;

    void Start()
    {
        menuCanvas.SetActive(false);
        Time.timeScale = 1f;
        IsMenuOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!IsShopOpen()) 
            {
                ToggleMenu();
            } 
        }
    }

    void ToggleMenu()
    {
        bool isPaused = !menuCanvas.activeSelf;
        menuCanvas.SetActive(isPaused);
        IsMenuOpen = isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = 1f; 
        }
    }

    bool IsShopOpen()
    {
        return ShopController.Instance != null && 
               ShopController.Instance.shopPanel != null && 
               ShopController.Instance.shopPanel.activeSelf;
    }
}