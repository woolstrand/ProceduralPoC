using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour, IOrderResponder {

    public GameObject ground;
    public GameObject selectedUnit;
    public GUIManager guiManager;
    public Camera camera;

    public float scrollSpeed = 6.0f;
    public float zoomSpeed = 100.0f;

    private bool forcedAttackMode;
    private int unitId = 0;

    private List<GameObject> selectedUnits;

    private Vector2 selectionStart;
    private Vector2 selectionEnd;

    UnitOrder? activeOrder;


    // Use this for initialization
    void Start() {
        selectedUnits = new List<GameObject>();
        guiManager.orderResponder = this;
    }
	
	// Update is called once per frame
	void Update () {
        PerformMapMoving();

        if (Input.GetButtonDown("ForcedAttack")) {
            forcedAttackMode = !forcedAttackMode;
        }

        if (Input.GetButtonDown("CycleUnits")) {
            unitId = (unitId + 1) % 2;
        }


        if (GUIUtility.hotControl != 0) return; //ignore mouse input if gui is active right now

        if (Input.GetKey(KeyCode.Mouse0)) { //left button to select
            if (Input.GetKeyDown(KeyCode.Mouse0))
                selectionEnd = selectionStart = Input.mousePosition;
            else  // Else we must be in "drag" mode.
                selectionEnd = Input.mousePosition;

            guiManager.selectionRect = new Rect(selectionStart, selectionEnd - selectionStart);
        } else {
            if (selectionStart != Vector2.zero) {
                if (Vector2.SqrMagnitude(selectionEnd - selectionStart) < 10) {
                    HandleSingleClickAt(selectionStart);
                } else {
                    SelectUnitsInCurrentRect();
                }

                selectionStart = selectionEnd = Vector2.zero;
                guiManager.selectionRect = new Rect(selectionStart, selectionEnd - selectionStart);
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
            if (selectedUnits.Count == 0) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                if (hitInfo.transform.gameObject.name.Equals("Ground")) {
                    Vector3 target = hitInfo.point;
                    if (forcedAttackMode) {
                        forcedAttackMode = false;
                        foreach (GameObject selectedUnit in selectedUnits) {
                            selectedUnit.GetComponent<Unit>().SetAttackTarget(hitInfo.point);
                            selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Attack);
                        }
                    } else {
                        foreach (GameObject selectedUnit in selectedUnits) {
                            selectedUnit.GetComponent<Unit>().SetMovementTarget(target);
                            selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Move);
                        }
                    }
                } else if (hitInfo.transform.gameObject.name.Contains("Unit")) {
                    foreach (GameObject selectedUnit in selectedUnits) {
                        selectedUnit.GetComponent<Unit>().SetAttackTarget(hitInfo.transform.gameObject);
                        selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Attack);
                    }
                }
            }
        }
    }

    public void HandleSingleClickAt(Vector2 position) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo)) return;
        GameObject targetObject = hitInfo.transform.gameObject;

        if (activeOrder == null) {
            if (targetObject.tag.CompareTo("unit") == 0) {
                selectedUnits = new List<GameObject> { targetObject };
                guiManager.selectedObjects = selectedUnits;
                return; //left click with no orders on unit => selection
            }
        } else {

            if (selectedUnits.Count == 0) return; //left click on ground with no selection => nothing happens

            if (targetObject.tag.CompareTo("unit") == 0) { 
                if (activeOrder == UnitOrder.Attack) {//left click on unit with attack => attack unit
                    foreach (GameObject selectedUnit in selectedUnits) {
                        selectedUnit.GetComponent<Unit>().SetAttackTarget(targetObject);
                        selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Attack);
                    }
                } else if (activeOrder == UnitOrder.Move) {//left click on unit with move => follow target
                    foreach (GameObject selectedUnit in selectedUnits) {
                        selectedUnit.GetComponent<Unit>().SetMovementTarget(targetObject);
                        selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Move);
                    }
                }
            } else {
                Vector3 targetPosition = hitInfo.point;
                if (activeOrder == UnitOrder.Attack) {//left click on ground with attack => force attack ground
                    foreach (GameObject selectedUnit in selectedUnits) {
                        selectedUnit.GetComponent<Unit>().SetAttackTarget(targetPosition);
                        selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Attack);
                    }
                } else if (activeOrder == UnitOrder.Move) {//left click on ground with move => move to location
                    foreach (GameObject selectedUnit in selectedUnits) {
                        selectedUnit.GetComponent<Unit>().SetMovementTarget(targetPosition);
                        selectedUnit.GetComponent<Unit>().SetOrder(UnitOrder.Move);
                    }
                }

            }
            activeOrder = null;
        }
    }

    //ORDER RESPONDER
    public void DidSelectMove() {
        activeOrder = UnitOrder.Move;
    }
    public void DidSelectAttack() {
        activeOrder = UnitOrder.Attack;
    }
    public void DidSelectStop() {
        activeOrder = null;
        foreach (GameObject o in selectedUnits) {
            if (o) {
                Unit u = o.GetComponent<Unit>();
                if (u.currentState.template.parametersTemplate.isControllable) { 
                    u.SetOrder(UnitOrder.Idle);
                }
            }
        }
    }



    void SelectUnitsInCurrentRect() {
        var camera = Camera.main;
        var viewportBounds =
            GetViewportBounds(camera, selectionStart, selectionEnd);

        selectedUnits.Clear();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("unit")) {
            if (viewportBounds.Contains(camera.WorldToViewportPoint(go.transform.position))) {
                selectedUnits.Add(go);
            }
        }

        guiManager.selectedObjects = selectedUnits;
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2) {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
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
