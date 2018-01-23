using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]

public class Player : LivingEntity {

    public float movespeed = 5;
    public Crosshair crosshair;
    private Camera viewCamera;
    private PlayerController controller;
    private GunController gunController;
    private PlayerInputWalk inputWalk;
    private PlayerInputAim inputAim;


    private void Awake() {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        inputWalk = FindObjectOfType<PlayerInputWalk>();
        inputAim = FindObjectOfType<PlayerInputAim>();

        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        viewCamera = Camera.main;
    }

    protected override void Start() {
        base.Start();

    }

    void Update() {
        // movement input
        Vector3 moveVelocity = inputWalk.MovementInput().normalized * movespeed;
        controller.Move(moveVelocity);

        //Look input
        inputAim.AimInput();        

        // weapon input
        //if (Input.GetMouseButton(0)){
        //   gunController.OnTriggerHold();
        //}
        //if (Input.GetMouseButtonUp(0)){
        //    gunController.OnTriggerRelease();
        //}
        if (Input.GetKeyDown(KeyCode.R)){
            gunController.Reload();
        }
        if (transform.position.y < -10){
            TakeDamage(health);
        }
    }

    void OnNewWave(int waveNr){
        health = startingHealth;
        gunController.EquipGun(waveNr - 1);
    }
}

/*
// look input
#if UNITY_STANDALONE_WIN
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray,out rayDistance)){
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshair.transform.position = point;
            crosshair.DetectTargets(ray);
            if ((new Vector2(point.x,point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1){
                gunController.Aim(point);
            }
        }
#endif

#if UNITY_ANDROID
        Vector3 aim = new Vector3(transform.position.x + inputAim.Horizontal() * 2, gunController.GunHeight, transform.position.z + inputAim.Vertical() * 2);
        crosshair.transform.position = aim;
        controller.LookAt(aim);
        if ((new Vector2(aim.x, aim.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1){
            gunController.Aim(aim);
        }else{
            gunController.Aim(aim * 1);
        }
        if (inputAim.Horizontal() != 0 || inputAim.Vertical() != 0){
            gunController.OnTriggerHold();
            gunController.OnTriggerRelease();
        }
#endif

// movement input
     
#if UNITY_STANDALONE_WIN
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
#endif

#if UNITY_ANDROID
        Vector3 moveInput = new Vector3(inputWalk.Horizontal(), 0, inputWalk.Vertical());   
#endif

Vector3 moveVelocity = moveInput.normalized * movespeed;
controller.Move(moveVelocity);
*/

