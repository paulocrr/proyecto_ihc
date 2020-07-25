using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerActivation : MonoBehaviour
{
    public bool trackStatus;
    private PhotonView PV;
    Text timeLeft;
    // Start is called before the first frame update
    void Start()
    {
        timeLeft = GameObject.Find("TimerText").GetComponent<Text>();
        PlayerPrefs.SetString("score1", GameSetup.GS.player1Score.ToString());
        PlayerPrefs.SetString("score2", GameSetup.GS.player2Score.ToString());
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        PV.RPC("RPC_changeTime", RpcTarget.All);
    }

    public void setActiveChilds()
    {
        trackStatus = true;
        GameObject g = GameObject.Find("ImageTarget");
        int count = g.transform.childCount;
        Debug.Log("Tracked!!!!");
        for (int i = 0; i < count; i++)
        {
            Transform child = g.transform.GetChild(i);
            child.gameObject.SetActive(true);
        }
    }

    public void disableChilds()
    {
        trackStatus = false;
        GameObject g = GameObject.Find("ImageTarget");
        int count = g.transform.childCount;
        Debug.Log("UnTracked!!!!");
        for (int i = 0; i < count; i++)
        {
            Transform child = g.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
    }
    
    [PunRPC]
    protected void RPC_changeTime()
    {
        GameSetup.GS.timeLeft -= Time.deltaTime;
        int left = (int)GameSetup.GS.timeLeft;
        timeLeft.text = "Tiempo Restante: " + left.ToString();
        if (GameSetup.GS.timeLeft < 0)
        {
            if (GameSetup.GS.player1Score == 0)
            {
                PlayerPrefs.SetString("score1", "0");
            }
            else
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
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(2);
        }
    }
}
