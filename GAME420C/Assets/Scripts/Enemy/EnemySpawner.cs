using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header ("Editables")]
    [SerializeField] private float spawnRate;
    [SerializeField] private GameObject[] enemyTypes;
    [SerializeField] private bool canSpawn = true;


    private void Start()
    {
        StartCoroutine(Spawner());
    }
    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(Random.Range(5,spawnRate));

        while (true)
        {
            yield return wait;

            int rand = Random.Range(0, enemyTypes.Length);
            GameObject enemyToSpawn = enemyTypes[rand];

            Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            spawnRate = Random.Range(1, spawnRate);
        }
    }
}
