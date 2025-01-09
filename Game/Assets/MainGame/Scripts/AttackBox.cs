using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            if (transform.CompareTag("Ice"))
            {
                //other.tag = "IceAnimal";
                other.GetComponent<Monster>().Iced(true);
            }

            else
            {
                other.GetComponent<Monster>().Damaged(1);
            }
        }

        else if (other.CompareTag("IceAnimal"))
        {
            other.GetComponent <Monster>().Damaged(2);
            other.GetComponent<Monster>().Iced(false);
        }
        
    }
}
