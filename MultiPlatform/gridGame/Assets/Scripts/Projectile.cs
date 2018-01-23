using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private float speed = 10;
    private float damage = 1;
    public LayerMask colMask;
    public Color trailcolour;
    private float lifeTime = 3;
    private float skinWidth = 0.1f;

	void Start () {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, colMask);
        if (initialCollisions.Length > 0){
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailcolour);
	}
	

	void Update () {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
	}

    public void SetSpeed(float newSpeed){
        speed = newSpeed;
    }

    void CheckCollisions(float moveDistace){
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit, moveDistace + skinWidth, colMask, QueryTriggerInteraction.Collide)){
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint){

        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null){
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
