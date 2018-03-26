using UnityEngine;
using System.Collections;

public class Block {
    private float gravity = 2.82f;
    private float mass = 0.1f;
    private float airDensity = 1.2f;
    private float e = -0.3f;
    private float area = 0.01f;
    //	private float drag = -5.95f;
    public GameObject c;
    public Transform transform;

    public bool alive = true;
    private float life = 1;
    private int bounces = 0;
    private float flip = 0;
    private float vy = 0;
    private float avg_ay = 0;
    private float vx = 0;
    private float vz = 0;
    private float avg_ax = 0;
    private float avg_az = 0;

    private float fx_ = 0;
    private float fz_ = 0;

    private float power = 1;

    // Use this for initialization
    public Block() {
        c = GameObject.CreatePrimitive (PrimitiveType.Cube);
        c.transform.position = new Vector3 (Random.Range(70, 150), Random.Range(0, 100), Random.Range(-10.0f, -30.0f));
        c.transform.localScale.Set (1, 1, 1);
        c.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (Vertex Color)");
        c.GetComponent<MeshRenderer>().material.EnableKeyword("_VERTEXCOLOR");
        c.GetComponent<Renderer> ().enabled = false;
        c.GetComponent<Renderer>().material.color = new Color32 (250, 200, 100, 255);
        transform = c.transform;
        Reset ();
    }

    public void Reset() {
        life = 1;
        bounces = 0;
        alive = true;
        fx_ = Random.Range (-0.5f, 0.5f);
        fz_ = Random.Range (-0.5f, 0.5f);
        life = Random.Range (1, 4);
        bounces = Random.Range (1, 5);
        vx = power;
        vy = power;
        vz = power;
        c.GetComponent<Renderer> ().enabled = false;
    }

    public void Update () {
        if(life <= 0 || bounces <= 0 || transform.position.y <= 0) {
            alive = false;
        } else {
            life -= Time.deltaTime;
            if (flip < 0) {
                flip = -0.5f;
            } else {
                flip = 0.5f;
            }
            float fy = mass * gravity;
            float fx = mass * gravity * flip;
            float fz = mass * gravity * flip;
            fy += -1.0f * 0.5f * airDensity * area * vy * vy;
            fx += -1.0f * 0.5f * airDensity * area * vx * vx;
            fz += -1.0f * 0.5f * airDensity * area * vz * vz;
            float dy = vy * Time.deltaTime + (0.5f * avg_ay * Time.deltaTime * Time.deltaTime);
            float dx = vx * Time.deltaTime + (0.5f * avg_ax * Time.deltaTime * Time.deltaTime);
            float dz = vz * Time.deltaTime + (0.5f * avg_az * Time.deltaTime * Time.deltaTime);
            Vector3 curr = transform.position;
            transform.position = new Vector3 (curr.x + (10 * dx * fx_), curr.y + (10* dy * fy), curr.z + (10* dz * fz_));

            float new_ay = fy / mass;
            avg_ay = 0.5f * (new_ay + avg_ay);
            vy -= avg_ay * Time.deltaTime;

            float new_ax = fx / mass;
            avg_ax = 0.5f * (new_ax + avg_ax);
            vx -= avg_ax * Time.deltaTime;

            float new_az = fz / mass;
            avg_az = 0.5f * (new_az + avg_az);
            vz -= avg_az * Time.deltaTime;

            transform.rotation = Quaternion.Euler (vx, vy, vz);

            int x = (int)transform.position.x;
            int y = (int)transform.position.y;
            int z = (int)transform.position.z;
            if (World.IsWithinWorld (x, y, z)) {
                if ((World.blocks [x, y, z] >> 8) != 0) {
                    c.GetComponent<Renderer> ().enabled = false;
                    alive = false;
                    life = 0;
                } else if ((World.blocks [x, y, z] >> 8) != 0 && vx < 0) {
                    transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
                    vx *= e;
                    bounces--;
                } else if ((World.blocks [x, y, z] >> 8) != 0 && vz < 0) {
                    transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
                    vz *= e;
                    bounces--;
                } else if ((World.blocks [x, y, z] >> 8) != 0 && vy < 0) {
                    transform.position = new Vector3 (transform.position.x, y + 4, transform.position.z);
                    transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
                    vy *= e;
                    bounces--;
                }
            }
        }
    }
}
