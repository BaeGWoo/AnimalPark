using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    
    [SerializeField] float curdistance;
    
    public Material redMaterial; // 빨간색 재질
    private Material originalMaterial; // 원래 재질
    private Renderer objectRenderer;
    
    public Vector3 curPostiion;
    
    public bool moveableArea = false;
    public static GameObject targetblock;
   
 
    void Awake()
    {
        // 기본 Material값 저장
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
    }

    private void Start()
    {
        curPostiion = transform.position;

    }

    void Update()
    {
        if (Hunter.Moveable || Hunter.fireball)
        {
            if (!Hunter.Running)
            {
                if (Hunter.Moveable)
                {
                    StartCoroutine(MoveableArea());

                }

                else if (Hunter.fireball)
                {
                    StartCoroutine(AttackAbleDirection());
                }

                else
                {
                    StartCoroutine(ReturnArea());
                }
            }

            else
            {
                StartCoroutine(ReturnArea());
            }
        }

        else
        {
            StartCoroutine(ReturnArea());
        }
        

    }

    public IEnumerator MoveableArea()
    {
        float distance;
        distance = Math.Abs(Hunter.HunterPosition.x - curPostiion.x) + Math.Abs(Hunter.HunterPosition.z - curPostiion.z);

        if (distance <= 6&&distance>=1)
        {
            if (gameObject != targetblock)
            {
                objectRenderer.material = redMaterial;
                moveableArea = true;
                gameObject.tag = "MoveAbleBlock";
            }

        }
        yield return null; 
    }

    public IEnumerator AttackAbleDirection()
    {
        float distance;
        distance = Math.Abs(Hunter.HunterPosition.x - curPostiion.x) + Math.Abs(Hunter.HunterPosition.z - curPostiion.z);
        if (distance <= 2&&distance>=1)
        {
            if (this.gameObject != targetblock)
            {
                objectRenderer.material = redMaterial;
                gameObject.tag = "AttackAbleDirection";
            }

        }
        yield return null; 
    }

    public IEnumerator ReturnArea()
    {
        objectRenderer.material = originalMaterial;
        gameObject.tag = "Block";
        moveableArea = false;
        yield return null; 
    }

}
