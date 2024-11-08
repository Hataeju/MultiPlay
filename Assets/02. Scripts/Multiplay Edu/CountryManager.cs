using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CountryManager : MonoBehaviour
{
    //https://apis.data.go.kr/1262000/TravelAlarmService2/getTravelAlarmList2?serviceKey=
    //cWnN%2Fzopp8hV7xsVtBqOdALILTz9k6M7QXyMqYijDTawpxOjoRktPtVVQkcl5MmF3S0o%2BflmyomLBvtWePE6Kg%3D%3D
    //&returnType=JSON
    //&numOfRows=10
    //&pageNo=1

    string country = "가나";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetCountryCondition(country));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GetCountryCondition(string country)
    {

        string Url = "http://apis.data.go.kr/1262000/TravelAlarmService2/getTravelAlarmList2?serviceKey=cWnN%2Fzopp8hV7xsVtBqOdALILTz9k6M7QXyMqYijDTawpxOjoRktPtVVQkcl5MmF3S0o%2BflmyomLBvtWePE6Kg%3D%3D&returnType=JSON&numOfRows=10&pageNo=1";
        
        using (UnityWebRequest request = UnityWebRequest.Get(Url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JObject json = JObject.Parse(request.downloadHandler.text);

                var items = json["data"];

                foreach (var item in items)
                {
                    if (item["country_nm"].ToString() == country)
                    {
                        string value = item["alarm_lvl"].ToString();
                        Debug.Log(country + "의 여행경보지수 : " + value);
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
