using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks // ���� �ݹ� Ű�ɰ� �������̺���� ��� ����
{
    // ������ ��ü ���� => ��𿡼��� �Ŵ����� ������ �� �ֵ��� �̱��� ���� ���
    private static PhotonManager instance = null;
    public static PhotonManager Instance { get { return instance; } }

    
    // �� ������ ���� �ʵ�
    // ���� �����̾�߸� ���� ������ ���� ����
    private string roomVersion = "1.0.0";
    // ���� �̸����� ���� ���� ���� ����
    // �� ������ �� �̸��� ��� ��ġ�ؾ� ����ȭ�� �� ����.
    private string roomName = "Test";



    // �÷��̾ �����Ǿ����� �˸�
    public delegate void PlayerCreatedEvent();
    public event PlayerCreatedEvent PlayerCreated;

    // �÷��̾� ���� ��ġ ����
    [SerializeField] private Vector3 createPosition = new Vector3(55f, 0, -55f);

    // �÷��̾�
    public GameObject player;

    private void OnPlayerCreated()
    {
        PlayerCreated?.Invoke();
    }

    // �÷��̾� ���� �ڷ�ƾ
    private IEnumerator CreatePlayer(Vector3 creatPos)
    {
        // �������� �����͸� �޾ƿ��� �ð�,
        // ���⼭�� ���Ƿ� 1�� ���ϴ�.
        yield return new WaitForSeconds(1f);

        // �÷��̾� ���� �� �г��� ����
        PhotonNetwork.NickName = Config.userNickName;

        // �÷��̾� ���� ȸ����
        Quaternion rotation = Quaternion.Euler(0, -90f, 0);

        // �÷��̾� ����
        player = PhotonNetwork.Instantiate("Player", creatPos, rotation, 0);

        OnPlayerCreated();

    }
    private void Awake()
    {
        // �̱��� �ʱ�ȭ ��, �� ��ȯ �� �ߺ�������Ʈ ����
        if(instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        // �� ��ȯ�ÿ��� �����ϵ��� ��.
        DontDestroyOnLoad(gameObject);
    }

    // ���� ������Ʈ�� Ȱ��ȭ�� ������ ȣ��
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // �� ��ȯ�� �� ������ ȣ���� �̺�Ʈ �ڵ鷯
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� ���� Ȱ��ȭ�� ���� 0�� ° ���̶�� (Login��)
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            // ������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ
            PhotonNetwork.AutomaticallySyncScene = true;
            // ���� ����
            PhotonNetwork.GameVersion = roomVersion;

            // ���� �������� �ʴ� ������ ���۷�
            Debug.Log("���� �������� �ʴ� ������ ���� : " + PhotonNetwork.SendRate);
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(CreatePlayer(createPosition));
        }
    }

    // 1. ���濡 �����ϴ� �Լ�
    public void ConnectToPhoton()
    {
        // ���� ����� ����Ǿ� ���� �ʴٸ�
        if(!PhotonNetwork.IsConnected)
        {
            // ���� ���� ������ ���濡 ����
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 2. ������ Ŭ���̾�Ʈ�� ���濡 ������ �� ȣ��Ǵ� �ݹ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("���濡 ���� ���� ���� : " + PhotonNetwork.IsConnected);

        // 3. �κ� �����Ѵ�.
        PhotonNetwork.JoinLobby();
    }

    // 4. �κ� �����Ͽ��ٸ� ȣ��Ǵ� �ݹ�
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ���� ���� : " + PhotonNetwork.InLobby);
        // �κ� -> ��
        JoinRoom();
    }

    // 5. �濡 ����
    public void JoinRoom()
    {
        // ���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 0; // �ִ� �÷��̾� ����(0�� ���� ����)
        roomOptions.IsOpen = true; // ���� ��������?
        roomOptions.IsVisible = true; // ���� ���̰� ����?

        // �濡 �����ϰų� ����� �Լ�
        // ������ Ŭ���̾�Ʈ�� ��� ���� ����鼭 ���ÿ� �����ϰ� �ǰ�,
        // �� �̿��� ������ ������ �ϰ� �ȴ�.
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // �� ���� �Ϸ�
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� �Ϸ� : " + PhotonNetwork.CurrentRoom.Name);
    }

    // �濡 ����� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.Log("�濡 ���� ���� ���� : " + PhotonNetwork.InRoom + ", �ο� �� : " + PhotonNetwork.CurrentRoom.PlayerCount);

        // 7. ���ξ� �ҷ�����
        LoadMainScene();
    }

    // ���� �� �ε�
    // ������ Ŭ���̾�Ʈ�� ��� ���� ���� �ε�,
    // �Ϲ� ������ ��� �����Ͱ� �ε��� ���� �ڵ� ����ȭ�˴ϴ�.
    private void LoadMainScene()
    {
        Debug.Log("���� �� �ε�,,.?");

        // ������ Ŭ���̾�Ʈ�� ��� ���ξ� �ҷ�����
        if( PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            Debug.Log("������ Ŭ���̾�Ʈ�� �ƴ�");

        }
    }
    // ���� ��Ȳ�� �����ϱ� ���� ���� �ڵ�
    // ��Ÿ Ư�̻��� �Ǵ� Ÿ�̹� �̽��� ���� ���� ���� ��Ȳ�� ����Ͽ�
    // �� ���� ���н� ȣ��Ǵ� �ݹ� �ۼ�
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("�� ���� ���� : " + returnCode + message + "�� ���� �õ�...?");
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
