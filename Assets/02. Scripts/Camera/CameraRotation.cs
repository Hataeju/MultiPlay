using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플랫폼에 따른 카메라 회전
/// </summary>

public class CameraRotation : MonoBehaviour
{    
    float xRotate, yRotate, xRotateMove, yRotateMove;

    const float pcRotateSpeed = 300.0f;
    const float mobileRotateSpeedOneFinger = 70.0f;
    const float mobileRotateSpeedTwoFinger = 10.0f;
    const float Limit_High = 85f;
    const float Limit_Low = -30f;

    

    void LateUpdate()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE || UNITY_EDITOR_OSX
        if (Input.GetMouseButton(1))
        {
            xRotateMove = -Input.GetAxis("Mouse Y") * Time.deltaTime * pcRotateSpeed;
            yRotateMove = Input.GetAxis("Mouse X") * Time.deltaTime * pcRotateSpeed;

            yRotate = transform.eulerAngles.y + yRotateMove;
            xRotate = xRotate + xRotateMove;
            xRotate = Mathf.Clamp(xRotate, Limit_Low, Limit_High); // 상하 각도 제한

            transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
        }

#elif UNITY_ANDROID || UNITY_IOS        
#endif
    }
}
