using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerName : MonoBehaviour
{
    // �÷��̾� �̸� ������Ʈ�� �÷��̾��� ����
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerName = Instantiate(UIManagerWorld.Instance.playerName, transform);
        playerName.transform.localPosition += offset;
        // ���� �� �������� �г����� �Ҵ�
        // Owner.NickName�� ���濡 ���� �ڵ� ����ȭ �ǹǷ� ������ ������ �ʿ����.

        playerName.GetComponent<TextMeshPro>().text = GetComponent<PhotonView>().Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
