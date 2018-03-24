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
    }

    public void SetAttackTarget(Vector3 target) {
        if (attackTargetPosition != null && target != null &&
            Vector3.Distance((Vector3)attackTargetPosition, target) > 0.5) {
            currentState.DefaultWeapon().StartTargeting();
        }

        this.attackTargetPosition = target;
        this.UpdateWeaponTargetAngles();
    }

    public void SetAttackTarget(GameObject target) {
        if (attackTargetUnit != target) { //reset targetting counter
            currentState.DefaultWeapon().StartTargeting();
        }

        this.attackTargetUnit = target;
        this.UpdateWeaponTargetAngles();
    }

	
    //Internal mechanics
	// Update is called once per frame
	void Update () {
        PerformMovementJob();
        PerformAttackJob(); 
        
        if (this.health <= 0) {
            GameObject.Destroy(this.gameObject);
        }  
    }

    private void OnDrawGizmos() {
        foreach (Action a in gizmoActions) {
            a();
        }
        gizmoActions.Clear();
    }

}
