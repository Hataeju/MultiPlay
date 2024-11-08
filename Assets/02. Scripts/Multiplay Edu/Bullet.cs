using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // �Ѿ��� �� ����� ���� ��ȣ
    public int actorNumber;

    private float moveSpeed = 10f;

    // �Ѿ��� ������ ���ư���, �÷��̾� �±׿� �浹 �� ��Ȱ��ȭ ó��
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
