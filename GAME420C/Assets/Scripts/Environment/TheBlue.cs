using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlue : MonoBehaviour
{
    public GameObject theBlue;
    public float shrinkRate;
    public int gracePeriod;
    public bool inGrace = true;
    public bool shrink = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Shrink());
    }

    // Update is called once per frame
    void Update()
    {
        if(!inGrace && shrink)
        {
            theBlue.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f) * shrinkRate * Time.deltaTime;
        }
    }

    private IEnumerator Shrink()
    {
        Debug.Log("Shrink begins in " + gracePeriod.ToString() + " seconds.");
        
        yield return new WaitForSeconds(gracePeriod);

        Debug.Log("Blue Active.");
        BlueActive();
    }

    private void BlueActive()
    {
        inGrace = false;
        shrink = true;
    }
}
