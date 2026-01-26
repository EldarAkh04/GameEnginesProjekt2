using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    public GameObject infoPanel;

    void Start()
    {
        infoPanel.SetActive(false);
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
}
