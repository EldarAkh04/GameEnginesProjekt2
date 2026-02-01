using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private UI_Fade fadeUI;

    //Health
    [SerializeField] public float startingHealth = 5.0f;
    [SerializeField] private GameObject gameOverPanel;
    public float currentHealth { get; private set; }

    private void Awake()
    {
        currentHealth = startingHealth;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject); 
        }
        else
        {
            Destroy(transform.root.gameObject); 
        }

        if (gameOverPanel != null){
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f;
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

    public void ChangeHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        if (currentHealth <= 0)
        {
            TriggerGameOver();
            Debug.Log("Game Over!");
        }
    }

    public void HealPlayer()
    {
        currentHealth += 1;
    }

    public void PoweUpPlayer()
    {
        startingHealth += 1;
        currentHealth += 1;
    }

    // Im GameManager.cs hinzufügen
    public void LoadHealthData(float health, float maxHealth)
    {
    startingHealth = maxHealth;
    currentHealth = health;

    // Wichtig: Falls du die HealthBar direkt aktualisieren willst, 
    // falls diese nicht nur über Update() läuft.
    }

    public void LeaveGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f; 
        MenuController menu = FindObjectOfType<MenuController>();
        if (menu != null && menu.menuCanvas != null)
        {
            menu.menuCanvas.SetActive(false);
        }
        SceneManager.LoadScene("Scenes/StartScene");
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over!");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; 
            currentHealth = startingHealth;
        }
    }
}
