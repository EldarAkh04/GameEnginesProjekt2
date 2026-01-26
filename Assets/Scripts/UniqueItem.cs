using UnityEngine;
using System.IO;

public class UniqueItem : MonoBehaviour
{
    private Item itemScript;
    private string savePath;

    void Start()
    {
        itemScript = GetComponent<Item>();
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        if (itemScript == null) return;
        if (itemScript.itemInInv) return;
        if (InventoryController.Instance != null)
        {
            if (IsItemInCurrentInventory(itemScript.ID))
            {
                Debug.Log($"Item {itemScript.ID} im RAM gefunden. Zerstöre Welt-Objekt.");
                Destroy(gameObject);
                return;
            }
        }
        if (IsItemInSaveFile(itemScript.ID))
        {
            Debug.Log($"Item {itemScript.ID} in saveData.json gefunden. Zerstöre Welt-Objekt.");
            Destroy(gameObject);
        }
    }

    private bool IsItemInCurrentInventory(int id)
    {
        var counts = InventoryController.Instance.GetItemCounts();
        return counts.ContainsKey(id) && counts[id] > 0;
    }

    private bool IsItemInSaveFile(int id)
    {
        if (!File.Exists(savePath)) return false;
        try
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data != null && data.inventorySaveData != null)
            {
                foreach (var invItem in data.inventorySaveData)
                {
                    if (invItem.itemID == id) return true;
                }
            }
        }
        catch {}
        return false;
    }
}