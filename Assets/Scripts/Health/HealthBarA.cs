using UnityEngine;
using UnityEngine.UI;

public class HealthBarA : MonoBehaviour
{
    public Slider slider;

/*
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    
    public void SetHealth(int health)
    {
        slider.value = health;
    }
*/
     private void Start()
    {
        slider.maxValue = GameManager.instance.currentHealth;
        slider.value = GameManager.instance.currentHealth;
    }

    private void Update()
    {
        if (GameManager.instance != null)
        {
            slider.value = GameManager.instance.currentHealth;
        }
    }
}
