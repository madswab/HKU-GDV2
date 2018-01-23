using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle, Chasing, Attacking };
    State currentState;

    public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    private NavMeshAgent pathfinding;
    private Transform target;
    private LivingEntity targetEntity;
    private float attackDistanceThreshold = .5f;
    private float timeBetweenAttacks = 1;
    private float damage = 1;

    private float nextAttackTime;
    private float myCollisionRadius;
    private float targetCollisionRadius;

    private Material skinMaterial;
    private Color originalColor;

    private bool hasTarget;

    private void Awake(){
        pathfinding = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null){
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            targetEntity = target.GetComponent<LivingEntity>();
        }
    }

    protected override void Start () {
        base.Start();

        if (hasTarget){
            currentState = State.Chasing;
            targetEntity.OnDeadth += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
	}
	
	void Update () {
        if (hasTarget){
            if (Time.time > nextAttackTime){
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)){
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
	}

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColour){
        pathfinding.speed = moveSpeed;
        if (hasTarget){
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        deathEffect.startColor = new Color(skinColour.r, skinColour.g, skinColour.b, 1);
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColour;
        originalColor = skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health){
            if (OnDeathStatic != null){
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath(){
        hasTarget = false;
        currentState = State.Idle;
    }

    public override void Die(){
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }

    IEnumerator Attack(){

        currentState = State.Attacking;
        pathfinding.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float percent = 0;
        float attackSpeed = 3;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1){
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage); 
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent ) *4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            
            yield return null;
        }
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinding.enabled = true;

    }

    IEnumerator UpdatePath(){
        float refreshRate = .25f;
        while (hasTarget){
            if (currentState == State.Chasing){
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetpos = target.position  - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead){
                    pathfinding.SetDestination(targetpos);                 
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
