using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;
    public GameObject[] disableTag;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        int i = 0; ;
        for (i = 0; i > GameSetup.GS.spawnPoints.Length; i++)
        {
            if (!GameSetup.GS.getTaken(i))
            {
                GameSetup.GS.SetTaken(i);
                break;
            }
        }
        if (PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
            GameSetup.GS.spawnPoints[i].position,GameSetup.GS.spawnPoints[i].rotation, 0);
            myAvatar.GetComponent<PlayerHPXP>().SetSpawnPos(i);
        }
    }
}
