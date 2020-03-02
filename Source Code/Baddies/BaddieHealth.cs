using UnityEngine;

public class BaddieHealth : MonoBehaviour
{
    public float currentHP = 100, maxHP = 100, Meleedamage = 25;

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public float GetExp()
    {
        return maxHP / 2;
    }

    public float getHP()
    {
        return currentHP;
    }

    public float getMaxHP()
    {
        return maxHP;
    }

    public float GetMeleeDamage()
    {
        return Meleedamage;
    }
}
