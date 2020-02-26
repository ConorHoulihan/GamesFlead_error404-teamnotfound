using Photon.Pun;
using UnityEngine;

public class RangedEnemyMovement : MonoBehaviour
{
    private Transform closestPlayer;
    private Rigidbody2D rigidbody2d;
    public GameObject middlePoint, firePoint;
    private PhotonView PV;
    private float fireRate = 2f, FireTime = -1f, angle;
    public float damage = 20;
    public GameObject prefab;

    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))//Should set closest player as owner
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
        float dist = Vector2.Distance(closestPlayer.transform.position, transform.position);

        if (dist > 6f && dist > 20)
        {
            rigidbody2d.velocity = (closestPlayer.transform.position - transform.position).normalized * 5;
        }
        else if (dist < 5f)
        {
            rigidbody2d.velocity = -(closestPlayer.transform.position - transform.position).normalized * 5;
        }
        else
        {
            rigidbody2d.velocity = new Vector2(0, 0);
            if (PhotonNetwork.IsMasterClient)
            {
                if (FireTime - Time.time < 0)
                {
                    FireTime = Time.time + fireRate;
                    PV.RPC("RPC_FireProjectile", RpcTarget.AllViaServer);
                }
            }
        }

        if (closestPlayer.position != transform.position)
        {
            Vector3 lookPos = closestPlayer.position - middlePoint.transform.position;
            angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            middlePoint.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    [PunRPC]
    void RPC_FireProjectile()
    {
        var bulletRef = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "EnemyProjectile"),
        firePoint.transform.position, Quaternion.Euler(0, 0, angle - 90), 0);
        bulletRef.GetComponent<EnemyBullet>().SetDamage(damage);
    }
}
