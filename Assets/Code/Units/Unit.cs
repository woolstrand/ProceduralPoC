using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    private Vector3 nextMovementTarget;
    public Rigidbody rb;

    private UnitTemplate template;
    private UnitState currentState;

        // Use this for initialization
    void Start() {
        rb = this.GetComponent<Rigidbody>();
        template = new UnitTemplate();
        currentState = new UnitState(template.initialState);
    }

    //External interactions
    public void SetTarget(Vector3 target) {
        this.nextMovementTarget = target;
    }

	
    //Internal mechanics
	// Update is called once per frame
	void Update () {

        //Arrange unit heading towards the next target point if it exists
        if (nextMovementTarget != transform.position) {
            Vector3 targetDirection = (nextMovementTarget - transform.position).normalized;
            float angle = Vector3.Angle(targetDirection, transform.forward);

            if (angle > 1) {
                Vector3 nextForward = Vector3.RotateTowards(transform.forward, targetDirection, currentState.currentMovementSettings().maxAngularSpeed * Time.deltaTime, 0);
                transform.forward = nextForward;
            } else { //rotation is acceptable for moving
                transform.forward = targetDirection; //keep orientation
                currentState.accelerateToMaximumSpeed(Time.deltaTime);
            } 

            if (currentState.speed > 0) { 

                Vector3 movement = Vector3.forward * currentState.speed * Time.deltaTime;

                if (movement.sqrMagnitude < (transform.position - nextMovementTarget).sqrMagnitude) {
                    transform.Translate(movement);
                } else {
                    transform.position = nextMovementTarget; //eliminating jitter around target
                }
            }

        }
    }

}
