using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSpawner : MonoBehaviour
{
    [Header("Editables")]
    [SerializeField] private float gracePeriod;
    [SerializeField] private float spawnRate;
    [SerializeField] private GameObject[] dataSpheres;
    [SerializeField] private int maxSpawn = 0;
    [SerializeField] private int spawnCount = 0;


    private void Start()
    {
        StartCoroutine(GracePeriod());
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (spawnCount <= maxSpawn)
        {
            yield return wait;

            spawnCount++;
            int rand = Random.Range(0, dataSpheres.Length);
            GameObject dataToSpawn = dataSpheres[rand];

            Instantiate(dataToSpawn, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator GracePeriod()
    {
        yield return new WaitForSeconds(gracePeriod);

        StartCoroutine(Spawner());
    }
}
