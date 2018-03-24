using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour {

    public GameObject ground;
    public GameObject selectedUnit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (hitInfo.transform.gameObject.name.Equals("Ground") && selectedUnit != null) {
                    selectedUnit.GetComponent<Unit>().SetMovementTarget(hitInfo.point);
                } else if (hitInfo.transform.gameObject.name.Contains("Unit")) {
                    selectedUnit = hitInfo.transform.gameObject;
                }
            }
        }

        if (Input.GetButtonDown("Fire2")) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (selectedUnit) {
                    var unit = Instantiate(selectedUnit);
                    unit.transform.position = new Vector3(hitInfo.point.x, 0.1f, hitInfo.point.z);
                    unit.GetComponent<Unit>().SetMovementTarget(unit.transform.position);
                    unit.GetComponent<Unit>().template = UnitTemplateFactory.defaultUnitTemplate();
                }
            }
        }

        if (Input.GetButtonDown("Fire3")) {
            if (selectedUnit == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (hitInfo.transform.gameObject.name.Equals("Ground")) {
                    selectedUnit.GetComponent<Unit>().SetAttackTarget(hitInfo.point);
                } else if (hitInfo.transform.gameObject.name.Contains("Unit")) {
                    selectedUnit.GetComponent<Unit>().SetAttackTarget(hitInfo.transform.gameObject);
                }
            }
        }

    }
}
