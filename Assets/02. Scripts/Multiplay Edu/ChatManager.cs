using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class ChatManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI chatObjPreafap; // ä�� ������Ʈ ������
    [SerializeField] private ScrollRect scrollView;            // ���ڷѺ�
    [SerializeField] private Transform content; // ä�� ������Ʈ�� ������ �θ�
    [SerializeField] private TMP_InputField tmpInputMessage; // �Է��� �޽���

    // ��Ӿ� ������ ���� ��ųʸ�
    private Dictionary<string, string> changeString = new Dictionary<string, string>();

    private void Start()
    {
        SetBadWords();


        // onSubmit : ��Ǭ �ʵ忡 �ؽ�Ʈ�� �ۼ��ϰ� �Ϸ��� �� ȣ���
        // ����Ű�� ������ ��, ���� ����Ͽ����� ä�ù�ư�� ������ ��
        tmpInputMessage.onSubmit.AddListener(GetMessage);
    }

    // Update is called once per frame
    void Update()
    {
        //ChatOnPC();
    }

    private void SetBadWords()
    {
        // key : ��Ӿ� (�����ؾ� �� �ܾ�)
        // value : �ٲ� �ܾ�
        changeString.Add("�ٺ�", "���Ѿ���");
        changeString.Add("��û��", "���� �Ϸ� �Ǽ���");
        changeString.Add("�˰�", "�ٸ���, ��");

    }

    // �޽��� ���͸�
    private string MessageFilter(string message)
    {
        foreach(KeyValuePair<string, string> item in changeString)
        {
            // Ű, �� �� ���� ��ųʸ��� Ȱ���Ͽ� Ű�� ���� �ٲ���
            message = message.Replace(item.Key, item.Value);
        }

        return message;
    }

    // �޽��� ������ �� ���͸� ���ļ� ä��
    private void GetMessage(string message)
    {
        string temp = MessageFilter(message);
        SendChat(temp);
        tmpInputMessage.text = string.Empty;
    }
    private void ChatOnPC()
    {
        /*// ����Ű�� ������ ��
        if(Input.GetKeyDown(KeyCode.Return))
        {
            
            SendChat(tmpInputMessage.text);
            // ���� Ű ���� �޽��� ��� �Ŀ��� ��ǲ�ʵ带 �����ش�.
            tmpInputMessage.text = string.Empty;
        }*/
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (tmpInputMessage.isFocused)
            {
                // �Է� �ʵ忡 ��Ŀ���� ���� ���� �޽��� ����
                SendChat(tmpInputMessage.text);
                tmpInputMessage.text = string.Empty; // �޽��� ���� �� �Է� �ʵ� ���
                
            }
            else
            {
                tmpInputMessage.Select();
                tmpInputMessage.ActivateInputField();
            }
        }
    }

    // ä�� �Լ�
    public void SendChat(string inputMSG)
    {
        // ���� �޽����� ���ٸ� ����
        if (inputMSG == string.Empty) return;

        // �г��Ӱ� �޽��� ������ ���� ��ųʸ�
        Dictionary<string, string> sendMsg = new Dictionary<string, string>(0);

        // �ӽ÷� �г����� test�� ����
        string nick = PhotonManager.Instance.player.GetComponent<PhotonView>().Owner.NickName;

        // �г��Ӱ� �޽����� ��ǳʸ��� �߰�
        sendMsg.Add("Nick", nick);
        sendMsg.Add("Message", inputMSG + "\n");

        //BroadCast(sendMsg);
        // ������� RPC�� ��� �ο����� ��ε�ĳ����
        // RPC("�Լ���", Ÿ��, �Ű�����)
        photonView.RPC("BroadCast", RpcTarget.All, sendMsg);
    }

    // ��� �Լ�
    [PunRPC]
    private void BroadCast(Dictionary<string, string> message)
    {
        // ä�� ������Ʈ�� ���� �� �θ� ����
        GameObject chat = Instantiate(chatObjPreafap.gameObject, content);
        TextMeshProUGUI tmpChat = chat.GetComponent<TextMeshProUGUI>();

        // �޽���
        DateTime now = DateTime.Now;
        int hour = now.Hour;
        int minute = now.Minute;

        string time = $"{hour:D2}:{minute:D2}";

        tmpChat.text = " [" + time + "] [" + message["Nick"] + "] : " + message["Message"] + "\n";

        // ���� �ֱ� �޽����� ����ǵ��� ������ ���� ������Ʈ
        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
    }
}
