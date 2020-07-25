using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyEnemyBarrier : MonoBehaviour
{
    public int userLife = 4;
    private PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Thunder") || other.gameObject.tag.Equals("Fire") || other.gameObject.tag.Equals("Earth"))
        {
            PhotonNetwork.Destroy(other.gameObject);
            PV.RPC("RPC_UpdateLifeText", RpcTarget.All);
        }

        
    }

    [PunRPC]
    protected void RPC_UpdateLifeText()
    {
        GameObject textField = GameObject.Find("LifeText");
        Text text = textField.GetComponent<Text>();
        GameSetup.GS.usersLife -= 1;
        text.text = "Vida: " + GameSetup.GS.usersLife.ToString();
        if (GameSetup.GS.usersLife <= 0)
        {
            if(GameSetup.GS.player1Score == 0)
            {
                PlayerPrefs.SetString("score1", "0");
            }else
            {
                PlayerPrefs.SetString("score1", GameSetup.GS.player1Score.ToString());
            }

            if (GameSetup.GS.player2Score == 0)
            {
                PlayerPrefs.SetString("score2", "0");
            }
            else
            {
                PlayerPrefs.SetString("score2", GameSetup.GS.player2Score.ToString());
            }
            if(PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(2);
        }
    }
}
