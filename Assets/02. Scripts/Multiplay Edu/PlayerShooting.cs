using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerShooting : MonoBehaviourPun
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPostion;

    private ChatManager chatManager;
    private Animator animator;

    private int maxHP = 100;
    private int currentHP;
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        chatManager = GetComponent<ChatManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            //StartCoroutine(Shooting(1));
            TryShooting();
        }
    }
    private IEnumerator Shooting(int actor)
    {
        animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(0.5f);

        GameObject bullet = Instantiate(bulletPrefab, shootingPostion.position, shootingPostion.rotation);

        bullet.GetComponent<Bullet>().actorNumber = actor;
    }

    public void TryShooting()
    {
        if(photonView.IsMine)
        {
            ShootingBullet(photonView.Owner.ActorNumber);
            photonView.RPC("ShootingBullet",RpcTarget.Others, photonView.Owner.ActorNumber);
        }
    }

    [PunRPC]
    private void ShootingBullet(int actor)
    {
        StartCoroutine(Shooting(actor));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if(other.CompareTag("Bullet") && currentHP>=0)
        {
            currentHP -= 50;

            if(currentHP <= 0)
            {
                int actor = other.GetComponent<Bullet>().actorNumber;

                // 현재 방에서 고유 번호로 쏜 사람의 정보를 가져온다.
                Player shooter = PhotonNetwork.CurrentRoom.GetPlayer(actor);

                string message = string.Format("<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",
                    photonView.Owner.NickName, shooter.NickName);

                chatManager.SendMessage(message);
            }
        }
    }
}
