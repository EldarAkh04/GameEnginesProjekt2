using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private UI_Fade fadeUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject); 
        }
        else
        {
            Destroy(transform.root.gameObject); 
        }
    }

    public void ChangeLevelTo(string levelName)
    {
        //StartCoroutine(ChangeLevelCo(levelName));
        Debug.Log("Wechsel zu Level: " + levelName);
    }

    /* private IEnumerator ChangeLevelCo(string levelName)
    {
        GetFadeUI().DoFadeOut();
        yield return GetFadeUI().fadeEffectCo; // Wartezeit f�r den Effekt
        SceneManager.LoadScene(levelName);
    }

    private UI_Fade GetFadeUI()
    {
        if (fadeUI == null)
            fadeUI = FindObjectOfType<UI_Fade>();
    
        return fadeUI;
    } */
}
