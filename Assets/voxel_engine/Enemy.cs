using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public static Chunk chunk;

    void Awake ()
    {
        chunk = Vox.LoadModel("models/player_stand.vox", "object");
        chunk.obj.transform.position = new Vector3 (30, 10, 100);
        chunk.obj.name = "Enemy";

        //chunk.obj.AddComponent<MeshCollider> ();
        chunk.obj.AddComponent<Rigidbody> ();
        chunk.obj.GetComponent<MeshCollider> ().convex = true;
        chunk.obj.GetComponent<Rigidbody> ().sleepThreshold = 1.0f;
        chunk.obj.GetComponent<Rigidbody> ().mass = 10;
        chunk.obj.GetComponent<Rigidbody> ().isKinematic = true;
        chunk.obj.GetComponent<Rigidbody> ().angularDrag = 0.09f;
    }

    void Update()
    {

    }

}
