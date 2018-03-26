using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		MeshFilter meshFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
		Mesh mesh = meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i].y = 0.1f * Mathf.Sin (i + (Time.fixedTime*2 + i) / 4);
		}
		mesh.vertices = vertices;
	}
}
