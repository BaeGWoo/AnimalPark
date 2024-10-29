using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Hunter.Moveable || Hunter.Attackable)
            ClickPosition();
         
    }

    public void ClickPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                Debug.Log(clickedObject);
                if (Hunter.Moveable)
                {
                    if (clickedObject.CompareTag("MoveAbleBlock"))
                    {
                        Hunter.moveAbleBlock=clickedObject;
                    }
                }

                if (Hunter.Attackable)
                {
                    if (clickedObject.CompareTag("AttackAbleDirection"))
                    {
                        Hunter.attackAbleDirection=clickedObject.transform.position;
                        Hunter.chooseDirection = true;
                    }
                }


            }
        }

    }

}
