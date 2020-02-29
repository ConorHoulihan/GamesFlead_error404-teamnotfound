using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRanged : MonoBehaviour
{
    private Transform closestPlayer;
    private PhotonView PV;
    private bool hasSpawned = false;
    public float damage;

    private void Start()
    {
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
        if (Vector2.Distance(closestPlayer.position, transform.position) < 20 && !hasSpawned && PV.IsMine)
        {
            hasSpawned = true;
            PV.RPC("RPC_SpawnMinions", RpcTarget.AllViaServer);
        }
    }
    [PunRPC]
    void RPC_SpawnMinions()
    {
        if (PV.IsMine)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                var minion = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "Enemy2"),
                    child.transform.position, Quaternion.Euler(0, 0, 0), 0);
                minion.GetComponent<RangedEnemyMovement>().SetParent(child.transform);
            }
        }
    }
}


