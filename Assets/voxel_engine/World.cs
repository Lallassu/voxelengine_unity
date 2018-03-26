using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;

// TBD: Singleton
public class World : MonoBehaviour {

	public static int floorHeight = 22; // Where to floor is in the world "height".
	public static int width = 30; // Number of chunks (width * chunkSize = actual world size = rWidth)
	public static int height = 2;
    public static int depth = 30; 
    public static int chunkSize = 32;
    public static int rWidth = width * chunkSize;
    public static int rDepth = depth * chunkSize;
    public static int rHeight = height * chunkSize;
    private static int blockSize = 1;
    private static bool started = false;


    private	static List<List<Vector3>> floodFill = new List<List<Vector3>>();

    public static int[,,] blocks; //  = new int[width*chunkSize, height*chunkSize, depth*chunkSize];
    public static Chunk[,,] chunks; // = new Chunk[width+1, height+1, depth+1];

    private static List<Chunk> rebuildList = new List<Chunk>();

    public static void RebuildChunks(Chunk chunk) {
        int sides = 0;
        List<Vector3> vertices = new List<Vector3> ();
        List<Color32> colors = new List<Color32> ();
        List<int> tri = new List<int> ();
        int[,,] blocks_;

        if(chunk.type == Chunk.TYPE_OBJ || chunk.type == Chunk.TYPE_FF) {
            blocks_ = chunk.blocks;
        } else {
            blocks_ = blocks;
        }

        // Block structure
        // BLOCK: [R-color][G-color][B-color][0][00][back_left_right_above_front]
        //           8bit    8bit     8it    1bit(below-face)  2bit(floodfill)     5bit(faces)

        // Reset faces
        int chunkYorg = chunk.toY;
        int chunkMaxY = chunk.toY;
        for (int y = chunk.fromY; y < chunk.toY; y++) {
            bool empty = true;
            for (int x = chunk.fromX; x < chunk.toX; x++) {
                for (int z = chunk.fromZ; z < chunk.toZ; z++) {
                    if (blocks_ [x, y, z] != 0) {
                        blocks_ [x, y, z] &= ~(1 << 0);
                        blocks_ [x, y, z] &= ~(1 << 1);
                        blocks_ [x, y, z] &= ~(1 << 2);
                        blocks_ [x, y, z] &= ~(1 << 3);
                        blocks_ [x, y, z] &= ~(1 << 4);
                        blocks_ [x, y, z] &= ~(1 << 7);
                        empty = false;
                    }
                }
            }
            if(empty) {
                chunkMaxY = y;
            }
        }

        //		if (chunk.ffActive) {
        //		if(chunk.type == Chunk.TYPE_WORLD){
        //			Debug.Log ("CHUNK MAX Y: " + chunkMaxY);
        //			chunk.toY = chunkMaxY;
        //		}


		// TBD: Fix indentation with vim, since monodevelop makes it fubar.
        for (int y = chunk.fromY; y < chunk.toY; y++) {
            for (int x = chunk.fromX; x < chunk.toX; x++) {
                for (int z = chunk.fromZ; z < chunk.toZ; z++) {
                    if ((blocks_ [x, y, z] >> 8) == 0) {
                        continue; // Skip empty blocks_
                    }
                    // Check if hidden
                    int left = 0, right = 0, above = 0, front = 0, back = 0, below = 0;
                    if (z > 0) {
                        if (blocks_ [x, y, z - 1] != 0) { 
                            back = 1;
                            blocks_ [x, y, z] = blocks_ [x, y, z] | 0x10;
                        }
                    }
                    if (x > 0) {
                        if (blocks_ [x - 1, y, z] != 0) {
                            left = 1;
                            blocks_ [x, y, z] = blocks_ [x, y, z] | 0x8;
                        }
                    }
					if (chunk.type == Chunk.TYPE_OBJ || chunk.type == Chunk.TYPE_FF) {
                        if(y > chunk.toY ) {
                            if (blocks_ [x, y - 1, z] != 0) {
                                below = 1;
                                blocks_ [x, y - 1, z] = blocks_ [x, y, z] | 0x80;
                            }
                        }
                        if(x < chunk.toX-1) {
                            if (blocks_ [x + 1, y, z] != 0) {
                                right = 1;
                                blocks_ [x, y, z] = blocks_ [x, y, z] | 0x4;
                            }
                        }
                    } else {
                        below = 1;
                        if (x < (width * chunkSize) - 1) {
                            if (blocks_ [x + 1, y, z] != 0) {
                                right = 1;
                                blocks_ [x, y, z] = blocks_ [x, y, z] | 0x4;
                            }
                        }
                    }
                    if (y < chunk.toY - 1) {
                        if (blocks_ [x, y + 1, z] != 0) {
                            above = 1;
                            blocks_ [x, y, z] = blocks_ [x, y, z] | 0x2;
                        }
                    }
                    if (chunk.type == Chunk.TYPE_OBJ || chunk.type == Chunk.TYPE_FF) {
                        if(z < chunk.toZ - 1) {
                            if (blocks_ [x, y, z + 1] != 0) {
                                front = 1;
                                blocks_ [x, y, z] = blocks_ [x, y, z] | 0x1;
                            }
                        }	
                    } else {
                        if (z < (chunkSize * depth) - 1) {
                            if (blocks_ [x, y, z + 1] != 0) {
                                front = 1;
                                blocks_ [x, y, z] = blocks_ [x, y, z] | 0x1;
                            }
                        }
                    }

                    if (front == 1 && left == 1 && right == 1 && above == 1 && back == 1 && below == 1) {
                        // If we are building a standalone mesh, remove invisible
                        if (chunk.type == Chunk.TYPE_OBJ || chunk.type == Chunk.TYPE_FF) {
                            blocks_ [x, y, z] = 0;
                        }
                        continue; // block is hidden
                    }
                    // Draw block
                    if(below == 0) {
                        if((blocks_[x,y,z] & 0x80) == 0) {
                            int maxX = 0;
                            int maxZ = 0;

                            for(int x_ = x; x_ < chunk.toX; x_++) {
                                // Check not drawn + same color
                                if ((blocks_ [x_, y, z] & 0x80) == 0 && SameColor (blocks_ [x_, y, z], blocks_ [x, y, z])) {
                                    maxX++;
                                } else {
                                    break;
                                }
                                int tmpZ = 0;
                                for (int z_ = z; z_ < chunk.toZ; z_++) {
                                    if ((blocks_ [x_, y, z_] & 0x80) == 0 && SameColor (blocks_ [x_, y, z_], blocks_ [x, y, z])) {
                                        tmpZ++;
                                    } else {
                                        break;
                                    }
                                }
                                if (tmpZ < maxZ || maxZ == 0) {
                                    maxZ = tmpZ;
                                }
                            }
                            for (int x_ = x; x_ < x + maxX; x_++) {
                                for (int z_ = z; z_ < z + maxZ; z_++) {
                                    blocks_ [x_, y, z_] = blocks_ [x_, y, z_] | 0x80;
                                }
                            }
                            maxX--;
                            maxZ--;

                            int idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize- blockSize, z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3 (x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize - blockSize));
                            vertices.Add ( new Vector3(x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize + (blockSize * maxZ)) );

                            // Add triangle indeces
                            tri.Add(idx+2);
                            tri.Add(idx+1);
                            tri.Add(idx);

                            idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize-blockSize, z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize - blockSize, z * blockSize - blockSize ));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize - blockSize ));

                            tri.Add(idx+2);
                            tri.Add(idx+1);
                            tri.Add(idx);

                            sides += 6;
                            for (int n = 0; n < 6; n++) {
                                colors.Add (new Color32((byte)((blocks_ [x, y, z] >> 24) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 16) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 8) & 0xFF),
                                                        (byte)255
                                                       ));
                            }
                        }
                    }

                    if (above == 0) {
                        // Get above (0010)
                        if ((blocks_ [x, y, z] & 0x2) == 0) {
                            int maxX = 0;
                            int maxZ = 0;

                            for (int x_ = x; x_ < chunk.toX; x_++) {
                                // Check not drawn + same color
                                if ((blocks_ [x_, y, z] & 0x2) == 0 && SameColor (blocks_ [x_, y, z], blocks_ [x, y, z])) {
                                    maxX++;
                                } else {
                                    break;
                                }
                                int tmpZ = 0;
                                for (int z_ = z; z_ < chunk.toZ; z_++) {
                                    if ((blocks_ [x_, y, z_] & 0x2) == 0 && SameColor (blocks_ [x_, y, z_], blocks_ [x, y, z])) {
                                        tmpZ++;
                                    } else {
                                        break;
                                    }
                                }
                                if (tmpZ < maxZ || maxZ == 0) {
                                    maxZ = tmpZ;
                                }
                            }
                            for (int x_ = x; x_ < x + maxX; x_++) {
                                for (int z_ = z; z_ < z + maxZ; z_++) {
                                    blocks_ [x_, y, z_] = blocks_ [x_, y, z_] | 0x2;
                                }
                            }
                            maxX--;
                            maxZ--;

                            int idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize, z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3 (x * blockSize - blockSize, y * blockSize, z * blockSize - blockSize));
                            vertices.Add ( new Vector3(x * blockSize - blockSize, y * blockSize, z * blockSize + (blockSize * maxZ)) );

                            // Add triangle indeces
                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize, z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize, z * blockSize - blockSize ));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize, z * blockSize - blockSize ));


                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            sides += 6;
                            for (int n = 0; n < 6; n++) {
                                colors.Add (new Color32((byte)((blocks_ [x, y, z] >> 24) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 16) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 8) & 0xFF),
                                                        (byte)255
                                                       ));
                            }
                        }
                    }
                    if (back == 0) {
                        // back  10000
                        if ((blocks_ [x, y, z] & 0x10) == 0) {
                            int maxX = 0;
                            int maxY = 0;

                            for (int x_ = x; x_ < chunk.toX; x_++) {
                                // Check not drawn + same color
                                if ((blocks_ [x_, y, z] & 0x10) == 0 && SameColor (blocks_ [x_, y, z], blocks_ [x, y, z])) {
                                    maxX++;
                                } else {
                                    break;
                                }
                                int tmpY = 0;
                                for (int y_ = y; y_ < chunk.toY; y_++) {
                                    if ((blocks_ [x_, y_, z] & 0x10) == 0 && SameColor (blocks_ [x_, y_, z], blocks_ [x, y, z])) {
                                        tmpY++;
                                    } else {
                                        break;
                                    }
                                }
                                if (tmpY < maxY || maxY == 0) {
                                    maxY = tmpY;
                                }
                            }
                            for (int x_ = x; x_ < x + maxX; x_++) {
                                for (int y_ = y; y_ < y + maxY; y_++) {
                                    blocks_ [x_, y_, z] = blocks_ [x_, y_, z] | 0x10;
                                }
                            }
                            maxX--;
                            maxY--;

                            int idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize + (blockSize * maxY), z * blockSize - blockSize));
                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize - blockSize, z * blockSize - blockSize));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize - blockSize ));

                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            idx = vertices.Count;


                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize + (blockSize * maxY), z * blockSize - blockSize));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize - blockSize ));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize + (blockSize * maxY), z * blockSize - blockSize));

                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            sides += 6;
                            for (int n = 0; n < 6; n++) {
                                colors.Add (new Color32((byte)((blocks_ [x, y, z] >> 24) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 16) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 8) & 0xFF),
                                                        (byte)255
                                                       ));
                            }
                        }
                    }
                    if (front == 0) {
                        // front 0001
                        if ((blocks_ [x, y, z] & 0x1) == 0) {
                            int maxX = 0;
                            int maxY = 0;

                            for (int x_ = x; x_ < chunk.toX; x_++) {
                                // Check not drawn + same color
                                if ((blocks_ [x_, y, z] & 0x1) == 0 && SameColor (blocks_ [x_, y, z], blocks_ [x, y, z])) {
                                    maxX++;
                                } else {
                                    break;
                                }
                                int tmpY = 0;
                                for (int y_ = y; y_ < chunk.toY; y_++) {
                                    if ((blocks_ [x_, y_, z] & 0x1) == 0 && SameColor (blocks_ [x_, y_, z], blocks_ [x, y, z])) {
                                        tmpY++;
                                    } else {
                                        break;
                                    }
                                }
                                if (tmpY < maxY || maxY == 0) {
                                    maxY = tmpY;
                                }
                            }
                            for (int x_ = x; x_ < x + maxX; x_++) {
                                for (int y_ = y; y_ < y + maxY; y_++) {
                                    blocks_ [x_, y_, z] = blocks_ [x_, y_, z] | 0x1;
                                }
                            }
                            maxX--;
                            maxY--;

                            int idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize + (blockSize * maxX), y * blockSize + (blockSize * maxY), z * blockSize));
                            vertices.Add (new Vector3( x * blockSize - blockSize, y * blockSize + (blockSize * maxY), z * blockSize ));
                            vertices.Add (new Vector3( x * blockSize + (blockSize * maxX), y * blockSize - blockSize, z * blockSize ));

                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            idx = vertices.Count;
                            vertices.Add (new Vector3( x * blockSize - blockSize, y * blockSize + (blockSize * maxY), z * blockSize ));
                            vertices.Add (new Vector3( x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize ));
                            vertices.Add (new Vector3( x * blockSize + (blockSize * maxX), y * blockSize - blockSize, z * blockSize ));

                            // Add triangle indeces
                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            sides += 6;
                            for (int n = 0; n < 6; n++) {
                                colors.Add (new Color32((byte)((blocks_ [x, y, z] >> 24) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 16) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 8) & 0xFF),
                                                        (byte)255
                                                       ));
                            }
                        }
                    }
                    if (left == 0) {
                        if ((blocks_ [x, y, z] & 0x8) == 0) {
                            int maxZ = 0;
                            int maxY = 0;

                            for (int z_ = z; z_ < chunk.toZ; z_++) {
                                // Check not drawn + same color
                                if ((blocks_ [x, y, z_] & 0x8) == 0 && SameColor (blocks_ [x, y, z_], blocks_ [x, y, z])) {
                                    maxZ++;
                                } else {
                                    break;
                                }
                                int tmpY = 0;
                                for (int y_ = y; y_ < chunk.toY; y_++) {
                                    if ((blocks_ [x, y_, z_] & 0x8) == 0 && SameColor (blocks_ [x, y_, z_], blocks_ [x, y, z])) {
                                        tmpY++;
                                    } else {
                                        break;
                                    }
                                }
                                if (tmpY < maxY || maxY == 0) {
                                    maxY = tmpY;
                                }
                            }
                            for (int z_ = z; z_ < z + maxZ; z_++) {
                                for (int y_ = y; y_ < y + maxY; y_++) {
                                    blocks_ [x, y_, z_] = blocks_ [x, y_, z_] | 0x8;
                                }
                            }
                            maxZ--;
                            maxY--;

                            int idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize - blockSize ));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize + (blockSize * maxY), z * blockSize + (blockSize * maxZ)));

                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            idx = vertices.Count;
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize - blockSize, z * blockSize - blockSize ));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize + (blockSize * maxY), z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3(x * blockSize - blockSize, y * blockSize + (blockSize * maxY), z * blockSize - blockSize));

                            // Add triangle indeces
                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);


                            sides += 6;
                            for (int n = 0; n < 6; n++) {
                                colors.Add (new Color32((byte)((blocks_ [x, y, z] >> 24) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 16) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 8) & 0xFF),
                                                        (byte)255
                                                       ));
                            }
                        }
                    }
                    if (right == 0) {
                        if ((blocks_ [x, y, z] & 0x4) == 0) {
                            int maxZ = 0;
                            int maxY = 0;

                            for (int z_ = z; z_ < chunk.toZ; z_++) {
                                // Check not drawn + same color
                                if ((blocks_ [x, y, z_] & 0x4) == 0 && SameColor (blocks_ [x, y, z_], blocks_ [x, y, z])) {
                                    maxZ++;
                                } else {
                                    break;
                                }
                                int tmpY = 0;
                                for (int y_ = y; y_ < chunk.toY; y_++) {
                                    if ((blocks_ [x, y_, z_] & 0x4) == 0 && SameColor (blocks_ [x, y_, z_], blocks_ [x, y, z])) {
                                        tmpY++;
                                    } else {
                                        break;
                                    }
                                }
                                if (tmpY < maxY || maxY == 0) {
                                    maxY = tmpY;
                                }
                            }
                            for (int z_ = z; z_ < z + maxZ; z_++) {
                                for (int y_ = y; y_ < y + maxY; y_++) {
                                    blocks_ [x, y_, z_] = blocks_ [x, y_, z_] | 0x4;
                                }
                            }
                            maxZ--;
                            maxY--;

                            int idx = vertices.Count;

                            vertices.Add (new Vector3(x * blockSize, y * blockSize - blockSize, z * blockSize - blockSize ));
                            vertices.Add (new Vector3(x * blockSize, y * blockSize + (blockSize * maxY), z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3(x * blockSize, y * blockSize - blockSize, z * blockSize + (blockSize * maxZ) ));
                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            idx = vertices.Count;
                            vertices.Add (new Vector3(x * blockSize, y * blockSize + (blockSize * maxY), z * blockSize + (blockSize * maxZ)));
                            vertices.Add (new Vector3(x * blockSize, y * blockSize - blockSize, z * blockSize - blockSize ));
                            vertices.Add (new Vector3(x * blockSize, y * blockSize + (blockSize * maxY), z * blockSize - blockSize ));

                            // Add triangle indeces
                            tri.Add(idx);
                            tri.Add(idx+1);
                            tri.Add(idx+2);

                            sides += 6;
                            for (int n = 0; n < 6; n++) {
                                colors.Add (new Color32((byte)((blocks_ [x, y, z] >> 24) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 16) & 0xFF),
                                                        (byte)((blocks_ [x, y, z] >> 8) & 0xFF),
                                                        (byte)255
                                                       ));
                            }
                        }
                    }
                }
            }
        }
        if (chunk.type == Chunk.TYPE_OBJ || chunk.type == Chunk.TYPE_FF) {
            if(vertices.Count == 0) {
                GameObject.Destroy (chunk.obj); // Remove empty chunks.
                return;
            }
            // TBD: Perhaps have a tag
            if (chunk.type == Chunk.TYPE_FF) {
                chunk.obj.GetComponent<Rigidbody> ().mass = 0.1f * vertices.Count;
            }
        }
        chunk.obj.GetComponent<Renderer> ().enabled = false;
        Mesh mesh = chunk.obj.GetComponent<MeshFilter>().mesh;
        mesh.Clear ();
        mesh.vertices = vertices.ToArray ();
        mesh.triangles = tri.ToArray ();
        mesh.colors32 = colors.ToArray ();
        mesh.RecalculateNormals ();
        mesh.RecalculateBounds ();
        ;
        chunk.obj.GetComponent<Renderer> ().enabled = true;

        chunk.dirty = false;

        chunk.obj.GetComponent<MeshCollider> ().sharedMesh = mesh;

        if (chunk.type == Chunk.TYPE_OBJ || chunk.type == Chunk.TYPE_FF) {
            chunk.obj.transform.position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z);
        } else {
            chunk.obj.transform.position = new Vector3(
                                                       (chunk.fromX/chunkSize)-chunkSize/2 - (chunk.fromX/chunkSize) + chunkSize/2,
                                                       (chunk.fromY/chunkSize)-chunkSize/2 - (chunk.fromY/chunkSize) + chunkSize/2,
                                                       (chunk.fromZ/chunkSize)-chunkSize/2 - (chunk.fromZ/chunkSize) + chunkSize/2
                                                      );

        }

        chunk.toY = chunkYorg;
    }

    public static bool IsWithinWorld(int x, int y, int z) {
        if(x > 0 && x < width*chunkSize &&
           y > 0 && y <height*chunkSize &&
           z > 0 && z < depth*chunkSize) {
            return true;
        }
        return false;
    }

    public static void Explode(int x, int y, int z, int power) {
		// TBD: Removal of lights should be outside of this code.
        // remove streelights
        List<GameObject> tmpList = new List<GameObject> ();
        foreach (GameObject l in Proc.streetLights) {
            if (Vector3.Distance (l.transform.position, new Vector3 ((float)x, (float)y, (float)z)) < 50) {
                l.GetComponent<Light> ().intensity = 0;
                tmpList.Add (l);
            }
        }
        foreach(GameObject l in tmpList) {
            Proc.streetLights.Remove (l);
        }
        // Remove house lights
        foreach (GameObject l in Proc.houseLights) {
            if (Vector3.Distance (l.transform.position, new Vector3 ((float)x, (float)y, (float)z)) < 50) {
                l.GetComponent<Light> ().intensity = 0;
                tmpList.Add (l);
            }
        }
        foreach(GameObject l in tmpList) {
            Proc.streetLights.Remove (l);
        }

        int pow = power*power;
        List<Vector3> ffBlocks = new List<Vector3>();
        List<Vector3> rmBlocks = new List<Vector3>();
        for(int rx = x+power; rx >= x-power; rx--) {
            int px = (rx - x) * (rx - x);
            for(int rz = z+power; rz >= z-power; rz--) {
                int pz = (rz - z) * (rz - z);
                for(int ry = y+power; ry >= y-power; ry--) {
                    int val = px+(ry-y)*(ry-y)+pz;
                    if(val < pow - 1) {
                        // if(ry > 0) {
                        rmBlocks.Add (new Vector3 (rx, ry, rz));
                        // }
                    } else if(val > pow -1 )  {
                        if (ry > 11) {
                            if (IsWithinWorld (rx, ry, rz)) {
                                if ((blocks [rx, ry, rz] >> 8) != 0) {
                                    ffBlocks.Add (new Vector3 (rx, ry, rz));
                                }
                            }
                        }
                    }
                }
            }
        }
        for(int i = 0; i < rmBlocks.Count; i++) {
            RemoveBlock ((int)rmBlocks[i].x, (int)rmBlocks[i].y, (int)rmBlocks[i].z);
        }
        floodFill.Add (ffBlocks);
    }

    private static bool FloodFill(out List<Vector3> result, Vector3 start) {
        result = new List<Vector3>();
        Stack stack = new Stack();
        stack.Push(start);
        bool ret = true;
		int minY = floorHeight + 3;

        if((blocks[(int)start.x, (int)start.y, (int)start.z] & 0x40 ) != 0) {
            return false;
        }

		int x = 0;
		int y = 0;
		int z = 0;
        while (stack.Count != 0) {
            Vector3 b = (Vector3)stack.Pop ();
			x = (int)b.x;
			y = (int)b.y;
			z = (int)b.z;
            if ((blocks [x, y, z] & 0x40) == 0 && y >= minY) {
                result.Add (b);
				//blocks[(int)b.x, (int)b.y, (int)b.z] = (255 & 0xFF) << 24 | (0 & 0xFF) << 16 | (0 & 0xFF) << 8;

                blocks [x, y, z] |= 0x40;
				if (checkPushFF (start, x, y - 1, z)) {
					stack.Push (new Vector3 (x, y - 1, z)); 
                } 
                if (checkPushFF (start, x, y + 1, z)) {
                    stack.Push (new Vector3 (x, y + 1, z)); 
                } 
                if (checkPushFF (start, x + 1, y, z)) {
                    stack.Push (new Vector3 (x + 1, y, z)); 
                } 
				if (checkPushFF (start, x - 1, y, z)) {
                    stack.Push (new Vector3 (x - 1, y, z)); 
                } 
                if (checkPushFF (start, x, y, z - 1)) {
                    stack.Push (new Vector3 (x, y, z - 1)); 
                } 
                if (checkPushFF (start, x, y, z + 1)) {
                    stack.Push (new Vector3 (x, y, z + 1)); 
                } 
            } else {
                if (y < minY) {
                    ret = false; // no new chunk
                }
            }
        }
        return ret; // results in a new chunk
    }

	private static bool checkPushFF(Vector3 b, int x, int y,  int z) {
    //    if(IsWithinWorld((int)p.x, (int)p.y, (int)p.z)) {
		float dist = Vector3.Distance (b, new Vector3 ((float)x, (float)y, (float)z));
		if((blocks[x, y, z] >> 8) != 0 && dist < 40.0f) {
                return true;
		}
        return false;
    }

    public static void RemoveHangingBlocks(List<Vector3> ffBlocks) {
        List<List<Vector3>> newChunks = new List<List<Vector3>> ();
        List<List<Vector3>> all = new List<List<Vector3>> ();

        for (int i = 0; i < ffBlocks.Count; i++) {
            List<Vector3> result = null;
            Vector3 b = ffBlocks [i];
            if((blocks[(int)b.x, (int)b.y, (int)b.z] & 0x40) != 0) {
                continue;
            }
            if (FloodFill (out result, ffBlocks [i])) {
                if (result.Count > 0) {
                    all.Add (result);
                    newChunks.Add (result);
                }
            } else {
                if (result.Count > 0) {
                    all.Add (result);
                }
            }
        }

        for(int m = 0; m < newChunks.Count; m++) {
            List<Vector3> ff = newChunks[m];
            // Create chunk 
            Chunk chunk = new Chunk();
            chunk.dirty = true;
            chunk.fromX = 5000; // just some large value > world.
            chunk.fromZ = 5000;
            chunk.fromY = 5000;
            chunk.type = Chunk.TYPE_FF;

            List<Vector4> tmp = new List<Vector4> ();
            for(int q = 0; q < ff.Count; q++) {
                Vector3 b = ff[q];
                int val = blocks[(int)b.x, (int)b.y, (int)b.z]; 
                tmp.Add(new Vector4(b.x, b.y, b.z, (float)val));

				Chunk c = GetChunk ((int)b.x, (int)b.y, (int)b.z);
                if (!c.dirty) {
                    c.dirty = true;
                    rebuildList.Add (c);
                }
                if(b.x < chunk.fromX) {
                    chunk.fromX = (int)b.x;
                }
                if(b.x > chunk.toX) {
                    chunk.toX = (int)b.x;
                }
                if(b.y > chunk.toY) {
                    chunk.toY = (int)b.y;
                }
                if(b.y < chunk.fromY) {
                    chunk.fromY = (int)b.y;
                }
                if(b.z < chunk.fromZ) {
                    chunk.fromZ = (int)b.z;
                }
                if(b.z > chunk.toZ) {
                    chunk.toZ = (int)b.z;
                }
            }
            // Increase area to view all voxels for mesh creation
            chunk.fromX--;
            chunk.fromY--;
            chunk.fromZ--;
            chunk.toX++;
            chunk.toY++;
            chunk.toZ++;
            chunk.EnablePhys ();

            chunk.totalBlocksFF = tmp.Count;
            for(int i = 0; i < tmp.Count; i++) {
                chunk.blocks [(int)tmp [i].x-chunk.fromX, (int)tmp [i].y-chunk.fromY, (int)tmp [i].z-chunk.fromZ] = (int)tmp [i].w;
                blocks [(int)tmp [i].x, (int)tmp [i].y, (int)tmp [i].z] = 0;
            }
            int fromX = chunk.fromX;
            int fromY = chunk.fromY;
            int fromZ = chunk.fromZ;
            chunk.fromX = 0;
            chunk.fromY = 0;
            chunk.fromZ = 0;
            chunk.toX = chunk.toX - fromX;
            chunk.toY = chunk.toY - fromY;
            chunk.toZ = chunk.toZ - fromZ;

            RebuildChunks(chunk);
        }

        // Clears AFTER we have built the chunks where 0x40 are used.
        for(int i = 0; i < all.Count; i++) {
            List<Vector3> allb = all[i];
            for (int n = 0; n < allb.Count; n++) {
                Vector3 b = allb [n];
                blocks [(int)b.x, (int)b.y, (int)b.z] &= ~(1 << 6);
            }
        }
    }

    public static void CreateChunks() {
		chunks = new Chunk[width, height, depth];
        for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				for(int z = 0; z < depth; z++) {
					chunks[x, y, z] = new Chunk();
					chunks[x, y, z].type = Chunk.TYPE_WORLD;
					chunks[x, y, z].fromY = y * blockSize * chunkSize;
					chunks [x, y, z].toY = y * blockSize * chunkSize + chunkSize;
					chunks[x, y, z].fromX = x * blockSize * chunkSize ;
					chunks [x, y, z].toX = x * blockSize * chunkSize + chunkSize;
					chunks [x, y, z].fromZ = z * blockSize * chunkSize;
					chunks [x, y, z].toZ = z * blockSize * chunkSize + chunkSize;
				}
			}
		}
        blocks = new int[width*chunkSize, height*chunkSize, depth*chunkSize];
    }

    public static void RemoveBlock(int x, int y, int z) {
        if (x < 0 || y < 0 || z < 0 || x > width * chunkSize - 1 || y > height * chunkSize - 1 || z > depth * chunkSize - 1) {
            return;
        }
        if ((blocks [x, y, z] >> 8) == 0) {
            return;
        }

        Chunk chunk = GetChunk (x, y, z);
        if (chunk != null) {
            if (!chunk.dirty) {
                chunk.dirty = true;
                rebuildList.Add (chunk);
            }
        }
        if (Random.value > 0.75) {
           BlockPool.AddBlock (x, y, z, blocks [x, y, z], 1);
        }
        blocks[x, y, z] = 0;
    }

    public static void AddBlock(int x, int y, int z, int color) {
        if(x < 0 || y < 0 || z < 0 || x > width*chunkSize-1 || y > height*chunkSize-1 || z > depth*chunkSize-1) {
            return;
        }
        if(blocks[x, y, z] == 0) {
            blocks [x, y, z] = color; 
        }

        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null) {
            if (!chunk.dirty) {
                chunk.dirty = true;
                rebuildList.Add (chunk);
            }
        }
    }

    public static void RebuildDirtyChunks(bool all = false) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				for (int z = 0; z < depth; z++) {
					if (chunks [x, y, z].dirty || all) {
						RebuildChunks (chunks [x, y, z]);
					}
				}
			}
		}
    }

    public static bool SameColor(int block1, int block2) {
        if( ((block1 >> 8) & 0xFFFFFF) == ((block2 >> 8) & 0xFFFFFF) && block1 != 0 && block2 != 0) {
            return true;
        }
        return false;
    }

    private static bool BlockHidden(int x, int y, int z) {
        if((blocks[x, y, z] >> 8) == 0) {
            return true;
        }

        int left = 0, right = 0, above = 0,front = 0, back = 0, below = 0;

        if(y > 0) {
            if((blocks[x, y-1, z] >> 8) != 0) {
                below = 1;
            }
        }
        if(z > 0){
            if((blocks[x, y, z-1] >> 8) != 0) {
                back = 1;
            }
        }
        if(x > 0) {
            if((blocks[x-1, y, z] >> 8) != 0) {
                left = 1;
            }
        }
        if(x < width*chunkSize) {
            if((blocks[x+1, y, z] >> 8) != 0) {
                right = 1;
            }
        }
        if(y < height*chunkSize) {
            if((blocks[x, y+1, z] >> 8) != 0) {
                above = 1;
            }
        }
        if(z < depth*chunkSize){
            if((blocks[x, y, z+1] >> 8) != 0) {
                front = 1;
            }
        }

        if( front == 1 && left == 1 && right == 1 && above == 1 && back == 1 && below == 1) {
            return true;
        }
        return false;	
    } 

    private static Chunk GetChunk(int x, int y, int z) {
        int posx = x  / chunkSize;
		int posy = y  / chunkSize;
        int posz = z  / chunkSize;
		if(posx < 0 || posz < 0 || posz > depth || posx > width || posy < 0 || posy > height) {
            return null;
        }
		return chunks[posx, posy, posz];
    }

    // Use this for initialization
    void Start () {
		// create the chunks
        CreateChunks ();

		// Uncomment to produce a procedurally built city
        Proc.Landscape();
		// Comment out this when using the above Landscape()
        //MapHandler m = new MapHandler();
		//Vox.LoadModel("Assets/maps/monu9_test.vox", "map");


        RebuildDirtyChunks (true);
        started = true;
    }


    // Update is called once per frame
    void Update () {
        if (!started) {
            return;
        }

        if(floodFill.Count > 0) {
            float temp = Time.realtimeSinceStartup;
            RemoveHangingBlocks(floodFill[0]);
            floodFill.RemoveAt(0);
			if(Time.realtimeSinceStartup-temp > 0.05f) {
				print ("TIME FF: "+(Time.realtimeSinceStartup - temp).ToString("f6"));
			}
        }

        if (rebuildList.Count > 0 ){
            int build = 3;
            if(rebuildList.Count < 3) {
                build = rebuildList.Count;
            }
            for (int i = 0; i < build; i++) {
                RebuildChunks (rebuildList [0]);
                rebuildList.RemoveAt (0);
            }
        }

        if (Input.GetKeyDown (KeyCode.R)) {
            Debug.Log ("Rebuilding all chunks");
            RebuildDirtyChunks(true);
        }

    }
}
