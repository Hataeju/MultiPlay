using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class ChatManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI chatObjPreafap; // 채팅 오브젝트 프리펩
    [SerializeField] private ScrollRect scrollView;            // 스코롤뷰
    [SerializeField] private Transform content; // 채팅 오브젝트가 생성될 부모
    [SerializeField] private TMP_InputField tmpInputMessage; // 입력할 메시지

    // 비속어 관리를 위한 딕셔너리
    private Dictionary<string, string> changeString = new Dictionary<string, string>();

    private void Start()
    {
        SetBadWords();


        // onSubmit : 인푼 필드에 텍스트를 작성하고 완료할 때 호출됨
        // 엔터키를 눌렀을 때, 도는 모바일에서는 채팅버튼을 눌렀을 때
        tmpInputMessage.onSubmit.AddListener(GetMessage);
    }

    // Update is called once per frame
    void Update()
    {
        //ChatOnPC();
    }

    private void SetBadWords()
    {
        // key : 비속어 (수정해야 할 단어)
        // value : 바뀔 단어
        changeString.Add("바보", "착한아이");
        changeString.Add("멍청이", "좋은 하루 되세요");
        changeString.Add("똥개", "바른말, 고운말");

    }

    // 메시지 필터링
    private string MessageFilter(string message)
    {
        foreach(KeyValuePair<string, string> item in changeString)
        {
            // 키, 값 한 쌍인 딕셔너리를 활용하여 키와 값을 바꿔줌
            message = message.Replace(item.Key, item.Value);
        }

        return message;
    }

    // 메시지 가져온 뒤 필터링 거쳐서 채팅
    private void GetMessage(string message)
    {
        string temp = MessageFilter(message);
        SendChat(temp);
        tmpInputMessage.text = string.Empty;
    }
    private void ChatOnPC()
    {
        /*// 엔터키를 눌렀을 때
        if(Input.GetKeyDown(KeyCode.Return))
        {
            
            SendChat(tmpInputMessage.text);
            // 엔터 키 눌러 메시지 출력 후에는 인풋필드를 지워준다.
            tmpInputMessage.text = string.Empty;
        }*/
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (tmpInputMessage.isFocused)
            {
                // 입력 필드에 포커스가 있을 때만 메시지 전송
                SendChat(tmpInputMessage.text);
                tmpInputMessage.text = string.Empty; // 메시지 전송 후 입력 필드 비움
                
            }
            else
            {
                tmpInputMessage.Select();
                tmpInputMessage.ActivateInputField();
            }
        }
    }

    // 채팅 함수
    public void SendChat(string inputMSG)
    {
        // 만약 메시지가 없다면 리턴
        if (inputMSG == string.Empty) return;

        // 닉네임과 메시지 구분을 위한 딕셔너리
        Dictionary<string, string> sendMsg = new Dictionary<string, string>(0);

        // 임시로 닉네임을 test로 지정
        string nick = PhotonManager.Instance.player.GetComponent<PhotonView>().Owner.NickName;

        // 닉네임과 메시지를 딕션너리에 추가
        sendMsg.Add("Nick", nick);
        sendMsg.Add("Message", inputMSG + "\n");

        //BroadCast(sendMsg);
        // 포톤뷰의 RPC로 모든 인원에게 브로드캐스팅
        // RPC("함수명", 타겟, 매개변수)
        photonView.RPC("BroadCast", RpcTarget.All, sendMsg);
    }

    // 방송 함수
    [PunRPC]
    private void BroadCast(Dictionary<string, string> message)
    {
        // 채팅 오브젝트를 생성 및 부모 지정
        GameObject chat = Instantiate(chatObjPreafap.gameObject, content);
        TextMeshProUGUI tmpChat = chat.GetComponent<TextMeshProUGUI>();

        // 메시지
        DateTime now = DateTime.Now;
        int hour = now.Hour;
        int minute = now.Minute;

        string time = $"{hour:D2}:{minute:D2}";

        tmpChat.text = " [" + time + "] [" + message["Nick"] + "] : " + message["Message"] + "\n";

        // 가장 최근 메시지가 노출되도록 포지션 강제 업데이트
        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
    }
}
