using UnityEngine;
using UnityEngine.UI;

public class HealthBarA : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Update()
    {
        if (GameManager.instance != null && slider != null)
        {
            slider.maxValue = GameManager.instance.startingHealth;
            slider.value = GameManager.instance.currentHealth;
        }
    }
}