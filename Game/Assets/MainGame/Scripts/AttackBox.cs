using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    [SerializeField] GameObject IceEffect;
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
            GameObject prefab=Instantiate(IceEffect);
            prefab.transform.position = other.transform.position;           

            other.GetComponent <Monster>().Damaged(2);
            other.GetComponent<Monster>().Iced(false);
        }

        else if (other.CompareTag("IceObject"))
        {
            int x = Random.Range(0, 7) * 2;
            int z = Random.Range(0, 4) * 2;
            other.transform.position = new Vector3(x, 0, z);
        }
        
    }

    IEnumerator IceMotion(GameObject motion)
    {
        motion.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        motion.SetActive(false);
    }
}
