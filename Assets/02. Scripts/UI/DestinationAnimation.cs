using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스 클릭한 곳의 목표지점 오브젝트 애니메이션
/// </summary>

public class DestinationAnimation : MonoBehaviour
{
    [SerializeField]
    float offSet = 0.7f;
    [SerializeField]
    float speed = 1f;

    bool movingUp = true;

    void OnEnable()
    {
        StartCoroutine(MoveUpDown());
    }

    IEnumerator MoveUpDown()
    {
        float y = UIManagerWorld.Instance.destinationPosY;
        while (true)
        {
            // Calculate the new y-value based on the movement direction
            float targetY = movingUp ? y + offSet : y;

            // Move the object towards the target y-value
            while (Mathf.Abs(transform.position.y - targetY) > 0.1f)
            {
                float newY = Mathf.MoveTowards(transform.position.y, targetY, speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                yield return null;  // Wait for the next frame
            }

            // Reverse the movement direction
            movingUp = !movingUp;

            // Wait for a brief pause before starting the next movement
            yield return new WaitForSeconds(0.1f);
        }
    }
}
