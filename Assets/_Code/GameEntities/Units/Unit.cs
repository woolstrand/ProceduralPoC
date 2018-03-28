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

    public int faction; //for now faction 0 is player, faction -1 is neutral, faction 1 is enemy


    private List<Action> gizmoActions;

        // Use this for initialization
    void Start() {
        rb = this.GetComponent<Rigidbody>();
        gizmoActions = new List<Action>();

        if (currentState == null) {
            InitializeInternalData();
        }

        UpdateMovementConstraints();
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

    public void SetOrder(UnitOrder order) {
        this.order = order;

        if (order == UnitOrder.Idle) {
            nextMovementTarget = transform.position;
            movementTargetObject = null;
            attackTargetUnit = null;
            attackTargetPosition = null;
            orientationTarget = null;
        }

        if (order == UnitOrder.Attack) {
            movementTargetObject = null;
        }

        if (order == UnitOrder.Move) {
            attackTargetPosition = null;
            attackTargetUnit = null;
        }

        UpdateTargetsWithinCurrentOrder();
    }

	
    //Internal mechanics
	// Update is called once per frame
	void Update () {

        if (health <= 0) {
            Debug.Log(gameObject.name + " got zero health, destroying");
            Destroy(gameObject);
        }

        if (attackTargetUnit != null) {
            UpdateWeaponTargetAngles(); //update target angles if aiming at possibly moving target or if the unit moves itself
        }

        if (order != UnitOrder.Idle) {
            UpdateTargetsWithinCurrentOrder();
        }

        PerformMovementJob();
        PerformAttackJob(); 
        
    }

    private void OnCollisionEnter(Collision collision) {
        List<EffectContainer> collisionList = currentState.template.parametersTemplate.EffectsForEvent("collision");
        if (collisionList != null) {
            foreach (EffectContainer ec in collisionList) {
                ec.ApplyEffect(collision.gameObject, collision.contacts[0].point);
            }
        }
    }

    private void OnDestroy() {
        List<EffectContainer> effectsList = currentState.template.parametersTemplate.EffectsForEvent("destruction");
        if (effectsList != null) {
            foreach (EffectContainer ec in effectsList) {
                ec.ApplyEffect(gameObject, transform.position);
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
