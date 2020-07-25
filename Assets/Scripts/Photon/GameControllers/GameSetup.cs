using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;

    public Transform[] spawnPoints;
    public int globalScore = 0;
    public int usersLife = 4;
    public float timeLeft = 120f;
    public int player1Score = 0;
    public int player2Score = 0;

    private void OnEnable()
    {
        if(GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }
}
