using UnityEngine;

public class Health : MonoBehaviour
{

    public void TakeDamage(float _damage)
    {
        GameManager.instance.ChangeHealth(-_damage);
        if (GameManager.instance.currentHealth > 0)
        {
            //player hurt
        } else
        {
            //player dies
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }
    }

}

