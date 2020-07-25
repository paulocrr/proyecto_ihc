using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private PhotonView PV;
    private GameObject targetParent;
    public GameObject selfAvatar;
    private GameObject walkPoint;
    public GameObject sonido_golpe;
    public GameObject sonido_muerte;
    public GameObject sonido_risa;


    public string walkPointId = "EnemyWalk (1)";
    private Animator animator;
    private int life;
    private Vector3 _direction;
    private Quaternion _lookRotation;

    
    private void Awake()
    {
        this.gameObject.SetActive(false);
        targetParent = GameObject.Find("ImageTarget");
        transform.SetParent(targetParent.transform);
        this.gameObject.SetActive(true);
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if (life == 4)
        {
            GameObject lifeBar = this.transform.GetChild(1).gameObject;
            lifeBar.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
            
        walkPoint = GameObject.Find(walkPointId);
        PV = GetComponent<PhotonView>();
        
        Transform[] t = this.GetComponentsInChildren<Transform>();
        selfAvatar = t[1].gameObject;
        animator = selfAvatar.GetComponent<Animator>();
        animator.SetInteger("battle", 1);
        animator.SetInteger("moving", 2);
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = _lookRotation;
        transform.rotation = transform.rotation * Quaternion.Euler(90, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            _direction = (walkPoint.transform.position - transform.position).normalized;

            //create the rotation we need to be in to look at the target
            
            transform.position = Vector3.MoveTowards(transform.position, walkPoint.transform.position, Time.deltaTime * 0.5f);
        }
        
    }

    void OnTriggerEnter(Collider colider)
    {
        Debug.Log(colider.gameObject.tag);
        if ((this.tag.Equals("Fire") && colider.gameObject.tag.Equals("PowerWater")) || (this.tag.Equals("Thunder") && colider.gameObject.tag.Equals("PowerEarth")) || (this.tag.Equals("Earth") && colider.gameObject.tag.Equals("PowerFire")))
        {
            life--;
            PV.RPC("RPC_UpdateReportScore", RpcTarget.All, colider.gameObject.GetComponent<PowerController>().playerOwner);
            GameObject lifeBar = this.transform.GetChild(1).gameObject;
            //lifeBar.transform.localScale = new Vector3(lifeBar.transform.localScale.x-0.4f, lifeBar.transform.localScale.y, lifeBar.transform.localScale.z);
            if(life == 3)
            {
                lifeBar.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                //Instantiate(sonido_golpe);
            }
            else if (life == 2)
            {
                //Instantiate(sonido_golpe);
                lifeBar.transform.localScale = new Vector3(lifeBar.transform.localScale.x - 0.4f, lifeBar.transform.localScale.y, lifeBar.transform.localScale.z);
                lifeBar.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            }else if(life == 1)
            {
                //Instantiate(sonido_golpe);
                lifeBar.transform.localScale = new Vector3(lifeBar.transform.localScale.x - 0.4f, lifeBar.transform.localScale.y, lifeBar.transform.localScale.z);
                lifeBar.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
            StartCoroutine(playHitAnimation());
         
            PhotonNetwork.Destroy(colider.gameObject);
        }
        else if(colider.gameObject.name.Equals("EnemyLimitBarrier"))
        {
            StartCoroutine(playDeffendAnimation());
        }

        


        if (life == 0)
        {
            //Instantiate(sonido_muerte);
            PV.RPC("RPC_UpdateScoreText", RpcTarget.All);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    IEnumerator playHitAnimation()
    {
        animator.SetInteger("moving", 11);
        yield return new WaitForSeconds(0.1f);
        animator.SetInteger("battle", 1);
        animator.SetInteger("moving", 2);

    }

    IEnumerator playDeffendAnimation()
    {
        animator.SetInteger("moving", 10);

        yield return new WaitForSeconds(0.1f);
        animator.SetInteger("battle", 1);
        animator.SetInteger("moving", 2);

        // animator.SetInteger("moving", 2);


    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        this.walkPointId = (string)data[0];
        this.tag = (string)data[1];
        this.life = int.Parse((string)data[2]);
        
    }

    [PunRPC]
    protected void RPC_UpdateScoreText()
    {
        GameObject textField = GameObject.Find("ScoreText");
        Text text = textField.GetComponent<Text>();
        GameSetup.GS.globalScore += 10;
        if (GameSetup.GS.globalScore >= 200)
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

        Debug.Log("Puntaje: " + GameSetup.GS.globalScore.ToString());
        
        text.text = "Puntaje: " + GameSetup.GS.globalScore.ToString();
    }

    [PunRPC]
    protected void RPC_UpdateReportScore(bool owner)
    {
        if (owner)
        {
            GameSetup.GS.player1Score += 10;
        }
        else
        {
            GameSetup.GS.player2Score += 10;
        }
    }
}
