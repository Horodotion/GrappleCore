using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    MeshRenderer mesh;
    Color originalColor;
    public float flashTime = 0.15f;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        originalColor = mesh.material.color;
    }

    public void Flashing()
    {
        mesh.material.color = Color.white;
        Invoke("StopFlash", flashTime);
    }

    private void StopFlash()
    {
        mesh.material.color = originalColor;
    }
}
