using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;

/// <summary>
/// UI 매니저 (월드 씬)
/// </summary>

public class UIManagerWorld : MonoBehaviour
{
    static UIManagerWorld instance = null;
    public static UIManagerWorld Instance { get { return instance; } }

    [Header("컨트롤러")]    
    public Transform cameraArm;
    public CameraZoomming cameraZoom;


    [Header("목표 지점")]
    public GameObject destinationPrefab;
    public float destinationPosY;

        

    [Header("캔버스")]
    public RectTransform canvas;

    // 플레이어 이름
    public GameObject playerName;

    void Awake()
    {
        if(instance == null) { instance = this; }
    }

    void Start()
    {
        
    }
}
