using UnityEngine;
using System.Collections;

public class MissileCollision : MonoBehaviour {
    public Missile missile = null;
    private GameObject missile_explosion;

    void Start () {
        //missile_explosion = GameObject.FindWithTag ("Explosion");
    }

    void OnCollisionEnter(Collision c) {
        if (c.collider.name == "Player" && missile.owner == Missile.owners.PLAYER) {
            // Dont hit itself.
            return;
        }
        if (missile.type == Missile.types.MISSILE) {
            //				missile_explosion = GameObject.FindWithTag ("Explosion");
            //			    Vector3 p = missile.gameObject.transform.position;
            //		     	missile_explosion.transform.position = p;
            //				missile_explosion.GetComponent<Detonator> ().Reset ();
            //				missile_explosion.GetComponent<Detonator> ().enabled = true;
            //				missile_explosion.GetComponent<Detonator> ().Explode ();
        }
        missile.Explode (c);
        GameObject.Destroy (missile.gameObject);
    }

    // Update is called once per frame
    void Update () {
        if (missile != null) {
            Vector3 pos = missile.gameObject.transform.position;
            if (!World.IsWithinWorld ((int)pos.x, (int)pos.y, (int)pos.z)) {
                GameObject.Destroy (missile.gameObject);
            }
        }
    }
}
