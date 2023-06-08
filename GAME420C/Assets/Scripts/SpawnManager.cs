using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType
{
    projectile,
    vfx
}

public class SpawnManager : MonoBehaviour
{
    // A static reference to the spawn manager
    public static SpawnManager instance;
    // A dictionary based off of gameobjects to be store all the spawned objects of various types
    public Dictionary<GameObject, List<GameObject>> currentlySpawnedGameObjects = new Dictionary<GameObject, List<GameObject>>();

    // Transforms to sort out the hierarchy
    public Transform projectileTransform;
    public Transform vfxTransform;


    void Awake()
    {
        // Setting up the static reference, and making sure there's not a duplicate instance

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // This function is what's called to get a gameobject, and if there isn't one available, it spawns a new one
    public GameObject GetGameObject(GameObject variantToGet, SpawnType typeOfObject)
    {
        GameObject gathered = null; // The gameobject to return
        bool needToSpawnNewTile = true; // A bool asking if we need to spawn a new gameobject

        //This checks if the type of object being spawned is already in the dictionary
        if (currentlySpawnedGameObjects.ContainsKey(variantToGet))
        {

            // If there is a key, it loops through the list available for an null or inactive object
            for (int i = 0; i < currentlySpawnedGameObjects[variantToGet].Count; i++)
            {
                // It checks if the current oject is null, and if it is, spawns a new object to replace the empty hole in the list
                if (currentlySpawnedGameObjects[variantToGet][i] == null)
                {
                    // Spawns a new object to replace the one in the dictionary that is null
                    currentlySpawnedGameObjects[variantToGet][i] = Instantiate(variantToGet);

                    //Sorts the hierarchy by parenting the new object to the child object using the function to collect the transform
                    currentlySpawnedGameObjects[variantToGet][i].transform.parent = NewSpawnParent(typeOfObject);

                    // Collects the new gameobject as the gathered reference
                    gathered = currentlySpawnedGameObjects[variantToGet][i];
                    needToSpawnNewTile = false;
                    break;
                }
                // If it's not null and inactive, it sets it to active
                else if (currentlySpawnedGameObjects[variantToGet][i].activeInHierarchy == false)
                {
                    //Sets the object found to active
                    currentlySpawnedGameObjects[variantToGet][i].SetActive(true);

                    // Collects the found gameobject as the gathered reference
                    gathered = currentlySpawnedGameObjects[variantToGet][i];
                    needToSpawnNewTile = false;
                    break;
                }
            }
        }
        else // If there is no dictionary to pull from, it adds a new key to the dictionary.
        {
            currentlySpawnedGameObjects.Add(variantToGet, new List<GameObject>());
        }

        // If no object has been found so far, a new game object will be spawned and sorted
        if (needToSpawnNewTile)
        {
            // Spawns a new game object
            gathered = Instantiate(variantToGet);
            
            // Sorts the hierarchy by parenting the new object to the child object using the function to collect the transform
            gathered.transform.parent = NewSpawnParent(typeOfObject);

            // Places the newly spawned object into the dictionary
            currentlySpawnedGameObjects[variantToGet].Add(gathered);
        }

        return gathered;
    }

    // this returns the transform of the child object selected, based on the SpawnType
    public Transform NewSpawnParent(SpawnType typeOfObject)
    {
        switch(typeOfObject)
        {
            case (SpawnType.projectile):
                return projectileTransform;

            case (SpawnType.vfx):
                return vfxTransform;

            default:
                return null;
        }
    }

    public void TurnOffEverything()
    {
        foreach(GameObject objectType in currentlySpawnedGameObjects.Keys)
        {
            for (int i = 0; i < currentlySpawnedGameObjects[objectType].Count; i++)
            {
                // if (currentlySpawnedGameObjects[objectType][i].GetComponent<ProjectileController>() != null)
                // {
                //     currentlySpawnedGameObjects[objectType][i].GetComponent<ProjectileController>().Deactivate();
                // }
            }
        }
    }
}
