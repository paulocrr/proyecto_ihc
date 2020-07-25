using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;
    private GameObject marker;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        marker = GameObject.Find("ImageTarget");

        if (PV.IsMine)
        {
            
            if (PhotonNetwork.IsMasterClient)
            {
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatarMaster"),
                           GameSetup.GS.spawnPoints[0].position, Quaternion.identity, 0);
            }
            else
            {
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatarClient"),
                           GameSetup.GS.spawnPoints[1].position, GameSetup.GS.spawnPoints[1].rotation, 0);
            }

            //myAvatar.SetActive(false);
            
            //myAvatar.transform.SetParent(marker.gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
