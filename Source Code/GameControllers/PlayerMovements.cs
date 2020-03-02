using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerMovements : MonoBehaviour, IPunObservable
{
    private float moveSpeed = 15.0f, currentSpeed, dashSpeed = 30f, playerDamage = 20, dashTimer, dashDuration = .1f, dashDelay = 1, dashTime = -1f, angle, fireTimer, fireRate = 1f, FireTime = -1f;
    public float AttackRange = 3;
    private bool canDash = true, canFire = true, exposition=true;
    public bool meleeChar = false;

    private Rigidbody2D body;
    private Vector2 movement, mousePos;
    private PhotonView PV;
    public UnityEngine.Camera cam;
    public AudioListener audioL;
    public Transform middleP;
    public GameObject projectilePrefab, expositionBox;
    public Transform arm;
    public Canvas canvas;
    public LayerMask whatIsEnemy;
    public Healthbar dashBar, attackBar;
    public PlayerHPXP pHpXp;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            ActivePlayer(false);
        }
        StartCoroutine(DelaySetMelee());
    }

    void Update()
    {
        if (PV.IsMine)
        {
            dashTimer = dashTime - Time.time;
            dashBar.SetMaxBar(dashDelay);
            dashBar.SetCurrent(dashTimer);

            fireTimer = FireTime - Time.time;
            attackBar.SetCurrent(fireTimer);
            attackBar.SetMaxBar(fireRate);

            if (dashTimer < 0)
            {
                dashTimer = 0;
                canDash = true;
            }
            if (fireTimer < 0)
            {
                fireTimer = 0;
                canFire = true;
            }

            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            middleP.position = body.position - new Vector2(0, 0.45f);

            var v3 = Input.mousePosition;
            v3.z = 10.0F;

            mousePos = cam.ScreenToWorldPoint(v3);
            if (pHpXp.IsAlive())
            {
                if (Input.GetKeyDown(KeyCode.Space) && canFire)
                {
                    if (!meleeChar)
                    {
                        PV.RPC("RPC_FireProjectile", RpcTarget.AllViaServer);
                    }
                    else
                    {
                        FireTime = Time.time + fireRate;
                        canFire = false;
                        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(arm.position, AttackRange, whatIsEnemy);
                        for (int i = 0; i < enemiesToDamage.Length; i++)
                        {
                            enemiesToDamage[i].GetComponent<BaddieHealth>().TakeDamage(playerDamage);
                            enemiesToDamage[i].transform.position = Vector2.MoveTowards(enemiesToDamage[i].transform.position, transform.position, -1 * 50 * Time.deltaTime);
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
                {
                    canDash = false;
                    dashTime = Time.time + dashDuration + dashDelay;
                    currentSpeed = dashSpeed;
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    exposition = false;
                }
                else if (Time.time > dashTime - dashDelay)
                {
                    currentSpeed = moveSpeed;
                }
            }
            if (!exposition)
                expositionBox.SetActive(false);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(arm.position, AttackRange);
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            body.velocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed);
            Vector2 aimDir = mousePos - body.position;
            angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + 90f;
            middleP.rotation.Set(0, 0, angle, 0);
            middleP.transform.eulerAngles = new Vector3(middleP.transform.eulerAngles.x, middleP.transform.eulerAngles.y, angle);
        }
    }

    public void ActivePlayer(bool isActiveP)
    {
        cam.enabled = isActiveP;
        audioL.enabled = isActiveP;
        canvas.enabled = isActiveP;
    }

    public void IncreaseDamage()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            playerDamage *= 1.25f;
            pHpXp.ReduceStatPoints();
        }
    }

    public void IncreaseAspeed()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            fireRate *= .8f;
            pHpXp.ReduceStatPoints();
        }
    }

    public void ReduceDashDelay()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            dashDelay *= .8f;
            pHpXp.ReduceStatPoints();
        }
    }

    public void MoveSpeedUp()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            moveSpeed *= 1.25f;
            pHpXp.ReduceStatPoints();
        }
    }

    public float GetDamage()
    {
        return playerDamage;
    }

    IEnumerator DelaySetMelee()
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in transform)
        {
            Debug.Log(child.name);
            if (child.name == "PlayerAvatar2(Clone)")
            {
                meleeChar = true;
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(middleP.localRotation);
            if (body)
                stream.SendNext(body.velocity);
        }
        else if (stream.IsReading)
        {
            middleP.localRotation = (Quaternion)stream.ReceiveNext();
            if (body)
                body.velocity = (Vector2)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void RPC_FireProjectile()
    {
        FireTime = Time.time + fireRate;
        canFire = false;
        if (PV.IsMine)
        {
            var bulletRef = PhotonNetwork.Instantiate(System.IO.Path.Combine("PhotonPrefabs", "Projectile"),
                arm.transform.position, Quaternion.Euler(0, 0, angle + 180), 0);
            bulletRef.GetComponent<FireBullet>().SetDamageDAndPlayer(playerDamage,this.transform);
            Physics2D.IgnoreCollision(bulletRef.GetComponent<Collider2D>(), GetComponentInChildren<Collider2D>(), true);

        }
    }
}
