using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Unit : MonoBehaviour {

    public Rigidbody rb;

    public UnitTemplate template;
    public UnitState currentState { get; private set; }
    public float health {get; private set; }

    private List<Action> gizmoActions;

        // Use this for initialization
    void Start() {
        rb = this.GetComponent<Rigidbody>();
        gizmoActions = new List<Action>();
        
        if (template == null) {
            template = UnitTemplateFactory.defaultUnitTemplate();
        }
        currentState = new UnitState(template.initialState);
        health = currentState.template.parametersTemplate.maximumHealth;
    }

    //External interactions
    public void SetMovementTarget(Vector3 target) {
        this.nextMovementTarget = target;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void SetAttackTarget(Vector3 target) {
        if (attackTargetPosition != null && target != null &&
            Vector3.Distance((Vector3)attackTargetPosition, target) > 0.5) {
            currentState.DefaultWeapon().StartTargeting();
        }

        this.attackTargetPosition = target;
        this.attackTargetUnit = null;
        this.UpdateWeaponTargetAngles();
    }

    public void SetAttackTarget(GameObject target) {
        if (attackTargetUnit != target) { //reset targeting counter
            currentState.DefaultWeapon().StartTargeting();
        }

        attackTargetUnit = target;
        attackTargetPosition = null;
        UpdateWeaponTargetAngles();
    }

	
    //Internal mechanics
	// Update is called once per frame
	void Update () {

        if (attackTargetUnit != null) {
            UpdateWeaponTargetAngles(); //update target angles if aiming at possibly moving target or if the unit moves itself
        }

        PerformMovementJob();
        PerformAttackJob(); 
        
        if (health <= 0) {
            Debug.Log(gameObject.name + " got zero health, destroying");
            Destroy(gameObject);
        }  
    }

    private void OnDrawGizmos() {
        if (gizmoActions != null) {
            foreach (Action a in gizmoActions) {
                a();
            }
            gizmoActions.Clear();
        }
    }

}
