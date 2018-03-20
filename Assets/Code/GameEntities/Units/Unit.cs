using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Unit : MonoBehaviour {

    public Rigidbody rb;

    public UnitTemplate template;
    public UnitState currentState { get; private set; }

        // Use this for initialization
    void Start() {
        rb = this.GetComponent<Rigidbody>();
        if (template == null) {
            template = UnitTemplateFactory.defaultUnitTemplate();
        }
        currentState = new UnitState(template.initialState);
    }

    //External interactions
    public void SetMovementTarget(Vector3 target) {
        this.nextMovementTarget = target;
    }

    public void SetAttackTarget(Vector3 target) {
        if (attackTargetPosition != null && target != null &&
            Vector3.Distance(attackTargetPosition, target) > 0.5) {
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
    }

}
