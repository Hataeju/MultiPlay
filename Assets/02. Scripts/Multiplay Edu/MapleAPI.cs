using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class  PlayerInfo
{
    public string character_name;
    public string world_name;
    public string character_class;
    public string character_image;
}
public class MapleAPI : MonoBehaviour
{
    private string apiKey = "";

    [SerializeField] private string nickName;
    [SerializeField] private Button buttonGetOCID;

    [SerializeField] private Button buttonGetInfo;

    [SerializeField] private TextMeshProUGUI tmpOCID;


    [SerializeField] private TextMeshProUGUI tmpName;
    [SerializeField] private TextMeshProUGUI tmpWorld;
    [SerializeField] private TextMeshProUGUI tmpJob;
    [SerializeField] private Image avatarImage;

    private string imageURL;
    // Start is called before the first frame update
    void Start()
    {
        buttonGetOCID.onClick.AddListener(() => 
        { 
            nickName = tmpName.text;
            StartCoroutine(GetOCID()); 
        });

        buttonGetInfo.onClick.AddListener(() =>
        {
            StartCoroutine(GetInfo());
        });
    }

    private IEnumerator GetOCID()
    {
        string url = "" + nickName;

        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("x-nxopen-api-key", apiKey);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success) 
        {
            Debug.Log(request.downloadHandler.text);

            string tempOcid = RemoveString(request.downloadHandler.text, 9 ,2);
            tmpOCID.text = tempOcid;
            //ocid = tempOcid;
        }
    }

    private IEnumerator GetInfo()
    {
        string url = "" + tmpOCID + "&date=2024-11-05";

        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("x-nxopen-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // json을 class로 역직렬화
            PlayerInfo info = JsonConvert.DeserializeObject<PlayerInfo>(request.downloadHandler.text);
            tmpName.text = info.character_name;
            tmpWorld.text = info.world_name;
            tmpJob.text = info.character_class;
            imageURL = info.character_image;
            Debug.Log(imageURL);

            StartCoroutine(GetImage());
        }
    }

    private IEnumerator GetImage()
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return request.SendWebRequest();

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            avatarImage.sprite = sprite;
        }


    }

    private string RemoveString(string input, int front, int back)
    {
        if(input.Length <= front)
        {
            return "";
        }

        string frontRemove = input.Substring(front);
        string backRemove = frontRemove.Substring(0, frontRemove.Length - back);
        return backRemove;
    }


    // Update is called once per frame
    void Update()
    {



    }
}
