using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 생성 시 카메라암이 플레이어를 따라가도록 함.
/// </summary>

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    Vector3 offset = new Vector3(0, 0.5f, 0);
    float followDamping = 0.5f;

    void Awake()
    {
        //player = GameObject.Find("Player").transform;
        PhotonManager.Instance.PlayerCreated += SetPlayer;
    }

    void SetPlayer()
    {
        player = PhotonManager.Instance.player.transform;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 pos = transform.position + offset;
        transform.position = Vector3.Lerp(pos, player.position, followDamping);
    }
}
