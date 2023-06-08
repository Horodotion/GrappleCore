using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralPlatform : MonoBehaviour
{
    //Adjust this to change speed
    [SerializeField] float speed = 5f;
    //Adjust this to change how high it goes
    [SerializeField] float translate = 0.5f;

    Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }
    void Update()
    {

        //calculate what the new Y position will be
        float newZ = Mathf.Sin(Time.time * speed) * translate + pos.z;
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
    }
}
