using UnityEngine;
using System.Collections;

public class ChunkCollision : MonoBehaviour {

    public Chunk chunk;

    // Use this for initialization
    void Start () {
    }

    void OnCollisionEnter(Collision c) {
		if (c.gameObject.name == "bullet") {
			chunk.CollisionExplode ();
		} else {
			// Bounce, remove some blocks only.
			chunk.CollisionBounce();
		}
    }

    // Update is called once per frame
    void Update () {
        if(chunk.obj.transform.position.y < -30) {
            GameObject.Destroy (chunk.obj);
        }

    }
}
