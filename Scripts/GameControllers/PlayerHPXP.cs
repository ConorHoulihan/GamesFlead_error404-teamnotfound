using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPXP : MonoBehaviour
{
    public float playerHealth, playerMaxHealth = 100f, playerXp, playerDamage=20;
    public int potionCount = 0, currentLevel = 1, statPoints = 0;
    public bool isAlive = true;
    public Healthbar healthbar, XPBar;
    public PlayerMovements pm;
    private PhotonView PV;
    public Text XPtext, statPointsText, HpText;
    public GameObject LevelUpUI;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        playerHealth = playerMaxHealth;
        if (PV.IsMine)
        {
            PV.RPC("RPC_AddChar", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedChar);
            healthbar.SetMaxBar(playerMaxHealth);
            SetXpBarValue();
        }
    }

    private void Update()
    {
        if (isAlive)
        {
            if (Input.GetKeyDown(KeyCode.E) && potionCount > 0)
            {
                playerHealth += 20;
                potionCount--;
                healthbar.SetCurrent(playerHealth);
                HpText.text = playerHealth + "/" + playerMaxHealth;

                if (playerHealth > playerMaxHealth)
                {
                    playerHealth = playerMaxHealth;
                }
            }
            if (playerHealth <= 0)
            {
                isAlive = false;
            }

            if (statPoints > 0)
            {
                LevelUpUI.SetActive(true);
                statPointsText.text = "Stat Points Available:" + statPoints;
            }
            else
            {
                LevelUpUI.SetActive(false);
            }
        }
        else
        {
            PV.RPC("RPC_IsDead", RpcTarget.AllBuffered);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.IsChildOf(this.transform))
        {
            if (collision.gameObject.tag == "EnemyProjectile")
            {
                Debug.Log("You got Shot");
                playerHealth -= collision.transform.GetComponent<EnemyBullet>().GetDamage();
                healthbar.SetCurrent(playerHealth);
                HpText.text = playerHealth + "/" + playerMaxHealth;
            }
            else if (collision.gameObject.tag == "Baddie" || collision.gameObject.tag == "Baddie1")
            {
                Debug.Log("You got Stabbed");
                playerHealth -= collision.transform.GetComponent<BaddieHealth>().GetMeleeDamage();
                healthbar.SetCurrent(playerHealth);
                HpText.text = playerHealth + "/" + playerMaxHealth;
            }
        }
        if (collision.gameObject.tag == "Potion")
        {
            potionCount++;
        }

    }

    public void AddExp(float xp)
    {
        playerXp += xp;
        LevelTracker();
        SetXpBarValue();
    }

    public void LevelTracker()
    {
        if (playerXp >= (currentLevel * currentLevel) * 100)
        {
            currentLevel++;
        }
    }

    private void SetXpBarValue()
    {
        XPBar.SetCurrent(playerXp);
        XPBar.SetMaxBar((currentLevel * currentLevel) * 100);
        XPtext.text = currentLevel.ToString() + ":  " + playerXp.ToString() + "/" + ((currentLevel * currentLevel) * 100);
        statPoints += (int)Mathf.Ceil(currentLevel/2);
    }
    public void IncreaseHealth()
    {
        if (statPoints > 0)
        {
            playerMaxHealth *= 1.25f;
            playerHealth = playerMaxHealth;
            ReduceStatPoints();
            healthbar.SetMaxBar(playerMaxHealth);
            healthbar.SetCurrent(playerHealth);
            HpText.text = playerHealth + "/" + playerMaxHealth;
        }
    }

    public void ReduceStatPoints()
    {
        statPoints--;
    }

    public int GetStatPoints()
    {
        return statPoints;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    [PunRPC]
    void RPC_IsDead()
    {
    foreach (Transform child in transform)
        {
            if (child.transform.name != "Camera")
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
