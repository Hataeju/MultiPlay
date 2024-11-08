using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VideoChatArea : MonoBehaviour
{
    // 채널에 접속하기 위해서 채널 이름
    private string channelName = "10";
    // 채널 이름이 동일하다면 함께 화상채팅 가능
    // 채널 이름이 다르다면 다른 공간에 존재하는 것
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PhotonView>().IsMine)
                VideoChatManager.Instance.Join(channelName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PhotonView>().IsMine)
                VideoChatManager.Instance.Leave();
        }
    }
}
