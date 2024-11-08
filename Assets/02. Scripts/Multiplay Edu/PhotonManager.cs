using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks // 포톤 콜백 키능과 모노비헤이비어의 기능 수행
{
    // 유일한 객체 선언 => 어디에서나 매니저에 접근할 수 있도록 싱글톤 패턴 사용
    private static PhotonManager instance = null;
    public static PhotonManager Instance { get { return instance; } }

    
    // 방 정보에 대한 필드
    // 같은 버전이어야만 같은 공간에 접속 가능
    private string roomVersion = "1.0.0";
    // 방의 이름으로 여러 방을 생성 가능
    // 즉 버전과 방 이름이 모두 일치해야 동기화할 수 있음.
    private string roomName = "Test";



    // 플레이어가 생성되었음을 알림
    public delegate void PlayerCreatedEvent();
    public event PlayerCreatedEvent PlayerCreated;

    // 플레이어 생성 위치 제어
    [SerializeField] private Vector3 createPosition = new Vector3(55f, 0, -55f);

    // 플레이어
    public GameObject player;

    private void OnPlayerCreated()
    {
        PlayerCreated?.Invoke();
    }

    // 플레이어 생성 코루틴
    private IEnumerator CreatePlayer(Vector3 creatPos)
    {
        // 서버에서 데이터를 받아오는 시간,
        // 여기서는 임의로 1초 쉽니다.
        yield return new WaitForSeconds(1f);

        // 플레이어 생성 시 닉네임 주입
        PhotonNetwork.NickName = Config.userNickName;

        // 플레이어 생성 회전값
        Quaternion rotation = Quaternion.Euler(0, -90f, 0);

        // 플레이어 생성
        player = PhotonNetwork.Instantiate("Player", creatPos, rotation, 0);

        OnPlayerCreated();

    }
    private void Awake()
    {
        // 싱글톤 초기화 및, 씬 전환 시 중복오브젝트 방지
        if(instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        // 씬 전환시에도 유지하도록 함.
        DontDestroyOnLoad(gameObject);
    }

    // 게임 오브젝트가 활성화될 때마다 호출
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬 전환이 될 때마다 호출할 이벤트 핸들러
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 만약 현재 활성화된 씬이 0번 째 씬이라면 (Login씬)
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            // 마스터 클라이언트의 씬 자동 동기화
            PhotonNetwork.AutomaticallySyncScene = true;
            // 버전 설정
            PhotonNetwork.GameVersion = roomVersion;

            // 포톤 서버와의 초당 데이터 전송량
            Debug.Log("포톤 서버와의 초당 데이터 전송 : " + PhotonNetwork.SendRate);
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(CreatePlayer(createPosition));
        }
    }

    // 1. 포톤에 접속하는 함수
    public void ConnectToPhoton()
    {
        // 만약 포톤과 연결되어 있지 않다면
        if(!PhotonNetwork.IsConnected)
        {
            // 포톤 세팅 값으로 포톤에 연결
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 2. 마스터 클라이언트가 포톤에 접속할 시 호출되는 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤에 접속 성공 유무 : " + PhotonNetwork.IsConnected);

        // 3. 로비에 접속한다.
        PhotonNetwork.JoinLobby();
    }

    // 4. 로비에 접속하였다면 호출되는 콜백
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속 성공 유무 : " + PhotonNetwork.InLobby);
        // 로비 -> 룸
        JoinRoom();
    }

    // 5. 방에 접속
    public void JoinRoom()
    {
        // 방의 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 0; // 최대 플레이어 제한(0은 제한 없음)
        roomOptions.IsOpen = true; // 방을 열어줄지?
        roomOptions.IsVisible = true; // 방을 보이게 할지?

        // 방에 참가하거나 만드는 함수
        // 마스터 클라이언트의 경우 방을 만들면서 동시에 참가하게 되고,
        // 그 이외의 유저는 조인을 하게 된다.
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // 방 생성 완료
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료 : " + PhotonNetwork.CurrentRoom.Name);
    }

    // 방에 입장시 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("방에 입장 성공 유무 : " + PhotonNetwork.InRoom + ", 인원 수 : " + PhotonNetwork.CurrentRoom.PlayerCount);

        // 7. 메인씬 불러오기
        LoadMainScene();
    }

    // 메인 씬 로드
    // 마스터 클라이언트의 경우 메인 씬을 로드,
    // 일반 유저의 경우 마스터가 로드한 씬과 자동 동기화됩니다.
    private void LoadMainScene()
    {
        Debug.Log("메인 씬 로드,,.?");

        // 마스터 클라이언트인 경우 메인씬 불러오기
        if( PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            Debug.Log("마스터 클라이언트가 아님");

        }
    }
    // 예외 상황에 대응하기 위한 안전 코드
    // 가타 특이사항 또는 타이밍 이슈로 인해 방이 없을 상황에 대비하여
    // 방 접속 실패시 호출되는 콜백 작성
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 접속 실패 : " + returnCode + message + "방 생성 시도...?");
        JoinRoom();
    }
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
