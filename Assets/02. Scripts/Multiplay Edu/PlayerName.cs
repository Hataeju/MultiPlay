using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerName : MonoBehaviour
{
    // 플레이어 이름 오브젝트와 플레이어의 간격
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerName = Instantiate(UIManagerWorld.Instance.playerName, transform);
        playerName.transform.localPosition += offset;
        // 포톤 뷰 소유자의 닉네임을 할당
        // Owner.NickName은 포톤에 의해 자동 동기화 되므로 별도의 설정이 필요없다.

        playerName.GetComponent<TextMeshPro>().text = GetComponent<PhotonView>().Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
