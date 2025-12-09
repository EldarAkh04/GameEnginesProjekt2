using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] panels;
    private static readonly Color INACTIVE_COLOR = Color.grey;
    private static readonly Color ACTIVE_COLOR = Color.white;
    void Start()
    {
        ActiveTab(0);
    }

    public void ActiveTab(int tabNo)
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
            tabImages[i].color = INACTIVE_COLOR;
        }
        panels[tabNo].SetActive(true);
        tabImages[tabNo].color = ACTIVE_COLOR;
    }
}
