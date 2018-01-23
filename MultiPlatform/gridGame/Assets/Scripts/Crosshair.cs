using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHeiglightColour;
    private Color originalDotColour;

    void Start () {
        Cursor.visible = false;
        originalDotColour = dot.color;
	}

    void Update () {
        transform.Rotate(Vector3.forward * -50 * Time.deltaTime);
	}

    public void DetectTargets(Ray ray){
        if (Physics.Raycast(ray,100,targetMask)){
            dot.color = dotHeiglightColour;
        }
        else{
            dot.color = originalDotColour;
        }
    }
}
