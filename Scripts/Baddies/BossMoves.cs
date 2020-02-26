using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMoves : MonoBehaviour
{
    private PhotonView PV;
    public Transform spawnPoint;
    public float spawnTime = -1f, spawnDelay = 1f;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if ((GameObject.FindGameObjectsWithTag("Baddie1").Length < 4) && (Time.time > spawnTime))
        {
            spawnTime = Time.time + spawnDelay;
            PV.RPC("RPC_FireProjectile", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    void RPC_FireProjectile()
    {
        var minion = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "Enemy"),
        spawnPoint.transform.position, Quaternion.Euler(0, 0, 0), 0);
        minion.GetComponent<EnemyAI>().SetParent(this.transform);
    }
}
