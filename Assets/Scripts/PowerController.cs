using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PowerController : MonoBehaviour
{
    public bool playerOwner;
    // Start is called before the first frame update

    private void Awake()
    {
        playerOwner = false;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}
