using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI가 항상 카메라를 보도록 함.
/// </summary>

public class LookAtCameraUI : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
