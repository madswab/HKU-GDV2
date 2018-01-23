using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInputAim : MonoBehaviour
#if UNITY_ANDROID
    , IDragHandler, IPointerUpHandler, IPointerDownHandler
#endif
{

    private Image bgImage;
    private Image joystickImage;
    private Vector3 inputVector;
    private PlayerController controller;
    private GunController gunController;
    private GameObject player;
    public Crosshair crosshair;


    void Start() {
        bgImage = GetComponent<Image>();
        joystickImage = transform.GetChild(0).GetComponent<Image>();
        player = GameObject.Find("Player");
        controller = player.GetComponent<PlayerController>();
        gunController = player.GetComponent<GunController>();
    }

    void Update() {

    }


    public void AimInput() {
#if UNITY_STANDALONE_WIN
        joystickImage.enabled = false;
        bgImage.enabled = false;

        if (Input.GetMouseButton(0)) {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0)) {
            gunController.OnTriggerRelease();
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshair.transform.position = point;
            crosshair.DetectTargets(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1) {
                gunController.Aim(point);
            }
        }
    }
}
#endif

#if UNITY_ANDROID
        Cursor.visible = true;
        joystickImage.enabled = true;
        bgImage.enabled = true;

        //if (Horizontal() != 0 && Vertical() != 0){
        //    gunController.OnTriggerHold();
        //    gunController.OnTriggerRelease();
        //}
       // if (Horizontal() == 0 && Vertical() == 0){
       //     gunController.OnTriggerRelease();
       // }
        Vector3 aim = new Vector3(player.transform.position.x + Horizontal() * 2, gunController.GunHeight, player.transform.position.z + Vertical() * 2);
        crosshair.transform.position = aim;
        controller.LookAt(aim);
        if ((new Vector2(aim.x, aim.z) - new Vector2(player.transform.position.x, player.transform.position.z)).sqrMagnitude > 1){
            gunController.Aim(aim);
        }
        else{
            gunController.Aim(aim * 1);
        }
    }

    public virtual void OnDrag(PointerEventData ped){
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform, ped.position, ped.pressEventCamera, out pos)){
            pos.x = (pos.x / bgImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImage.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2 + 1, 0, pos.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1f) ? inputVector.normalized : inputVector;
           
            // move joystick img
            joystickImage.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImage.rectTransform.sizeDelta.x / 2.4f), inputVector.z * (bgImage.rectTransform.sizeDelta.y / 2.4f));
            gunController.OnTriggerHold();

        }
    }
    public virtual void OnPointerDown(PointerEventData ped){
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped){
        //inputVector = Vector3.zero;
        joystickImage.rectTransform.anchoredPosition = Vector3.zero;
        gunController.OnTriggerRelease();
    }

    public float Horizontal(){
        if (inputVector.x != 0){
            return inputVector.x;
        }
        else{
            return Input.GetAxis("Horizontal");
        }
    }

    public float Vertical(){
        if (inputVector.z != 0){
            return inputVector.z;
        }
        else{
            return Input.GetAxis("Vertical");
        }
    }
}
#endif

