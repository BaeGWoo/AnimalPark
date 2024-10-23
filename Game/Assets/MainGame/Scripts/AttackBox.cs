using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            other.GetComponent<Animal>().Damaged();
        }
        
    }
}
