using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoints : MonoBehaviour
{
    [SerializeField] GameObject graphic1;
    [SerializeField] GameObject graphic2;
    public Transform[] spawnPointsList;

    // Start is called before the first frame update
    void Start()
    {
        graphic1.SetActive(false);
        graphic2.SetActive(false);
    }
}
