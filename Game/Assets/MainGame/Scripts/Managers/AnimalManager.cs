using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [SerializeField] GameObject[] curAnimals;

    public GameObject[] GetCurrentAnimals() { return curAnimals; }
}
