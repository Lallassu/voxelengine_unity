using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;

public class Player : MonoBehaviour {
    public static Chunk player;
    //public GameObject camera;
    public GameObject jet; 
//    private GameObject playerJet1;
//    private GameObject playerJet2;
    public static List<GameObject> tempLights = new List<GameObject>();
    private static List<Missile> missiles = new List<Missile>();

    // Controls
    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode jump = KeyCode.Space;

	private Light pLight;
	private Light pUnder;

    private float speedR = 2.0f;

    private int y = 10;

    void Awake () {
        player = Vox.LoadModel("Assets/models/player_stand.vox", "object");
        player.obj.transform.position = new Vector3 (150, 11, 150);
        player.obj.name = "Player";

		GameObject lightGameObject = new GameObject("LightAbovePlayer");
    	pLight = lightGameObject.AddComponent<Light>();
		pLight.type = LightType.Point;
		pLight.color = Color.grey;
		pLight.range = 200;
		pLight.intensity = 1.0f;
		pLight.renderMode = LightRenderMode.ForceVertex;

		GameObject lightGameObject2 = new GameObject("LightBelowPlayer");
		pUnder = lightGameObject2.AddComponent<Light>();
		pUnder.type = LightType.Point;
		pUnder.color = Color.white;
		pUnder.range = 200;
		pUnder.intensity = 3.4f;
		pUnder.renderMode = LightRenderMode.ForceVertex;

        //player.obj.AddComponent<MeshCollider> ();
        player.obj.AddComponent<Rigidbody> ();
        player.obj.GetComponent<MeshCollider> ().convex = true;
        player.obj.GetComponent<Rigidbody> ().sleepThreshold = 1.0f;
        player.obj.GetComponent<Rigidbody> ().mass = 10;
        player.obj.GetComponent<Rigidbody> ().isKinematic = true;
        player.obj.GetComponent<Rigidbody> ().angularDrag = 0.09f;

        // Add player as target for the smoothfollow camera script.
       //SmoothFollow sf = Camera.main.GetComponent<SmoothFollow> ();
		//SmoothCamera sf = Camera.main.GetComponent<SmoothCamera>();
		SmoothFollow script = Camera.main.GetComponent<SmoothFollow>();
		script.target = player.obj.transform; 
		//sf.target = player.obj.transform;
		//GameObject.Find("Camera").GetComponent("SmoothFollow").target = player.obj.transform;

//        playerJet1 = (GameObject)Instantiate(jet, player.obj.transform.position, Quaternion.identity);
//        playerJet1.transform.parent = player.obj.transform;
//        playerJet1.transform.localRotation = Quaternion.Euler (new Vector3(90, 0, 0));
//        playerJet1.transform.localPosition = new Vector3 (0.5f, 1.89f, -0.7f);
//        playerJet1.GetComponent<ParticleSystem> ().loop = false;
//        playerJet1.GetComponent<ParticleSystem> ().startSize = 5;
//        playerJet1.GetComponent<ParticleSystem> ().Pause ();
//
//
//        playerJet2 = (GameObject)Instantiate(jet, player.obj.transform.position, Quaternion.identity);
//        playerJet2.transform.parent = player.obj.transform;
//        playerJet2.transform.localRotation = Quaternion.Euler (new Vector3(90, 0, 0));
//        playerJet2.transform.localPosition = new Vector3 (2.5f, 1.89f, -0.7f);
//        playerJet2.GetComponent<ParticleSystem> ().loop = false;
//        playerJet2.GetComponent<ParticleSystem> ().startSize = 5;
//        playerJet2.GetComponent<ParticleSystem> ().Pause ();

    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        pLight.transform.position = new Vector3 (player.obj.transform.position.x, y+20, player.obj.transform.position.z);
		pUnder.transform.position = new Vector3 (player.obj.transform.position.x, World.floorHeight, player.obj.transform.position.z);

        if (Input.GetKeyDown ("escape")) {
            Cursor.lockState = CursorLockMode.Confined;
        }

        float speed = 70;

        if (Input.GetButtonDown("Fire1")) {
            Missile m = new Missile ();
            m.owner = Missile.owners.PLAYER;
            missiles.Add (m);
            Cursor.lockState = CursorLockMode.Locked;
            //			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //			RaycastHit[] hits = Physics.RaycastAll (ray);
            //			for(int i = 0; i < hits.Length; i++) {
            //				Vector3 pos = ray.GetPoint (150f);
            //				World.Explode ((int)pos.x, (int)pos.y, (int)pos.z, 10);
            //				pos.y -= 1.0f;
            //				//Instantiate(explosion, pos, Quaternion.identity);
            //				GameObject lightGameObject = new GameObject("StreetLight");
            //				Light lightComp = lightGameObject.AddComponent<Light>();
            //				lightComp.color = Color.white;
            //				lightComp.type = LightType.Point;
            //				lightComp.range = 100;
            //				lightComp.shadows = LightShadows.Soft;
            //				lightComp.bounceIntensity = 0.0f;
            //				lightComp.shadowNormalBias = 2.76f;
            //				lightComp.intensity = 5.9f;
            //				lightGameObject.transform.position = new Vector3((int)pos.x+1, (int)pos.y+World.height+4, (int)pos.z+1);
            //				tempLights.Add (lightGameObject);
            //				//break;
            //			}

        }

        foreach(Missile l in missiles) {
            l.Update ();
        }

        // Fade out explode lights
        List<GameObject> tmp = new List<GameObject>();
        foreach(GameObject l in tempLights) {
            l.GetComponent<Light> ().intensity -= 0.8f;
            if(l.GetComponent<Light> ().intensity <= 0) {
                tmp.Add(l);
            }
        }

        bool move = false;

        foreach(GameObject t in tmp) {
			Destroy (t);
            tempLights.Remove (t);
        }

        if(Input.GetKey(forward))
        {
//            playerJet1.transform.localRotation = Quaternion.Euler (new Vector3(180, 0, 0));
//            playerJet2.transform.localRotation = Quaternion.Euler (new Vector3(180, 0, 0));
            move = true;
            player.obj.transform.Translate(Vector3.forward * Time.deltaTime*speed);
            //player.obj.GetComponent<Rigidbody> ().AddTorque (Vector3.forward * Time.deltaTime * speed*100);
        }

        if(Input.GetKey(left))
        {
//            playerJet1.transform.localRotation = Quaternion.Euler (new Vector3 (60, 60, 0));
//            playerJet2.transform.localRotation = Quaternion.Euler (new Vector3 (60, 60, 0));
            move = true;
            player.obj.transform.Translate(Vector3.left * Time.deltaTime*speed);
        }

        if (Input.GetKey (backward)) {
//            playerJet1.transform.localRotation = Quaternion.Euler (new Vector3 (40, 0, 0));
//            playerJet2.transform.localRotation = Quaternion.Euler (new Vector3 (40, 0, 0));
            move = true;
            player.obj.transform.Translate(Vector3.back * Time.deltaTime*speed);
        }

        if(Input.GetKey(right))
        {
//            playerJet1.transform.localRotation = Quaternion.Euler (new Vector3 (60, -60, 0));
//            playerJet2.transform.localRotation = Quaternion.Euler (new Vector3 (60, -60, 0));
            move = true;
            player.obj.transform.Translate(Vector3.right * Time.deltaTime*speed);
        }
        if(!move) {
//            playerJet1.transform.localRotation = Quaternion.Euler (new Vector3 (90, 0, 0));
//            playerJet2.transform.localRotation = Quaternion.Euler (new Vector3 (90, 0, 0));
        }
        if (Input.GetKey (jump)) {
//            playerJet1.GetComponent<ParticleSystem> ().Play ();
//            playerJet2.GetComponent<ParticleSystem> ().Play ();
            if (y < 16) {
//                playerJet1.GetComponent<ParticleSystem> ().startLifetime = 0.8f;
//                playerJet2.GetComponent<ParticleSystem> ().startLifetime = 0.8f;
//                playerJet1.GetComponent<ParticleSystem> ().startSize = 15;
//                playerJet2.GetComponent<ParticleSystem> ().startSize = 15;
            } else {
//                playerJet1.GetComponent<ParticleSystem> ().startLifetime = 0.3f;
//                playerJet2.GetComponent<ParticleSystem> ().startLifetime = 0.3f;
//                playerJet1.GetComponent<ParticleSystem> ().startSize = 5;
//                playerJet2.GetComponent<ParticleSystem> ().startSize = 5;
            }
            if (y < 100) {
                y += 1;
            }
        } else {
			if(y > World.floorHeight) {
                y -= 1;
//                playerJet1.transform.localRotation = Quaternion.Euler (new Vector3 (90, 0, 0));
//                playerJet2.transform.localRotation = Quaternion.Euler (new Vector3 (90, 0, 0));
//                playerJet1.GetComponent<ParticleSystem> ().startLifetime = 0.05f;
//                playerJet2.GetComponent<ParticleSystem> ().startLifetime = 0.05f;
            } else {
//                playerJet1.GetComponent<ParticleSystem> ().Stop ();
//                playerJet2.GetComponent<ParticleSystem> ().Stop ();
            }
        }
        player.obj.transform.position = new Vector3 (player.obj.transform.position.x, y, player.obj.transform.position.z);
        float h = speedR * Input.GetAxis("Mouse X");
        player.obj.transform.Rotate(0, h, 0); // TBD: Rotate around center of object.
    }
}
