using UnityEngine;
using UnityEngine.SceneManagement;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;
using System.Data;
using System.Net;
using TMPro;

public class LoginManager : MonoBehaviour
{

    [Header("Login Input")]
    public TMP_InputField Username;
    public TMP_InputField Password;

    public TextMeshProUGUI ErrorMessage;

    public GameObject Register;

    public String UserCredit;

    public DatabaseHandler DatabaseHandler;

    
    public void CheckCredentials()
    {
        ErrorMessage.gameObject.SetActive(false);

        var username = Username.text;
        var password = Password.text;        
        UserCredit = "User Data";
        SceneManager.LoadScene("MainMenu");
    } 

    private void ErrorOnLogin()
    {
        ErrorMessage.text = "Couldn't Login, try again";
        ErrorMessage.gameObject.SetActive(true);

    }
}
