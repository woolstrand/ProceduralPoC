using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public partial class Unit : MonoBehaviour {

    public Rigidbody rb;

    public UnitTemplate template;
    public UnitState currentState { get; private set; }
    public float health {get; private set; }

    public int faction; //for now faction 0 is player, faction -1 is neutral, faction 1 is enemy. it's pretty fun to have faction attached to a certain state, but, well, not today.


    private List<Action> gizmoActions;

        // Use this for initialization
    void Start() {
        rb = this.GetComponent<Rigidbody>();
        gizmoActions = new List<Action>();

        if (currentState == null) {
            InitializeInternalData();
        }

        StartEffects();
        UpdateMovementConstraints();
        SetOrder(UnitOrder.Idle);
    }

    public void InitializeInternalData() {
        if (template == null) {
            template = UnitTemplateFactory.defaultUnitTemplate();
        }

        currentState = new UnitState(template.initialState);
        health = currentState.template.parametersTemplate.maximumHealth;
        nextMovementTarget = transform.position;
    }

    public void UpdateMovementConstraints() {
        if (currentState.currentMovementSettings().isLockedVertically) {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        } else {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    //External interactions
    public void SetMovementTarget(Vector3 target) {
        nextMovementTarget = target;
    }

    public void SetMovementTarget(GameObject target) {
        movementTargetObject = target;
    }

    public void SetAttackTarget(Vector3 target) {
        if (attackTargetPosition != null && 
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

    public void SetOrder(UnitOrder order) {
        DoSetOrder(order);
    }

	
    //Internal mechanics
	// Update is called once per frame
	void Update () {

        if (health <= 0) {
            Die();
        }

        if (attackTargetUnit != null || autoTarget != null) {
            UpdateWeaponTargetAngles(); //update target angles if aiming at possibly moving target or if the unit moves itself
        }

        if (order != UnitOrder.Idle) {
            UpdateTargetsWithinCurrentOrder();
        } else {
            if (autoTarget != null) {
                Vector3 direction = autoTarget.transform.position - transform.position;
                AdjustOrientationForAttack(direction);
            }
        }

        PerformMovementJob();
        PerformAttackJob();
        UpdateEffectsState();


    }

    //we use die to distinguish between system "on destroy" and in-game "on die"
    private void Die() {
        List<EffectContainer> effectsList = currentState.template.parametersTemplate.EffectsForEvent("destruction");
        if (effectsList != null) {
            foreach (EffectContainer ec in effectsList) {
                ec.ApplyEffect(gameObject, transform.position);
            }
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        List<EffectContainer> collisionList = currentState.template.parametersTemplate.EffectsForEvent("collision");
        if (collisionList != null) {
            foreach (EffectContainer ec in collisionList) {
                if (ec.target != ContainerTarget.Self) { //apply to collider
                    ec.ApplyEffect(collision.gameObject, collision.contacts[0].point);
                }
                if (ec.target != ContainerTarget.Other) { //apply to self TODO: limit AOE overdrive
                    ec.ApplyEffect(gameObject, collision.contacts[0].point);
                }
            }
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
