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

    private string appId = "612c2529093845179eb8f8359d2d264d"; // 고유 아이디
    private string token = ""; // 토큰 값(상용에서 사용)
    private string currentChannelName;

    private static IRtcEngine rtcEngine; //rtc엔진
    private static GameObject  videoChatObj; //화상 화면 rsw 이미지
    private static Transform videoChatLayout; // 레이아웃
    private void Awake()
    {
        if(instance == null) instance = this;
        
        SetupRTCEngine();
        SetVideo();
        SetUserEventHandler();
    }

    private void Start()
    {
        // Resource 폴더에서 Video Chat Obj 이름의 게임 오브젝트를 로드함.
        videoChatObj = Resources.Load<GameObject>("Video Chat Obj");

        // 캔버스에서 이름으로 찾기.
        videoChatLayout = UIManagerWorld.Instance.canvas.Find("Video Chat");
    }


    // 비디오 서페이스 생성
    private static VideoSurface CreateVideoSurface(string name)
    {
        GameObject videoChat = Instantiate(videoChatObj); // 프리펩 생성

        if (videoChat == null) return null;

        videoChat.name = name;// 이름 할당
         
        if(videoChatLayout != null) /// 레이아웃 검사
        {
            videoChat.transform.SetParent(videoChatLayout, false);
            Debug.Log("Add Video View");
        }
        else
        {
            Debug.Log("Layout is null");
        }

        // 화면 속성 정의
        videoChat.transform.Rotate(0, 0, 180f);
        videoChat.transform.localPosition = Vector3.zero;
        var videoSurface = videoChat.AddComponent<VideoSurface>();

        return videoSurface;
    }

    // 뷰 생성(화면)
    private static void CreateVideoView(uint id, string channelId = "")
    {
        var videoSurface = CreateVideoSurface(id.ToString());

        if (ReferenceEquals(videoSurface, null)) return;



        // 로컬, 리모트 유저
        if (id == 0)
        {
            videoSurface.SetForUser(id, channelId);
        }
        else // 고유 아이디가 0이 아니라면 리모트 유저( 다른 사람)
        {
            videoSurface.SetForUser(id, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        }
        videoSurface.SetEnable(true);
    }

    // 참여
    public void Join(string channelName)
    {
        currentChannelName = channelName;
        rtcEngine.JoinChannel(token, channelName);
        rtcEngine.EnableVideo();

        CreateVideoView(0, channelName);
        Debug.Log("Channel Join : " + channelName);


    }

    // 채널 떠나기
    public void Leave()
    {
        rtcEngine.LeaveChannel();
        rtcEngine.DisableVideo();

        DestroyAll();
        Debug.Log("Leave Channel");
    }

    // 모든 비디오 삭제
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

    // 해당 아이디의 비디오 삭제
    private static void DestroyVideoView(uint id)
    {
        var obj = FindChild(UIManagerWorld.Instance.canvas, id.ToString()).gameObject;
        if(!ReferenceEquals(obj,null)) Destroy(obj);
    }

    // 게임 오브젝트의 이름으로 찾아서 반환하는 함수
    private static Transform FindChild(Transform parent, string name)
    {
        Transform result = parent.Find(name);

        if (result != null) return result;

        // 자식 오브젝트 검색
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
        // agora 인스턴스 생성
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

        //화면 영역
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
        // 채널 참가 시 호출
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("채널 참가 : " + connection.channelId);
        }
        // 유저 오프라인 시 호출
        public override void OnUserOffline(RtcConnection connection, uint remoteUid, USER_OFFLINE_REASON_TYPE reason)
        {
            DestroyVideoView(remoteUid);
        }
        //유저 온라인 시 호출
        public override void OnUserJoined(RtcConnection connection, uint remoteUid, int elapsed)
        {
            CreateVideoView(remoteUid, video.currentChannelName);
        }
    }
}
