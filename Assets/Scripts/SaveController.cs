using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{
    private string saveLoc;
    private InventoryController inventoryController;

    void Start()
    {
        saveLoc = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindObjectOfType<InventoryController>();
        LoadGame();
    }

    public void saveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            inventorySaveData = inventoryController.GetInventoryItems()
            
        };

        File.WriteAllText(saveLoc, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLoc))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLoc));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPos;
            inventoryController.SetInventoryItems(saveData.inventorySaveData);
        }
        else
        {
            saveGame();
        }
    }
}
