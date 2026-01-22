using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        if (totalHealthBar != null)
        {
            totalHealthBar.fillAmount = 1; 
        }
    }
    private void Update()
    {
        if (GameManager.instance != null)
        {
            currentHealthBar.fillAmount = GameManager.instance.currentHealth / GameManager.instance.startingHealth;
        }
    }
}