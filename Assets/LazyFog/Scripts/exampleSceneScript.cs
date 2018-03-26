using UnityEngine;
using System.Collections;

public class exampleSceneScript : MonoBehaviour {

	// Use this for initialization
	
	public float scale = 0.6f;
	public float intensity = 0.8f;
	public float alpha = 0.45f;
	public float alphasub = 0.05f;
	public float pow = 1.2f;
	public Color color = new Color(1f, 0.95f, 0.9f, 1.0f);
	public Material fogMaterial;
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		fogMaterial.SetFloat("_Scale", scale);
		fogMaterial.SetFloat("_Intensity", intensity);
		fogMaterial.SetFloat("_Alpha", alpha);
		fogMaterial.SetFloat("_AlphaSub", alphasub);
		fogMaterial.SetFloat("_Pow", pow);
		fogMaterial.SetColor("_Color", color);
		
	}
	
	void OnGUI () {
		float dy = 25;
		float y = 1;
		float x2 = 200;
		
		GUI.Label(new Rect(25,dy*y,100,30), "Scale");
		scale = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), scale, 0.0f, 5.0f);
		
		GUI.Label(new Rect(25,dy*y,100,30), "Intensity");
		intensity = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), intensity, 0.0f, 1.0f);
		
		GUI.Label(new Rect(25,dy*y,100,30), "Alpha");
		alpha = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), alpha, 0.0f, 2.5f);
		
		GUI.Label(new Rect(25,dy*y,100,30), "AlphaSub");
		alphasub = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), alphasub, 0.0f, 1.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Pow");
		pow = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), pow, 0.0f, 4.0f);

		GUI.Label(new Rect(25,dy*y,100,30), "Red");
		color.r = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), color.r, 0.0f, 1.0f);
		
		GUI.Label(new Rect(25,dy*y,100,30), "Green");
		color.g = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), color.g, 0.0f, 1.0f);
		
		GUI.Label(new Rect(25,dy*y,100,30), "Blue");
		color.b = GUI.HorizontalSlider (new Rect (x2, dy*y++, 100, 30), color.b, 0.0f, 1.0f);
		
	}			
	
}
