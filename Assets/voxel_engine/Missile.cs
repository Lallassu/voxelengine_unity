using UnityEngine;
using System.Collections;

public class Missile {
    //	private Transform target;
    //	private float force = 2f;
    //	private bool ifTarget = false;
    private float starttime;
    public GameObject gameObject;
    private float speed = 2500f;

    public enum owners {
        PLAYER,
        ENEMY,
    };

    public enum types {
        BULLET,
        MISSILE,
    };
    public types type;
    public owners owner;

    public Missile () {
        this.gameObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
        Vector3 v = Player.player.obj.transform.position;
        v.y += 3;
        v.z += 5;
        v.x += 5;
		gameObject.name = "bullet";
        gameObject.transform.position = v;
        //gameObject.transform.localScale.Set(1, 1, 1);
        gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (Vertex Color)");
        gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_VERTEXCOLOR");
        gameObject.GetComponent<Renderer> ().enabled = true;
        gameObject.AddComponent<Rigidbody> ();
        gameObject.GetComponent<Renderer>().material.color = new Color32 (250, 200, 100, 255);
        //		target = Player.player.obj.transform;
        //		ifTarget = true;
        gameObject.transform.rotation = Player.player.obj.transform.rotation;
        //gameObject.transform.Rotate (Vector3.up*90);
        //		gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * speed * Time.deltaTime);
        gameObject.GetComponent<Rigidbody>().velocity = (gameObject.transform.forward * speed * Time.deltaTime);
        gameObject.AddComponent<MissileCollision> ();
        gameObject.GetComponent<MissileCollision> ().missile = this;

        type = types.MISSILE;
    }

    public void Explode(Collision c) {
        Vector3 pos = gameObject.transform.position;
        World.Explode ((int)pos.x, (int)pos.y, (int)pos.z, 10);
        GameObject lightGameObject = new GameObject("ExplodeLight");
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.color = Color.white;
        lightComp.type = LightType.Point;
        lightComp.range = 150;
        lightComp.shadows = LightShadows.Soft;
        lightComp.bounceIntensity = 0.0f;
        lightComp.shadowNormalBias = 2.76f;
        lightComp.intensity = 15.9f;
    	lightComp.renderMode = LightRenderMode.ForceVertex;

        lightGameObject.transform.position = new Vector3((int)pos.x+1, (int)pos.y+World.height+4, (int)pos.z+1);
        Player.tempLights.Add (lightGameObject);
    }

    // Update is called once per frame
    public void Update () {
        //		if(ifTarget){
        //			if(target == null){
        //	gameObject.GetComponent<Rigidbody>().velocity = (gameObject.transform.forward * 1500 * Time.deltaTime);
        //				Debug.Log ("Move obj");
        //
        //			} else {
        //
        //				if(Time.time > starttime){
        //					Vector3 dir = target.position - gameObject.transform.position;
        //					Quaternion rot = Quaternion.LookRotation(dir);
        //					gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rot, force * Time.deltaTime);
        //				}
        //				gameObject.GetComponent<Rigidbody>().velocity = (gameObject.transform.forward * 1500 * Time.deltaTime);
        //			}            
        //		} else {
        //			Debug.Log ("No target.");
        //		}
    }
}
