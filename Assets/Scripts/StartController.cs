using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class StartController : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject ladGameButton;

    void Start()
    {
        infoPanel.SetActive(false);
        ladGameButton.SetActive(false);
        string path = Path.Combine(Application.persistentDataPath, "saveData.json");
        if (File.Exists(path))
        {
            ladGameButton.SetActive(true);
        }
    }
    public void OnStartLoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    Application.Quit();
    }

    public void OnStartNewGame()
    {
        infoPanel.SetActive(true);
    }

    public void GoBackToMenu()
    {
        infoPanel.SetActive(false);
    }

    public void StartNewAdventure()
    {
        string path = Path.Combine(Application.persistentDataPath, "saveData.json");
        if(File.Exists(path))
        {
            File.Delete(path);
        }
        SceneManager.LoadScene("SampleScene");
    }
}
