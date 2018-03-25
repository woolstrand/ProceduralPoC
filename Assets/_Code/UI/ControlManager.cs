using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour {

    public GameObject ground;
    public GameObject selectedUnit;
    public Camera camera;

    public float scrollSpeed = 6.0f;
    public float zoomSpeed = 100.0f;

    private bool forcedAttackMode;
    private int unitId = 0;

    // Use this for initialization
    void Start() {
    }
	
	// Update is called once per frame
	void Update () {
        PerformMapMoving();

        if (Input.GetButtonDown("Fire1")) { //left button to select
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (hitInfo.transform.gameObject.name.Contains("Unit")) {
                    selectedUnit = hitInfo.transform.gameObject;
                }
            }
        }

        if (Input.GetButtonDown("Fire3")) { //middle button to create new unit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {

                GameObject o = null;

                if (unitId == 0) {
                    o = UnitFactory.CreateUnit(UnitTemplateFactory.defaultUnitTemplate(), "unit");
                } else if (unitId == 1) {
                    o = UnitFactory.CreateUnit(UnitTemplateFactory.defaultUnitTemplate2(), "unit2");
                }

                o.transform.position = new Vector3(hitInfo.point.x, 0.01f, hitInfo.point.z);
                o.GetComponent<Unit>().SetMovementTarget(o.transform.position);

            }
        }

        if (Input.GetButtonDown("Fire2")) { //right mouse button to target terrain for movement or unit for attack
            if (selectedUnit == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (hitInfo.transform.gameObject.name.Equals("Ground")) {
                    Vector3 target = hitInfo.point;
                    if (forcedAttackMode) {
                        forcedAttackMode = false;
                        selectedUnit.GetComponent<Unit>().SetAttackTarget(hitInfo.point);
                        selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Attack);
                    } else {
                        selectedUnit.GetComponent<Unit>().SetMovementTarget(target);
                        selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Move);
                    }
                } else if (hitInfo.transform.gameObject.name.Contains("Unit")) {
                    selectedUnit.GetComponent<Unit>().SetAttackTarget(hitInfo.transform.gameObject);
                    selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Attack);
                }
            }
        }

        if (Input.GetButtonDown("ForcedAttack")) {
            forcedAttackMode = !forcedAttackMode;
        }

        if (Input.GetButtonDown("CycleUnits")) {
            unitId = (unitId + 1) % 2;
        }

    }

    void PerformMapMoving() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float scroll = -Input.GetAxis("Mouse ScrollWheel");

        if (horizontal != 0 || vertical != 0 || scroll != 0) {
            camera.transform.Translate(horizontal * scrollSpeed * Time.deltaTime, scroll * zoomSpeed * Time.deltaTime, vertical * scrollSpeed * Time.deltaTime, Space.World);
        }
    }
}
