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
        if (Input.GetButtonDown("Fire1")) { //left button to select or move
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (hitInfo.transform.gameObject.name.Equals("Ground") && selectedUnit != null) {
                    Vector3 target = hitInfo.point;
                    selectedUnit.GetComponent<Unit>().SetMovementTarget(target);
                } else if (hitInfo.transform.gameObject.name.Contains("Unit")) {
                    selectedUnit = hitInfo.transform.gameObject;
                }
            }
        }

        if (Input.GetButtonDown("Fire2")) { //right button to create new unit (also Alt does that, so Alt-Tab will produce some units)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (selectedUnit) {
                    var unit = Instantiate(selectedUnit);
                    unit.transform.position = new Vector3(hitInfo.point.x, 0.1f, hitInfo.point.z);
                    unit.GetComponent<Unit>().SetMovementTarget(unit.transform.position);
                    unit.GetComponent<Unit>().template = UnitTemplateFactory.defaultUnitTemplate();

                    unit.name = unit.name + ((int)(Random.value * 100)).ToString();
                }
            }
        }

        if (Input.GetButtonDown("Fire3")) { //Middle mouse button to target terrain or unit
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
