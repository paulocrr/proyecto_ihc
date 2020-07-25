using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject player1Score = GameObject.Find("ScorePlayer1");
        GameObject player2Score = GameObject.Find("ScorePlayer2");

        player1Score.GetComponent<Text>().text = PlayerPrefs.GetString("score1");
        player2Score.GetComponent<Text>().text = PlayerPrefs.GetString("score2");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
