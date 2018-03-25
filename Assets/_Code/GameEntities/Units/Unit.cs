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

        if (health <= 0) {
            Debug.Log(gameObject.name + " got zero health, destroying");
            Destroy(gameObject);
        }

        if (attackTargetUnit != null) {
            UpdateWeaponTargetAngles(); //update target angles if aiming at possibly moving target or if the unit moves itself
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


    private void OnDrawGizmos() {
        if (gizmoActions != null) {
            foreach (Action a in gizmoActions) {
                a();
            }
            gizmoActions.Clear();
        }
    }


    //TEMPORARY STUFF. LOL, HI FROM 2018 GUYS

    private GUIStyle guiredstyle = null;
    private GUIStyle guigreenstyle = null;

    void OnGUI() {
        if (guiredstyle == null) {
            guiredstyle = new GUIStyle(GUI.skin.box);
            guiredstyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 0.5f));
            guigreenstyle = new GUIStyle(GUI.skin.box);
            guigreenstyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 0.5f));
        }        var barPos = Camera.main.WorldToScreenPoint(transform.position);        barPos.y = Screen.height - barPos.y;        float healthLength = 60.0f * health / currentState.template.parametersTemplate.maximumHealth;        var style = guigreenstyle;        if (healthLength < 20) style = guiredstyle;        GUI.Box(new Rect(barPos.x - 30, barPos.y - 50, healthLength, 5), (int)health + "/" + (int)currentState.template.parametersTemplate.maximumHealth, style);

    }

    private Texture2D MakeTex(int width, int height, Color col) {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i) {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

}
