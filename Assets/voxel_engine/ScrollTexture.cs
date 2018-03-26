using UnityEngine;
using System.Collections;

public class ScrollTexture : MonoBehaviour {

	public float scrollSpeed = 0.5f;
	private float offset = 0.0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Renderer r = (Renderer)GetComponent<Renderer>();
		offset += (Time.deltaTime*scrollSpeed)/10.0f;
		r.material.SetTextureOffset ("_MainTex", new Vector2(offset, -offset));
	}
}
