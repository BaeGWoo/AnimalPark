using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosionBomb : MonoBehaviour
{
    public void ShootPosionBomb(Vector3 pivot)
    {
        Vector3 dir = (transform.position - pivot).normalized;
        StartCoroutine(BombMove(dir));    
    }


    IEnumerator BombMove(Vector3 dir)
    {
        while (true)
        {
            transform.position += dir * 2.0f * Time.deltaTime;

            // x, z ���� üũ
            if (transform.position.x < 0 || transform.position.x > 14 || transform.position.z < 0 || transform.position.z > 14)
            {
                // ������ ����� ��ü ����
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }


    }
}
