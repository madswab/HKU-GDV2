﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    private Gun equippedGun;
    public Transform weaponHold;
    public Gun[] allGuns;

    void Start () {

	}

    void Update () {
		
	}


    public void EquipGun(Gun gunToEquip){
        if (equippedGun != null){
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int gunIndex){
        EquipGun(allGuns[gunIndex]);
    }

    public void OnTriggerHold(){
        if (equippedGun != null){
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease(){
        if (equippedGun != null){
            equippedGun.OnTriggerRelease();
        }
    }

    public float GunHeight{
        get{
            return weaponHold.position.y; 
        }
    }

    public void Aim(Vector3 aimPoint){
        if (equippedGun != null){
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload(){
        if (equippedGun != null){
            equippedGun.Reload();
        }
    }
}