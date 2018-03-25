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
                if (selectedUnit) {
                    var unit = Instantiate(selectedUnit);
                    unit.transform.position = new Vector3(hitInfo.point.x, 0.1f, hitInfo.point.z);

                    if (unit.name.Contains("Unit2")) {
                        unit.GetComponent<Unit>().template = UnitTemplateFactory.defaultUnitTemplate2();
                    } else {
                        unit.GetComponent<Unit>().template = UnitTemplateFactory.defaultUnitTemplate();
                    }

                    unit.GetComponent<Unit>().InitializeInternalData();
                    unit.GetComponent<Unit>().SetMovementTarget(unit.transform.position);
                    unit.name = unit.name + ((int)(Random.value * 100)).ToString();
                }
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
                    } else {
                        selectedUnit.GetComponent<Unit>().SetMovementTarget(target);
                    }
                } else if (hitInfo.transform.gameObject.name.Contains("Unit")) {
                    selectedUnit.GetComponent<Unit>().SetAttackTarget(hitInfo.transform.gameObject);
                }
            }
        }

        if (Input.GetButtonDown("ForcedAttack")) {
            forcedAttackMode = !forcedAttackMode;
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
