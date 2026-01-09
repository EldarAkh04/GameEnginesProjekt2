using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        totalHealthBar.fillAmount = GameManager.instance.currentHealth / 10;
    }

    private void Update()
    {
        if (GameManager.instance != null)
        {
            currentHealthBar.fillAmount = GameManager.instance.currentHealth / 10;
        }
    }
}
