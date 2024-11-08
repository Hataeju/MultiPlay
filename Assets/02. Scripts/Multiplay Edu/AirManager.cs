using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AirManager : MonoBehaviour
{
    //https://apis.data.go.kr/B552584/ArpltnInforInqireSvc/getCtprvnRltmMesureDnsty?serviceKey=
    //cWnN%2Fzopp8hV7xsVtBqOdALILTz9k6M7QXyMqYijDTawpxOjoRktPtVVQkcl5MmF3S0o%2BflmyomLBvtWePE6Kg%3D%3D
    //&returnType=json
    //&numOfRows=100
    //&pageNo=1
    //&sidoName=
    //%EC%B6%A9%EB%B6%81
    //&ver=1.0
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetAirCondition("ÃæºÏ"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GetAirCondition(string city)
    {
        
        string url = "http://apis.data.go.kr/B552584/ArpltnInforInqireSvc/getCtprvnRltmMesureDnsty?serviceKey=cWnN%2Fzopp8hV7xsVtBqOdALILTz9k6M7QXyMqYijDTawpxOjoRktPtVVQkcl5MmF3S0o%2BflmyomLBvtWePE6Kg%3D%3D&returnType=json&numOfRows=100&pageNo=1&sidoName=";
        string sido = Uri.EscapeDataString(city);
        string ver = "&ver=1.0";

        string fullUrl = url + sido + ver;

        using(UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.Success)
            {
                JObject json = JObject.Parse(request.downloadHandler.text);

                var items = json["response"]["body"]["items"];

                foreach(var item in items )
                {
                    if (item["stationName"].ToString() == "¿ÀÃ¢À¾")
                    {
                        string value = item["o3Grade"].ToString();
                        Debug.Log("ÃæºÏ ¿ÀÃ¢À¾ÀÇ ¿ÀÁ¸Áö¼ö : " + value);
                    }
                }
            }
            else
            {

            }
        }
        yield return null;
    }
}
