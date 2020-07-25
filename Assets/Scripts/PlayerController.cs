using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Windows.Speech;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Data;

public class PlayerController : MonoBehaviourPun
{
    private GameObject targetParent;
    private PhotonView PV;
    public bool hasWater = false;
    public bool hasFire = false;
    public bool hasEarth = false;
    public int currentPower;

    [SerializeField]
    private string[] m_Keywords;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, int> actions = new Dictionary<string, int>();
    public ConfidenceLevel confidence = ConfidenceLevel.Low;
    public int userLife;
    Material redShield;
    Material yellowShield;


    // Start is called before the first frame update
    void Start()
    {
        redShield = Resources.Load<Material>("ForceShieldMatRed");
        yellowShield = Resources.Load<Material>("ForceShieldMatYellow");
        userLife = GameSetup.GS.usersLife;
        actions.Add("disparar", 0);
        List<string> acc = actions.Keys.ToList();
        keywordRecognizer = new KeywordRecognizer(m_Keywords, confidence);
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void Awake()
    {

        PV = GetComponent<PhotonView>();
        this.gameObject.SetActive(false);
        targetParent = GameObject.Find("ImageTarget");
        transform.SetParent(targetParent.transform);


        if (PhotonNetwork.IsMasterClient)
        {
            
           // actions.Add("disparar uno", 0);
            if (PlayerPrefs.GetInt("Water") == 0  )
            {
                hasWater = true;
                Image label = GameObject.Find("WaterLabel").GetComponent<Image>();
                label.enabled = true;
            }
            if (PlayerPrefs.GetInt("Fire") == 0)
            {
                hasFire = true;
                Image label = GameObject.Find("FireLabel").GetComponent<Image>();
                label.enabled = true;
            }
            if (PlayerPrefs.GetInt("Earth") == 0)
            {
                hasEarth = true;
                Image label = GameObject.Find("EarthLabel").GetComponent<Image>();
                label.enabled = true;
            }

        }
        else
        {
          //  actions.Add("disparar dos", 1);
            if (PlayerPrefs.GetInt("Water") == 1)
            {
                hasWater = true;
                Image label = GameObject.Find("WaterLabel").GetComponent<Image>();
                label.enabled = true;
            }
            if (PlayerPrefs.GetInt("Fire") == 1)
            {
                hasFire = true;
                Image label = GameObject.Find("FireLabel").GetComponent<Image>();
                label.enabled = true;
            }
            if (PlayerPrefs.GetInt("Earth") == 1)
            {
                hasEarth = true;
                Image label = GameObject.Find("EarthLabel").GetComponent<Image>();
                label.enabled = true;
            }

            
        }
      /*  List<string> acc = actions.Keys.ToList();
        keywordRecognizer = new KeywordRecognizer(acc.ToArray(), confidence);
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
        */

        if (PV.IsMine)
        {

            if (hasWater)
            {
                
                PV.RPC("selectPower", RpcTarget.AllBufferedViaServer, 0, PV.ViewID);
            }
            else if (hasFire)
            {
               
                PV.RPC("selectPower", RpcTarget.AllBufferedViaServer, 1, PV.ViewID);
            }
            else if (hasEarth)
            {
               
                PV.RPC("selectPower", RpcTarget.AllBufferedViaServer, 2, PV.ViewID);
            }
        }


    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
  

    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.W) && hasWater)
            {

                changePower(0);
            }
            if (Input.GetKeyDown(KeyCode.F) && hasFire)
            {

                changePower(1);
            }
            if (Input.GetKeyDown(KeyCode.R) && hasEarth)
            {

                changePower(2);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                shootPower();
            }

            if (Input.GetKey(KeyCode.Y))
            {
                PV.RPC("moveBackward", RpcTarget.All, PV.ViewID);
            }

            if (Input.GetKey(KeyCode.H))
            {
                PV.RPC("moveFordward", RpcTarget.All, PV.ViewID);
            }
          

            if(GameSetup.GS.usersLife != userLife)
            {
                Debug.Log(GameSetup.GS.usersLife);
                PV.RPC("updateUserLife", RpcTarget.All, PV.ViewID);
            }
        }
        
    }

    public void shootPower()
    {

        if (PV.IsMine)
        {
            GameObject powerShooted;
            if (currentPower == 1)
            {
                powerShooted = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PowerFire"),
                                                transform.position, Quaternion.identity, 0);
            }
            else if (currentPower == 0)
            {
                powerShooted = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PowerWater"),
                                                transform.position, Quaternion.identity, 0);
            }
            else
            {
                powerShooted = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PowerEarth"),
                                                transform.position, Quaternion.identity, 0);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                powerShooted.GetComponent<PowerController>().playerOwner = true;
            }
        }

       
    }

    public void changePower(int powerIndex)
    {
        if (PV.IsMine)
        {
            if (powerIndex == 0 && hasWater)
            {
                //0 Agua
                PV.RPC("selectPower", RpcTarget.AllBufferedViaServer, 0, PV.ViewID);
            }
            if (powerIndex == 1 && hasFire)
            {
                //1 Fire
                PV.RPC("selectPower", RpcTarget.AllBufferedViaServer, 1, PV.ViewID);
            }
            if (powerIndex == 2 && hasEarth)
            {
                //2 Earth
                PV.RPC("selectPower", RpcTarget.AllBufferedViaServer, 2, PV.ViewID);
            }
        }
        
    }

    [PunRPC]
    protected void selectPower(int powerIndex,int viewId)
    {
        currentPower = powerIndex;
        GameObject playerObject = PhotonView.Find(viewId).gameObject;
        Debug.Log(viewId);
        
        Transform child = playerObject.transform.GetChild(powerIndex);
        child.gameObject.SetActive(true);
        int count_childs = playerObject.transform.childCount;
        for (int i = 0; i < count_childs; i++)
        {
            if(i!= powerIndex && i != 3 && i !=4 )
            {
                Transform otherPowers = playerObject.transform.GetChild(i);
                otherPowers.gameObject.SetActive(false);
            }
            
        }
        
        
    }

    [PunRPC]
    protected void updateUserLife(int viewId)
    {
        userLife = GameSetup.GS.usersLife;
        GameObject avatar = PhotonView.Find(viewId).gameObject;
        GameObject shield = avatar.transform.GetChild(4).gameObject;
        if (GameSetup.GS.usersLife == 3)
        {
            Debug.Log("Amarillo");
            shield.GetComponent<Renderer>().material = yellowShield;
        }
        else if (GameSetup.GS.usersLife == 2)
        {
            Debug.Log("Rojo");
            shield.GetComponent<Renderer>().material = redShield;
        }
    }
    [PunRPC]
    protected void moveBackward(int viewId)
    {
        GameObject avatar = PhotonView.Find(viewId).gameObject;
        avatar.transform.Translate(Vector3.back * Time.deltaTime);
        
    }

    [PunRPC]
    protected void moveFordward(int viewId)
    {
        GameObject avatar = PhotonView.Find(viewId).gameObject;
        avatar.transform.Translate(Vector3.forward * Time.deltaTime);
        
    }

    void OnDestroy()
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }



}
