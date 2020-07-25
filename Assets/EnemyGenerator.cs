using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public ArrayList enemiesGenerated;
    public GameObject lastEnemyGenerated;
    public int spanPointIndex = 2;
    public string walkPoint = "EnemyWalk (1)";
    private PhotonView PV;
   
    // Start is called before the first frame update
    private void Awake()
    {
        enemiesGenerated = new ArrayList();
    }
    void Start()
    {
        PV = GetComponent<PhotonView>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGenerateEnemies()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("GenerateEnemy", 2.0f, 5.5f);
        }
        
    }

    public void GenerateEnemy()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            object[] data = new object[3];
            data[0] = walkPoint;
            int randomEnemy = Random.Range(0, 3);
            spanPointIndex = Random.Range(2, 4);

            if(spanPointIndex == 2)
            {
                walkPoint = "EnemyWalk (1)";
            }else if(spanPointIndex == 3)
            {
                walkPoint = "EnemyWalk (2)";
            }

            data[2] = "3";
            if (GameSetup.GS.timeLeft < 50)
            {
                data[2] = "4";
            }

            if (randomEnemy == 0)
            {
                data[1] = "Fire";
               
                lastEnemyGenerated = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "EnemyFireAvatar"),
                                            GameSetup.GS.spawnPoints[spanPointIndex].position, Quaternion.identity, 0, data);
            }else if(randomEnemy == 1)
            {
                data[1] = "Thunder";
                lastEnemyGenerated = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "EnemyThunderAvatar"),
                                            GameSetup.GS.spawnPoints[spanPointIndex].position, Quaternion.identity, 0, data);
            }
            else
            {
                data[1] = "Earth";
                lastEnemyGenerated = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "EnemyEarthAvatar"),
                                            GameSetup.GS.spawnPoints[spanPointIndex].position, Quaternion.identity, 0, data);
            }
            
            this.enemiesGenerated.Add(lastEnemyGenerated);
        }
        
        
       
        
    }

    public void DestroyAllEnemiesGenerated()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CancelInvoke("GenerateEnemy");
            foreach (GameObject enemyGenerated in enemiesGenerated)
            {
                PhotonNetwork.Destroy(enemyGenerated.gameObject);
            }

            if (this.enemiesGenerated.Count > 0)
            {
                this.enemiesGenerated.Clear();
            }
        }

        

    }

}
