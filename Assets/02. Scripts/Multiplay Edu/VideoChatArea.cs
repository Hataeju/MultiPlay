using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VideoChatArea : MonoBehaviour
{
    // ä�ο� �����ϱ� ���ؼ� ä�� �̸�
    private string channelName = "10";
    // ä�� �̸��� �����ϴٸ� �Բ� ȭ��ä�� ����
    // ä�� �̸��� �ٸ��ٸ� �ٸ� ������ �����ϴ� ��
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
