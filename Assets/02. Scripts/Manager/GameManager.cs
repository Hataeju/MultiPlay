using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }
        

    [Header("컨트롤러")]
    GameObject player;    
    Animator ani;
    public NavMeshAgent agent;
    bool isRunning;
    float originSpeed;   


    void Awake()
    {
        if (instance == null)
            instance = this;

        //SetPlayer();
    }

    void Start()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE || UNITY_EDITOR_OSX
        // 포톤 매니저에서 플레이어가 생성되었다면 SetPlayer를 호출
        PhotonManager.Instance.PlayerCreated += SetPlayer;


#elif UNITY_ANDROID
        battery.SetActive(true);
        StartCoroutine(CheckBattery(1));
#endif
    }    

    void Update()
    {
        
    }

    void SetPlayer()
    {
        player = PhotonManager.Instance.player;
        
        ani = player.GetComponent<Animator>();
        agent = player.GetComponent<NavMeshAgent>();
        originSpeed = agent.speed;       
        
    }
        
    public void SetAni()
    {
        ani.SetInteger("PlayerState", isRunning ? (int)PlayerState.Run : (int)PlayerState.Run);
        
    }
    
    public void SetIdle()
    {
        ani.SetInteger("PlayerState", (int)PlayerState.Idle);
        
    }
}
