using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffectVisualizer : MonoBehaviour {

    public float ttl;
    public float radius;

    private float creationTime;

	// Use this for initialization
	void Start () {
        gameObject.transform.localScale = new Vector3(radius * 2, 0.01f, radius * 2); //object initial diameter is 1.0 => scale = radius * 2

        Material material = gameObject.GetComponent<MeshRenderer>().material;
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        material.SetFloat("_Mode", 4f);

        creationTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float age = Time.time - creationTime;

        if (age > ttl) {
            Destroy(gameObject);
            return;
        }

        gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.5f - age / ttl);
	}
}
