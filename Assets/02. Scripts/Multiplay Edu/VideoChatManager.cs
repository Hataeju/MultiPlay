using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agora.Rtc;
using System.Runtime.InteropServices.WindowsRuntime;
using System;

public class VideoChatManager : MonoBehaviour
{
    private static VideoChatManager instance = null;
    public static VideoChatManager Instance {get {return instance;}}

    private string appId = "612c2529093845179eb8f8359d2d264d"; // ���� ���̵�
    private string token = ""; // ��ū ��(��뿡�� ���)
    private string currentChannelName;

    private static IRtcEngine rtcEngine; //rtc����
    private static GameObject  videoChatObj; //ȭ�� ȭ�� rsw �̹���
    private static Transform videoChatLayout; // ���̾ƿ�
    private void Awake()
    {
        if(instance == null) instance = this;
        
        SetupRTCEngine();
        SetVideo();
        SetUserEventHandler();
    }

    private void Start()
    {
        // Resource �������� Video Chat Obj �̸��� ���� ������Ʈ�� �ε���.
        videoChatObj = Resources.Load<GameObject>("Video Chat Obj");

        // ĵ�������� �̸����� ã��.
        videoChatLayout = UIManagerWorld.Instance.canvas.Find("Video Chat");
    }


    // ���� �����̽� ����
    private static VideoSurface CreateVideoSurface(string name)
    {
        GameObject videoChat = Instantiate(videoChatObj); // ������ ����

        if (videoChat == null) return null;

        videoChat.name = name;// �̸� �Ҵ�
         
        if(videoChatLayout != null) /// ���̾ƿ� �˻�
        {
            videoChat.transform.SetParent(videoChatLayout, false);
            Debug.Log("Add Video View");
        }
        else
        {
            Debug.Log("Layout is null");
        }

        // ȭ�� �Ӽ� ����
        videoChat.transform.Rotate(0, 0, 180f);
        videoChat.transform.localPosition = Vector3.zero;
        var videoSurface = videoChat.AddComponent<VideoSurface>();

        return videoSurface;
    }

    // �� ����(ȭ��)
    private static void CreateVideoView(uint id, string channelId = "")
    {
        var videoSurface = CreateVideoSurface(id.ToString());

        if (ReferenceEquals(videoSurface, null)) return;



        // ����, ����Ʈ ����
        if (id == 0)
        {
            videoSurface.SetForUser(id, channelId);
        }
        else // ���� ���̵� 0�� �ƴ϶�� ����Ʈ ����( �ٸ� ���)
        {
            videoSurface.SetForUser(id, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        }
        videoSurface.SetEnable(true);
    }

    // ����
    public void Join(string channelName)
    {
        currentChannelName = channelName;
        rtcEngine.JoinChannel(token, channelName);
        rtcEngine.EnableVideo();

        CreateVideoView(0, channelName);
        Debug.Log("Channel Join : " + channelName);


    }

    // ä�� ������
    public void Leave()
    {
        rtcEngine.LeaveChannel();
        rtcEngine.DisableVideo();

        DestroyAll();
        Debug.Log("Leave Channel");
    }

    // ��� ���� ����
    private void DestroyAll()
    {
        List<uint> ids = new List<uint>();

        for(int i = 0; i< videoChatLayout.transform.childCount; i++)
        {
            ids.Add(uint.Parse(videoChatLayout.transform.GetChild(i).name));
        }

        for(int i = 0; i<ids.Count;i++)
        {
            DestroyVideoView(ids[i]);
        }
    }

    // �ش� ���̵��� ���� ����
    private static void DestroyVideoView(uint id)
    {
        var obj = FindChild(UIManagerWorld.Instance.canvas, id.ToString()).gameObject;
        if(!ReferenceEquals(obj,null)) Destroy(obj);
    }

    // ���� ������Ʈ�� �̸����� ã�Ƽ� ��ȯ�ϴ� �Լ�
    private static Transform FindChild(Transform parent, string name)
    {
        Transform result = parent.Find(name);

        if (result != null) return result;

        // �ڽ� ������Ʈ �˻�
        foreach(Transform child in parent)
        {
            result = FindChild(child, name);
            if (result != null) return result;
        }

        return null;
    }

    // Agora Engine Setup
    private void SetupRTCEngine()
    {
        // agora �ν��Ͻ� ����
        rtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        // settings
        RtcEngineContext context = new RtcEngineContext(appId, 0,
            CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
            AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT,
            AREA_CODE.AREA_CODE_GLOB, null);

        rtcEngine.Initialize(context);
        Debug.Log("Agora Engine Setup");
    }

    private void SetVideo()
    {
        rtcEngine.EnableAudio();
        rtcEngine.EnableVideo();

        VideoEncoderConfiguration config = new VideoEncoderConfiguration();

        //ȭ�� ����
        config.dimensions = new VideoDimensions(200, 200);
        config.frameRate = 15;
        config.bitrate = 0;

        rtcEngine.SetVideoEncoderConfiguration(config);
        rtcEngine.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        rtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        Debug.Log("Agora Video Setup");
    }

    private void SetUserEventHandler()
    {
        UserEventHandler handler = new UserEventHandler(this);
        rtcEngine.InitEventHandler(handler);
        Debug.Log("Setup Euser Event Handler");
    }
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        readonly VideoChatManager video;


        internal UserEventHandler(VideoChatManager videoSample)
        {
            video = videoSample;
        }

        //callback
        // ä�� ���� �� ȣ��
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("ä�� ���� : " + connection.channelId);
        }
        // ���� �������� �� ȣ��
        public override void OnUserOffline(RtcConnection connection, uint remoteUid, USER_OFFLINE_REASON_TYPE reason)
        {
            DestroyVideoView(remoteUid);
        }
        //���� �¶��� �� ȣ��
        public override void OnUserJoined(RtcConnection connection, uint remoteUid, int elapsed)
        {
            CreateVideoView(remoteUid, video.currentChannelName);
        }
    }
}
