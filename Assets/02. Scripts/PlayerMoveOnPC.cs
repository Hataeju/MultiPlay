using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.Rendering;

/// <summary>
/// pc버전의 플레이어 이동
/// </summary>

public class PlayerMoveOnPC : MonoBehaviour
{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE || UNITY_EDITOR_OSX    
        
    NavMeshAgent agent;
    GameObject destinationArrow;
    Vector3 target;

    PhotonView photonView;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();

    }

    void Update()
    {  
        if (Input.GetMouseButtonDown(0))
        {
            if (!photonView.IsMine) return;
            // 마우스가 UI위에 있을 시 이동 X
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // 클릭한 곳이 플레이어일 때 이동 X
            int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
            Ray playerRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, Mathf.Infinity, playerLayerMask))
            {
                if (playerHit.collider.CompareTag("Player"))
                {
                    return;
                }
            }

            // 목표 지점 화살표
            if (destinationArrow != null)
                destinationArrow.SetActive(false);

            int groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.Log("마우스 포지션 : " + Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
            {
                //target = hit.point;
                // 정확한 계산을 위해 네비메쉬 샘플 포지션 사용
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas))
                {
                    target = navMeshHit.position;
                    agent.enabled = true;
                    agent.isStopped = false;
                    agent.SetDestination(target);

                    GameManager.Instance.SetAni();

                    //Debug.Log("클릭한 곳 좌표 : " + target);
                    if (destinationArrow == null)
                    {
                        destinationArrow = Instantiate(UIManagerWorld.Instance.destinationPrefab);
                    }
                    destinationArrow.SetActive(true);
                    destinationArrow.transform.position = new Vector3(target.x, target.y + 0.7f, target.z);
                    UIManagerWorld.Instance.destinationPosY = destinationArrow.transform.position.y;
                }
            }
        }        
    }    

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Arrow"))
        {
            other.gameObject.SetActive(false);
            agent.isStopped = true;
            agent.enabled = false;
            GameManager.Instance.SetIdle();
        }
    }
#endif
}
