using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

//https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey=
//cWnN%2Fzopp8hV7xsVtBqOdALILTz9k6M7QXyMqYijDTawpxOjoRktPtVVQkcl5MmF3S0o%2BflmyomLBvtWePE6Kg%3D%3D
//&pageNo=1
//&numOfRows=1000
//&dataType=JSON
//&base_date=20241104
//&base_time=2100
//&nx=61
//&ny=126

public class WeatherManager : MonoBehaviour
{
    string url = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey=";    // �ȵȴٸ� https->http
    string key = "cWnN%2Fzopp8hV7xsVtBqOdALILTz9k6M7QXyMqYijDTawpxOjoRktPtVVQkcl5MmF3S0o%2BflmyomLBvtWePE6Kg%3D%3D";
    string pageNo = "&pageNo=1";
    string numOfRows = "&numOfRows=1000";
    string dataType = "&dataType=JSON";
    string base_date = "&base_date=20241104";
    string base_time = "&base_time=2100";
    string nx = "&nx=61";
    string ny = "&ny=126";
    //public string weather;

    [SerializeField] private GameObject rain;
    [SerializeField] private GameObject snow;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(GetWeather());

        // ��� �ð� �ֱ⸶�� ���� �ǽð� ������ ������Ʈ����
        StartCoroutine(GetWeather(1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �ǽð� ���� ����, �ð��� ���� ���� ����

    private IEnumerator GetWeather(float time)
    {
        while (true)
        {
            // ���� �ð�
            DateTime currentTime = DateTime.Now;
            int currentHour = currentTime.Hour;

            base_date = currentTime.ToString();
            Debug.Log(base_date);


            base_date = new string(base_date.Where(char.IsDigit).ToArray());
            base_date = base_date.Substring(0, 8);
            Debug.Log(base_date);

            // �ǽð� �ð�
            // �� �ð� 30�п� �����Ͱ� ������Ʈ�Ǳ� ������ 30�� ������ �ð��̶�� �� �ð� ���� ������ �޾ƿ´�.
            if (currentTime.Minute < 30)
            {
                currentTime = currentTime.AddHours(-1);
                base_time = currentTime.ToString("HHmm");
                Debug.Log("30�� �����Դϴ�. 1�ð� �� �� : " + base_time);
            }
            else
            {
                base_time = currentTime.ToString("HHmm");
                Debug.Log(base_time);
            }

            // �䱸���� ��� ���߾���.
            // ���� ��¥, ���� �ð�
            StartCoroutine(RequestWeatherData(base_date, base_time));

            yield return new WaitForSeconds(time*3600f);

            // chatgpt ���
            /*DateTime dateTime = DateTime.Parse(base_date);
            base_date = dateTime.ToString("yyyyMMdd");
            Debug.Log(base_date);
            yield return null;*/
        }
    }

    private IEnumerator RequestWeatherData(string base_date, string base_time)
    {
        string fullUrl = url + key + pageNo + numOfRows + dataType + "&base_date=" + base_date + "&base_time=" + base_time + nx + ny;

        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            // {"response":{"header":{"resultCode":"00","resultMsg":"NORMAL_SERVICE"},"body":{"dataType":"JSON","items":{"item":[{"baseDate":"20241104","baseTime":"2100","category":"PTY","nx":61,"ny":126,"obsrValue":"0"},{"baseDate":"20241104","baseTime":"2100","category":"REH","nx":61,"ny":126,"obsrValue":"47"},{"baseDate":"20241104","baseTime":"2100","category":"RN1","nx":61,"ny":126,"obsrValue":"0"},{"baseDate":"20241104","baseTime":"2100","category":"T1H","nx":61,"ny":126,"obsrValue":"11.3"},{"baseDate":"20241104","baseTime":"2100","category":"UUU","nx":61,"ny":126,"obsrValue":"1.8"},{"baseDate":"20241104","baseTime":"2100","category":"VEC","nx":61,"ny":126,"obsrValue":"276"},{"baseDate":"20241104","baseTime":"2100","category":"VVV","nx":61,"ny":126,"obsrValue":"-0.1"},{"baseDate":"20241104","baseTime":"2100","category":"WSD","nx":61,"ny":126,"obsrValue":"1.8"}]},"pageNo":1,"numOfRows":1000,"totalCount":8}}}



            // ���� �߻���
            if (request.result != UnityWebRequest.Result.Success)
            {
                int requestAgain = 0;
                while(requestAgain <= 4)
                {
                    // ��û ���н� ��� �ð� �������� ���û�� ���� ���ϱ�.
                    yield return new WaitForSeconds(5f);

                    StartCoroutine(RequestWeatherData(base_date,base_time));

                    Debug.Log("���� : " + request.error + " ���û Ƚ�� : " + requestAgain);
                    requestAgain++;
                }
                Debug.Log("5���� ���û ��� ���� �߻�!, ��Ʈ��ũ ������ �ʿ��մϴ�.");
            }
            // ��û�� �����Ͽ� ������ ���� ���
            else
            {
                JObject json = JObject.Parse(request.downloadHandler.text);

                var items = json["response"]["body"]["items"]["item"];

                foreach (var item in items)
                {
                    if (item["category"].ToString() == "PTY")
                    {
                        string value = item["obsrValue"].ToString();
                        int weather = int.Parse(value);
                        SetWeather(weather);
                        //SetWeather(weather);
                        Debug.Log("���� �ǽð� ���� ������ �� : " + value);
                    }

                }
            }

        }
        // ��Ÿ ���� ��ֿ�ҷ� ���� ���� �������µ� �����Ͽ��� ��쿡 �ٽ� �����.
    }

    IEnumerator GetWeather()
    {
        // ���� �ּ�
        string fullUrl = url + key + pageNo + numOfRows + dataType + base_date + base_time + nx + ny;

        // �����͸� �������� get���
        // api�������� �Ծ��� ������ ���� ������ ������ ���� �䱸������ ����. 
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            // {"response":{"header":{"resultCode":"00","resultMsg":"NORMAL_SERVICE"},"body":{"dataType":"JSON","items":{"item":[{"baseDate":"20241104","baseTime":"2100","category":"PTY","nx":61,"ny":126,"obsrValue":"0"},{"baseDate":"20241104","baseTime":"2100","category":"REH","nx":61,"ny":126,"obsrValue":"47"},{"baseDate":"20241104","baseTime":"2100","category":"RN1","nx":61,"ny":126,"obsrValue":"0"},{"baseDate":"20241104","baseTime":"2100","category":"T1H","nx":61,"ny":126,"obsrValue":"11.3"},{"baseDate":"20241104","baseTime":"2100","category":"UUU","nx":61,"ny":126,"obsrValue":"1.8"},{"baseDate":"20241104","baseTime":"2100","category":"VEC","nx":61,"ny":126,"obsrValue":"276"},{"baseDate":"20241104","baseTime":"2100","category":"VVV","nx":61,"ny":126,"obsrValue":"-0.1"},{"baseDate":"20241104","baseTime":"2100","category":"WSD","nx":61,"ny":126,"obsrValue":"1.8"}]},"pageNo":1,"numOfRows":1000,"totalCount":8}}}


            
            // ���� �߻���
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) 
            {
                // ���� �α׸� ���
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                JObject json = JObject.Parse(request.downloadHandler.text);

                var items = json["response"]["body"]["items"]["item"];

                foreach (var item in items) 
                {
                    if (item["category"].ToString() == "PTY")
                    {
                        string value = item["obsrValue"].ToString();
                        Debug.Log("obsrValue : " + value);
                    }
                    
                }
            }
            
        }
    }
    
    private void SetWeather(int weather)
    {
        Weather weatherEnum = (Weather)weather;
        Debug.Log("���� ���� : " + weatherEnum);

        rain.SetActive(false);
        snow.SetActive(false);

        weather = 1;

        if(weather.Equals((int)Weather.Sunny))
        {
            return;
        }
        else if(weather.Equals((int)Weather.Rainy)||weather.Equals((int)Weather.Raindrop))
        {
            rain.SetActive(true);
        }
        else
        {
            snow.SetActive(false);
        }
    }
    /*private void SetWeather(string weather)
    {
        rain.SetActive(false);
        snow.SetActive(false);

        if(weather=="0")
        {
            return;
        }
        else if(weather=="1")
        {
            rain.SetActive(true);
        }
        else if (weather == "2")
        {
            rain.SetActive(true);
            snow.SetActive(true);
        }
        else if(weather == "3")
        {
            snow.SetActive(true);
        }
    }*/
}
