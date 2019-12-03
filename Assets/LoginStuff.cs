using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginStuff : MonoBehaviour
{
    public string clientID;
    public TMPro.TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetClientID(string clientID) {
        this.clientID = clientID;
        text.text = clientID;
    }
}
