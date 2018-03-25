using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffectVisualizer : MonoBehaviour {

    public float ttl;
    public float radius;

    private float creationTime;

	// Use this for initialization
	void Start () {
        gameObject.transform.localScale = new Vector3(radius * 2, 0.01f, radius * 2); //object diameter is 1.0 => scale = radius * 2
        creationTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float age = Time.time - creationTime;

        if (age > ttl) {
            Destroy(gameObject);
        }

        gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 1.0f - age / ttl);
	}
}
