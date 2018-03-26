using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockPool : MonoBehaviour {
    private static int poolSize = 500;
    private static List<GameObject> freePool = new List<GameObject>();
    private static List<GameObject> usedPool = new List<GameObject>();
    //private static int timer = 0;


    public static void AddBlock(int x, int y, int z, int color, int scale, float drag = 0.09f) {
        if(freePool.Count == 0) { return; }

        GameObject b = freePool [0];
        freePool.RemoveAt (0);
        b.GetComponent<Renderer> ().enabled = true;
        b.GetComponent<Rigidbody> ().WakeUp();
        b.GetComponent<Rigidbody> ().angularDrag = drag;
        b.transform.position = new Vector3 ((float)x, (float)y, (float)z);
        Color32	c = new Color32 ((byte)((color >> 24) & 0xFF),
                                 (byte)((color >> 16) & 0xFF),
                                 (byte)((color >> 8) & 0xFF),
                                 (byte)255);
        b.GetComponent<Renderer> ().material.color = c;
        usedPool.Add (b);
    }

    // Use this for initialization
    void Start () {
        for(int i = 0; i < poolSize; i++) {
            GameObject c = GameObject.CreatePrimitive (PrimitiveType.Cube);
            c.transform.position = new Vector3 (Random.Range(70, 150), Random.Range(0, 100), Random.Range(-10.0f, -30.0f));
            c.transform.localScale.Set (1, 1, 1);
            c.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (Vertex Color)");
            c.GetComponent<MeshRenderer>().material.EnableKeyword("_VERTEXCOLOR");
            c.GetComponent<Renderer> ().enabled = false;
            c.AddComponent<Rigidbody> ();
            c.GetComponent<Rigidbody> ().mass = 10;
            c.GetComponent<Rigidbody> ().Sleep ();
            c.GetComponent<Rigidbody> ().sleepThreshold = 1.0f;
            c.GetComponent<Rigidbody> ().angularDrag = 0.09f;
			c.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            c.GetComponent<Renderer>().material.color = new Color32 (250, 200, 100, 255);
            freePool.Add(c);
        }
    }

    private static void StopBlocks(int pos) {
        if(pos >= usedPool.Count) { return; }

        GameObject b = usedPool [pos];


        //float v = b.GetComponent<BoxCollider> ().bounds.extents.y;
        Vector3 bp = b.GetComponent<Rigidbody>().transform.position;
        float vm = b.GetComponent<Rigidbody> ().velocity.magnitude;
        //if (Physics.Raycast (b.transform.position, -Vector3.up, v + 0.1f) || bp.y <= 0 || vm < 0.3f) {
		if(vm < 0.3f) {
            //World w = FindObjectOfType<World> ();
            Color32 col = b.GetComponent<Renderer> ().material.color;

            if ((int)bp.y > 0 && (int)bp.x > 0 && (int)bp.z > 0) {
                if (World.IsWithinWorld ((int)bp.x, (int)bp.y - 1, (int)bp.z)) {
                    if ((World.blocks [(int)bp.x, (int)bp.y - 1, (int)bp.z] >> 8) != 0) {
                        World.AddBlock ((int)bp.x, (int)bp.y, (int)bp.z, ((col.r & 0xFF) << 24 | (col.g & 0xFF) << 16 | (col.b & 0xFF) << 8));
                    }
                }
            }
            b.GetComponent<Renderer> ().enabled = false;
            b.GetComponent<Rigidbody> ().Sleep ();
            freePool.Add (b);
            usedPool.RemoveAt (pos);
        } else if(!World.IsWithinWorld((int)bp.x, (int)bp.y, (int)bp.z)) {
            b.GetComponent<Renderer> ().enabled = false;
            b.GetComponent<Rigidbody> ().Sleep ();
            freePool.Add (b);
            usedPool.RemoveAt (pos);
        }
    }

    // Update is called once per frame
    void Update () {
        //		List<Block> tmp = new List<Block> ();
        //		foreach(Block p in usedPool) {
        //			if (p.alive) {
        //				p.Update ();
        //			} else {
        //				tmp.Add (p);
        //			}
        //		}
        //		foreach(Block p in tmp) {
        //			p.Reset ();
        //			freePool.Add (p);
        //			usedPool.Remove (p);
        //		}
        if (usedPool.Count > 0) {
            int max = 50;
            if (usedPool.Count < max) {
                max = usedPool.Count;
            }
            for (int i = 0; i < max; i++) {
                StopBlocks (i);
            }
        }
    }
}
