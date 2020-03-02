using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    private PhotonView PV;
    private Rigidbody2D rb;
    public Collider2D arm1, arm2;
    public Transform spawnPoint, firePoint, spinPoint1, spinPoint2, middle, closestPlayer;
    private float spawnTime = -1f, spawnDelay = 5f, fireTime = -1f, damage = 20, angle = 0, armAngle = 270, armMoveCount = 1, baddieCount = 0;
    private bool goingDown = true, firstboost=true;
    public float fireDelay = .3f;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(arm1, GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(arm2, GetComponent<Collider2D>(), true);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, middle.position, 3*Time.deltaTime);

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            float temp = Vector2.Distance(go.transform.position, transform.position);
            if (!closestPlayer)
            {
                closestPlayer = go.transform;
            }
            else if (temp < Vector2.Distance(closestPlayer.transform.position, transform.position))
            {
                closestPlayer = go.transform;
            }
        }
        baddieCount = 0;
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.tag == "Baddie1")
            {
                baddieCount++;
            }
        }
        if (closestPlayer)
        {
            if ((Time.time > spawnTime) && (Vector2.Distance(closestPlayer.position, transform.position) < 30))
            {
                spawnTime = Time.time + spawnDelay;
                if (baddieCount < 5)
                    PV.RPC("RPC_SpawnMinion", RpcTarget.AllViaServer);
            }
            if (Time.time > fireTime && (Vector2.Distance(closestPlayer.position, transform.position) < 30))
            {
                fireTime = Time.time + fireDelay;
                angle += 10;
                PV.RPC("RPC_FireProjectile", RpcTarget.AllViaServer);
            }
        }
        if(transform.position == middle.position)
        {
            arm1.gameObject.SetActive(true);
            arm2.gameObject.SetActive(true);
            MoveArms();
            firstboost = true;
        }
        else
        {
            arm1.gameObject.SetActive(false);
            arm2.gameObject.SetActive(false);
        }
        if (armMoveCount % 3 == 0 && firstboost)
        {
            rb.velocity = Random.onUnitSphere * 30;
            firstboost = false;
            armMoveCount++;
        }
    }

    private void MoveArms()
    {
        armAngle = spinPoint2.rotation.eulerAngles.z;
        if (armAngle >= 270)
        {
            armMoveCount++;
            goingDown = true;
        }
        else if (armAngle <= 90)
        {
            goingDown = false;
        }
        if (goingDown)
        {
            spinPoint1.Rotate(new Vector3(0, 0, .5f));
            spinPoint2.Rotate(new Vector3(0, 0, -.5f));
        }
        else
        {
            spinPoint1.Rotate(new Vector3(0, 0, -.5f));
            spinPoint2.Rotate(new Vector3(0, 0, .5f));
        }
    }

    [PunRPC]
    void RPC_SpawnMinion()
    {
        if (PV.IsMine)
        {
            var minion = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "Enemy2"),
            spawnPoint.transform.position + new Vector3(2, 0, 0), Quaternion.Euler(0, 0, 0), 0);Debug.Log(minion.GetComponent<EnemyAI>() + "FFFFF");
            minion.GetComponent<EnemyAI>().SetParent(this.transform);
            //Physics2D.IgnoreCollision(minion.GetComponent<Collider2D>(), arm1.GetComponent<Collider2D>(), true);
            //Physics2D.IgnoreCollision(minion.GetComponent<Collider2D>(), arm2.GetComponent<Collider2D>(), true);
            //Physics2D.IgnoreCollision(minion.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        }
    }

    [PunRPC]
    void RPC_FireProjectile()
    {
        var bulletRef = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "BossProjectile"),
            firePoint.transform.position, Quaternion.Euler(0, 0, angle - 90), 0);
        bulletRef.GetComponent<EnemyBullet>().SetDamageAndOwner(damage, this.transform);
        Physics2D.IgnoreCollision(bulletRef.GetComponent<Collider2D>(), arm1.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(bulletRef.GetComponent<Collider2D>(), arm2.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(bulletRef.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
    }
}
