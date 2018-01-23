using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

    public Rigidbody rb;
    public float forceMin;
    public float forceMax;

    private float lifetime = 4;
    private float fadetime = 2;

    void Start () {
        float force = Random.Range(forceMin, forceMax);
        rb.AddForce(transform.right * force);
        rb.AddTorque(Random.insideUnitSphere * force);
        StartCoroutine(Fade());
	}


    void Update () {
		
	}

    IEnumerator Fade(){
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadespeed = 1 / fadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColour = mat.color;

        while(percent < 1){
            percent += Time.deltaTime * fadespeed;
            mat.color = Color.Lerp(initialColour, Color.clear, percent);
            yield return null;

        }
        Destroy(gameObject);
    }
}
