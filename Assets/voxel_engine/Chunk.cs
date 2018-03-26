using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk {
    public const int TYPE_WORLD = 0;
    public const int TYPE_FF = 1;
    public const int TYPE_OBJ = 2;

    public int fromX = 0;
    public int fromZ = 0;
    public int fromY = 0;
    public int toX = 0;
    public int toZ = 0;
    public int toY = 0;
    public int type = TYPE_WORLD;
    public bool dirty = false;
    public bool ffActive = false;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject obj = new GameObject();
    public int totalBlocksFF = 0;
    private int noOfCollisions = 0;

    public int[,,] blocks;

    public Chunk() {
        this.obj.AddComponent<MeshRenderer>();
        this.obj.AddComponent<MeshFilter>().mesh = new Mesh ();
        Material[] mats = new Material[1];
        mats [0] = new Material(Shader.Find ("Standard (Vertex Color)"));
        //		mats [1] = new Material(Shader.Find ("Toon/Lit"));
        //		mats [2] = new Material(Shader.Find ("Diffuse"));
        this.obj.GetComponent<MeshRenderer> ().materials = mats;
        //this.obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (Vertex Color)");
        this.obj.GetComponent<MeshRenderer>().material.EnableKeyword("_VERTEXCOLOR");
        this.obj.GetComponent<MeshRenderer> ().receiveShadows = true;
        this.obj.AddComponent<MeshCollider> ();
        //this.obj.GetComponent<MeshCollider> ().convex = true;

        this.obj.name = "WORLD_CHUNK";
    }

    public void EnablePhys() {
        this.obj.GetComponent<MeshCollider> ().convex = true;
        this.obj.AddComponent<Rigidbody> ();
        this.obj.GetComponent<Rigidbody> ().mass = 10;
        this.obj.GetComponent<Rigidbody> ().angularDrag = 0.09f;
        this.obj.AddComponent<ChunkCollision> ();
        this.obj.GetComponent<ChunkCollision> ().chunk = this;

        this.obj.name = "FF_CHUNK";
        this.blocks = new int[toX-fromX, toY-fromY, toZ-fromZ]; // +5 on all?
        this.position.Set (fromX, fromY, fromZ);
    }

    public void CollisionBounce() {
		// TBD
		if (Random.value > 0.95) {
//			Vector3 pos = this.obj.GetComponent<MeshCollider> ().transform.position;
//			for (int x = 0; x < this.toX; x++) {
//				for (int y = 0; y < this.toY; y++) {
//					for (int z = 0; z < this.toZ; z++) {
//						if (this.blocks [x, y, z] != 0) {
//							if (Random.value > 0.8) {
//								// TBD: Apply rotation.
//								BlockPool.AddBlock ((int)pos.x + x, (int)pos.y + y, (int)pos.z + z, this.blocks [x, y, z], 1, 0.001f);
//							}
//						}
//					}
//				}
//			}
			GameObject.Destroy (this.obj);
		}
	}

    public void CollisionExplode() {
        this.noOfCollisions++;
        Vector3 pos = this.obj.GetComponent<MeshCollider> ().transform.position;
        //if (this.noOfCollisions > 10 && this.blocks.Length < 300) {
            // TBD:  foreach block in chunk
            for (int x = 0; x < this.toX; x++) {
                for (int y = 0; y < this.toY; y++) {
                    for (int z = 0; z < this.toZ; z++) {
                        if (this.blocks [x, y, z] != 0) {
                            if (Random.value > 0.5) {
                                // TBD: Apply rotation.
                                BlockPool.AddBlock ((int)pos.x + x, (int)pos.y + y, (int)pos.z + z, this.blocks[x,y,z], 1, 0.001f);
                            }
                        }
                    }
                }
            }
            GameObject.Destroy (this.obj);
        //} else if(this.noOfCollisions > 10 ){
            //			// Remove random blocks
            //			int noOfBlocks = (int)Random.Range(1,this.blocks.Length/2);
            //			for(int i = 0; i < noOfBlocks; i++) {
            //				int x = (int)Random.Range (0, toX);
            //				int y = (int)Random.Range (0, toY);
            //				int z = (int)Random.Range (0, toZ);
            //				if (this.blocks [x, y, z] != 0) {
            //					this.blocks [x, y, z] = 0;
            //					BlockPool.AddBlock ((int)pos.x + x, (int)pos.y + y, (int)pos.z + z, this.blocks[x,y,z], 1, 0.001f);
            //				}
            //			}
            //			position = pos;
            //			World.RebuildChunks (this);
            //		}
    //}
}

public void EnableObject(int width, int height, int depth) {
    this.obj.name = "OBJ_CHUNK";
    this.blocks = new int[width+1, height+1, depth+1];
    this.fromX = 0;
    this.fromY = 0;
    this.fromZ = 0;
    this.toX = width;
    this.toY = height;
    this.toZ = depth;
}
}
