using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField tmpInput;
    [SerializeField] private Button buttonLogin;

    // Start is called before the first frame update
    void Start()
    {
        buttonLogin.onClick.AddListener(() =>
        {
            Config.userNickName = tmpInput.text;
            Debug.Log("´Ð³×ÀÓ : " +Config.userNickName);
            PhotonManager.Instance.ConnectToPhoton();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
