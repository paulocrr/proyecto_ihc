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
public class VozController : MonoBehaviour
{
    private PhotonView PV;
    private GameObject targetParent;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, int> actions = new Dictionary<string, int>();
    public ConfidenceLevel confidence = ConfidenceLevel.Low;
    private bool shoot = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {

        PV = GetComponent<PhotonView>();
        this.gameObject.SetActive(false);
        targetParent = GameObject.Find("ImageTarget");
        transform.SetParent(targetParent.transform);


        if (PhotonNetwork.IsMasterClient)
        {

        }
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        if (speech.text.Equals("disparar"))
        {
            Debug.Log("Disparando");
        }
        else if (speech.text.Equals("agua"))
        {
            Debug.Log("Cambio");
        }
        else if (speech.text.Equals("tierra"))
        {
            Debug.Log("Cambio");
        }
        else if (speech.text.Equals("fuego"))
        {
            Debug.Log("Cambio");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
