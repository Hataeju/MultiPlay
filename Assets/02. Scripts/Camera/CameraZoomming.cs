using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플랫폼에 따른 카메라 줌
/// </summary>

public class CameraZoomming : MonoBehaviour
{        
    CinemachineVirtualCamera virtualCamera;
    Cinemachine3rdPersonFollow followCamera;
        
    float wheel;
    float wheelSpeed = 5f;

    const float MIN_ZOOM = -6f;
    const float MAX_ZOOM = -0.6f;

    public bool isZoomming;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        
        followCamera = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    void Update()
    {
        ZoomInput();
    }

    void LateUpdate()
    {        
#if UNITY_EDITOR_WIN || UNITY_STANDALONE || UNITY_EDITOR_OSX
        Zoom();

#elif UNITY_ANDROID || UNITY_IOS // 스위칭한 플랫폼에 따라 줌 방식 결정 
            
#endif
    }

    void ZoomInput()
    {        
        wheel += Input.GetAxis("Mouse ScrollWheel");
    }

    void Zoom()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (wheel != 0)
        {        
            wheel = Mathf.Clamp(wheel, MIN_ZOOM, MAX_ZOOM);
            followCamera.CameraDistance = -wheel * wheelSpeed;
        }
    }
}
