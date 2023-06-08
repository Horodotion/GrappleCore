using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningPlatform : MonoBehaviour
{
    [SerializeField] float spinSpeed = 30f;
    [SerializeField] Vector3 _axis = Vector3.forward;
    void Update()
    {
        transform.Rotate(_axis.normalized * spinSpeed* Time.deltaTime);
    }
}
