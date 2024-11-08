using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 총알을 쏜 사람의 고유 번호
    public int actorNumber;

    private float moveSpeed = 10f;

    // 총알은 앞으로 나아가고, 플레이어 태그와 충돌 시 비활성화 처리
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }

    }
}
