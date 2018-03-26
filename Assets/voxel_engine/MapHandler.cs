using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapHandler
{
    public int max = 30;
	const int width = 40;
	const int height = 40;
//	bool[, ] cellmap = new bool[width, height];
	float chanceToStartAlive = 0.4f;
	int numberOfSteps = 10;
	int deathLimit = 3;
	int birthLimit = 4;

	public bool[,] initialiseMap(bool[,] map){
		for(int x=0; x<width; x++){
			for(int y=0; y<height; y++){
				if(Random.value < chanceToStartAlive){
					map[x, y] = true;
				}
			}
		}
		return map;
	}
	public bool[,] doSimulationStep(bool[,] oldMap){
		bool[,] newMap = new bool[width, height];
		for(int x=0; x<oldMap.Length; x++){
			for(int y=0; y<oldMap.Length; y++){
				if(x >= width || y >= height) {
					continue;
				}
				int nbs = countAliveNeighbours(oldMap, x, y);
				if(oldMap[x, y]){
					if(nbs < deathLimit){
						newMap[x, y] = false;
					}
					else{
						newMap[x, y] = true;
					}
				}
				else{
					if(nbs > birthLimit){
						newMap[x, y] = true;
					}
					else{
						newMap[x, y] = false;
					}
				}
			}
		}
		return newMap;
	}

	public int countAliveNeighbours(bool[, ] map, int x, int y){
		int count = 0;
		for(int i=-1; i<2; i++){
			for(int j=-1; j<2; j++){
				int neighbour_x = x+i;
				int neighbour_y = y+j;
				if(neighbour_x >= width || neighbour_y >= height) {
					continue;
				}
				if(i == 0 && j == 0){
				}
				else if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.Length || neighbour_y >= map.Length){
					count = count + 1;
				}
				else if(map[neighbour_x, neighbour_y]){
					count = count + 1;
				}
			}
		}
		return count;
	}

	private Mesh CreateMesh(float width, float height)
	{
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = new Vector3[] {
			new Vector3(-width, -height, 0.01f),
			new Vector3(width, -height, 0.01f),
			new Vector3(width, height, 0.01f),
			new Vector3(-width, height, 0.01f)
		};
		m.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (0, 1),
			new Vector2(1, 1),
			new Vector2 (1, 0)
		};
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		m.RecalculateNormals();

		return m;
	}


	private void CreatePlane(float w, float h, float rotation, int x, int y) {
		GameObject plane = new GameObject("Plane");
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = CreateMesh(w/2, h);
		MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		//renderer.material.shader = Shader.Find ("Particles/Additive");
		TextureHandler th = (TextureHandler)GameObject.FindObjectOfType<TextureHandler> ();
		//renderer.material.mainTexture = th.pillars;
		renderer.material.color = th.pillars;
		renderer.material.shader = Shader.Find ("Standard (Vertex Color)");
		//renderer.material.SetTextureScale("Tiling", new Vector2(5f,5f))
//		Texture2D tex = new Texture2D(1, 1);
//		tex.SetPixel(0, 0, Color.green);
//		tex.Apply();
//		renderer.material.mainTexture = tex;
//		renderer.material.color = Color.green;
		plane.transform.rotation = Quaternion.Euler(new Vector3(0,rotation,0));
		plane.transform.position = new Vector3 (x, -h+World.floorHeight-1, y);
	}

	public MapHandler() {
		DungeonGenerator gen = new DungeonGenerator ();
		int width = 1024;
		int height = 1024;
		int[,] map = gen.Generate (width, height, 50, 30, 250, 100);
		World.width = height / World.chunkSize;
		World.depth = width / World.chunkSize;
		World.CreateChunks ();

		TextureHandler th = (TextureHandler)GameObject.FindObjectOfType<TextureHandler> ();

		// Create pillars below rooms
		List<KeyValuePair<int, int[]>> pillars = gen.GetPillars ();
		foreach(var p in pillars ) {
			CreatePlane (p.Key, 100, p.Value[2], p.Value[0], p.Value[1]);
		}

		// Create lights
		List<int[]> lights = gen.GetLights ();
		foreach(var l in lights ) {
			// Create light pillars.
			int size = 10;
			for(int x = l[0]-size/2+1; x < l[0]+size/2-1; x++) {
				for(int y = l[1]-size/2+1; y < l[1]+size/2-1; y++) {
					float r = Random.Range (1, 15);
					for (int h = 15; h > 15-r; h--) {
//						World.blocks [x, h, y] = th.GetGroundColor2 (x, h);
						World.blocks [x, h, y] = th.GetColor(th.pillars);
					}
					for(int h = 40; h > 15; h--) {
						//World.blocks [x, h, y] = th.GetGroundColor2 (x, h);
						World.blocks [x, h, y] = th.GetColor(th.pillars);
					}
				}
			}
			CreateLight (l[0]-1, 45, l[1]-1);
		}

		// Create brdige lights
		List<int[]> bridgeLights = gen.GetBridgeLights ();
		foreach(var l in bridgeLights ) {
			// Create light pillars.
			int size = 6;
			for(int x = l[0]-size/2; x < l[0]+size/2; x++) {
				for(int y = l[1]-size/2; y < l[1]+size/2; y++) {
					float r = Random.Range (1, 15);
					for (int h = 15; h > 15-r; h--) {
						World.blocks [x, h, y] = th.GetColor (th.pillars);
//						World.blocks [x, h, y] = th.GetGroundColor2 (x, h);
					}
					for(int h = 35; h > 15; h--) {
//						World.blocks [x, h, y] = th.GetGroundColor2 (x, h);
						World.blocks [x, h, y] = th.GetColor (th.pillars);
					}
				}
			}
			World.blocks [l [0]-1, 36, l [1]-1] = (255 & 0xFF) << 24 | (255 & 0xFF) << 16 | (251 & 0xFF) << 8;
			World.blocks [l [0]-1, 37, l [1]-1] = (255 & 0xFF) << 24 | (255 & 0xFF) << 16 | (251 & 0xFF) << 8;
			CreateBridgeLight (l[0]-1, 37, l[1]-1);
		}

		// Fences on bridges
		List<int[]> bridgeFence = gen.GetBridgeFence();
		foreach(var l in bridgeFence ) {
			// Create light pillars.
			int size = 4;
			for(int x = l[0]-size/2; x < l[0]+size/2; x++) {
				for(int y = l[1]-size/2; y < l[1]+size/2; y++) {
					World.blocks [x, 30, y] = th.GetColor (th.fence);
					//World.blocks [x, 30, y] = th.GetFenceColor (x, y);
				}
			}
		}




		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				if (map [x, y] == (int)DungeonGenerator.types.Room) {
//					World.blocks [x, World.floorHeight, y] = th.GetGroundColor (x,y);
					World.blocks [x, World.floorHeight, y] = th.GetColor (th.floor);
				} else if(map[x,y] == (int)DungeonGenerator.types.Road) {
//					World.blocks [x, World.floorHeight, y] = th.GetGroundColor2 (x,y);
					World.blocks [x, World.floorHeight, y] = th.GetColor (th.bridge);
				} else if(map[x,y] == (int)DungeonGenerator.types.Player) {
					Player.player.obj.transform.position = new Vector3(x,World.floorHeight,y);
				}

				// ROOMS
				if (x + 1 < width) {
					if (map [x + 1, y] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Room) {
						int r = (int)Random.Range (4, 15);

						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
								World.blocks [x + i, n, y] = th.GetColor (th.wall);
//								World.blocks [x + i, n, y] = th.GetWallColor (n, y);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 3; i++) {
							//	World.blocks [x + i, n, y] = th.GetWallColor (n, y);
								World.blocks [x + i, n, y] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight+2; n <= World.floorHeight+15; n++) {
							for (int i = 0; i < 6; i++) {
///								World.blocks [x + i, n, y] = th.GetWallColor (n, y);
								World.blocks [x + i, n, y] = th.GetColor (th.wall);
							}
							if(n == World.floorHeight+15) {
								for (int i = 0; i < 6; i++) {
									//World.blocks [x + i, n, y] = th.GetGroundColor2 (x-i, y);
									World.blocks [x + i, n, y] = th.GetColor (th.floor);
								}
							}
						}
					}
				}
				if (x - 1 > 0) {
					if (map [x - 1, y] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Room) {
						int r = (int)Random.Range (4, 15);
						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x - i, n, y] = th.GetWallColor (n, y);
								World.blocks [x - i, n, y] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 3; i++) {
//								World.blocks [x - i, n, y] = th.GetWallColor (n, y);
								World.blocks [x - i, n, y] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight+2; n <= World.floorHeight+15; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x - i, n, y] = th.GetWallColor (n, y);
								World.blocks [x - i, n, y] = th.GetColor (th.wall);
							}
							if(n == World.floorHeight+15) {
								for (int i = 0; i < 6; i++) {
//									World.blocks [x - i, n, y] = th.GetGroundColor2 (x+i, y);
									World.blocks [x - i, n, y] = th.GetColor (th.floor);
								}
							}
						}
					}
				}
				if (y + 1 < height) {
					if (map [x, y + 1] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Room) {
						int r = (int)Random.Range (4, 15);
						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x, n, y+i] = th.GetWallColor (x, n);
								World.blocks [x, n, y+i] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 3; i++) {
//								World.blocks [x, n, y+i] = th.GetWallColor (x, n);
								World.blocks [x, n, y+i] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight+2; n <= World.floorHeight+15; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x, n, y+i] = th.GetWallColor (x, n);
								World.blocks [x, n, y+i] = th.GetColor (th.wall);
							}
							if(n == World.floorHeight+15) {
								for (int i = 0; i < 6; i++) {
//									World.blocks [x, n, y+i] = th.GetGroundColor2 (x, y+i);
									World.blocks [x, n, y+i] = th.GetColor (th.floor);
								}
							}
						}
					}
				}
				if (y > 0) {
					if (map [x, y - 1] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Room) {
						int r = (int)Random.Range (4, 15);
						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x, n, y-i] = th.GetWallColor (x, n);
								World.blocks [x, n, y-i] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 3; i++) {
								World.blocks [x, n, y-i] = th.GetColor (th.wall);
//								World.blocks [x, n, y-i] = th.GetWallColor (x, n);
							}
						}
						for (int n = World.floorHeight+2; n <= World.floorHeight+15; n++) {
							for (int i = 0; i < 6; i++) {
								World.blocks [x, n, y-i] = th.GetColor (th.wall);
//								World.blocks [x, n, y-i] = th.GetWallColor (x, n);
							}
							if(n == World.floorHeight+15) {
								for (int i = 0; i < 6; i++) {
//									World.blocks [x, n, y-i] = th.GetGroundColor2 (x, y-i);
									World.blocks [x, n, y-i] = th.GetColor (th.floor);
								}
							}
						}
					}
				}

				// ROADS
				if (x + 1 < width) {
					if (map [x + 1, y] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Road) {
						int r = (int)Random.Range (4, 10);
						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x + i, n, y] = th.GetWallColor (n, y);
								World.blocks [x + i, n, y] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight; n++) {
							for (int i = 0; i < 3; i++) {
								World.blocks [x + i, n, y] = th.GetColor (th.wall);
//								World.blocks [x + i, n, y] = th.GetWallColor (n, y);
							}
						}
						for (int n = World.floorHeight; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 2; i++) {
//								World.blocks [x + i, n, y] = th.GetWallColor (n, y);
								World.blocks [x + i, n, y] = th.GetColor (th.wall);
							}
							if(n == World.floorHeight+2) {
								for (int i = 0; i < 6; i++) {
									World.blocks [x + i, n, y] = th.GetColor (th.floor);
//									World.blocks [x + i, n, y] = th.GetGroundColor2 (x-i, y);
								}
							}
						}
					}
				}
				if (x - 1 > 0) {
					if (map [x - 1, y] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Road) {
						int r = (int)Random.Range (4, 10);
						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x - i, n, y] = th.GetWallColor (n, y);
								World.blocks [x - i, n, y] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight; n++) {
							for (int i = 0; i < 3; i++) {
								World.blocks [x - i, n, y] = th.GetColor (th.wall);
//								World.blocks [x - i, n, y] = th.GetWallColor (n, y);
							}
						}
						for (int n = World.floorHeight; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 6; i++) {
								World.blocks [x - i, n, y] = th.GetColor (th.wall);
//								World.blocks [x - i, n, y] = th.GetWallColor (n, y);
							}
							if(n == World.floorHeight+2) {
								for (int i = 0; i < 6; i++) {
//									World.blocks [x - i, n, y] = th.GetGroundColor2 (x+i, y);
									World.blocks [x - i, n, y] = th.GetColor (th.floor);
								}
							}
						}
					}
				}
				if (y + 1 < height) {
					if (map [x, y + 1] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Road) {
						int r = (int)Random.Range (4, 10);
						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
								World.blocks [x, n, y+i] = th.GetColor (th.wall);
//								World.blocks [x, n, y+i] = th.GetWallColor (x, n);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight; n++) {
							for (int i = 0; i < 3; i++) {
								World.blocks [x, n, y+i] = th.GetColor (th.wall);
//								World.blocks [x, n, y+i] = th.GetWallColor (x, n);
							}
						}
						for (int n = World.floorHeight; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x, n, y+i] = th.GetWallColor (x, n);
								World.blocks [x, n, y+i] = th.GetColor (th.wall);
							}
							if(n == World.floorHeight+2) {
								for (int i = 0; i < 6; i++) {
									World.blocks [x, n, y+i] = th.GetColor (th.floor);
//									World.blocks [x, n, y+i] = th.GetGroundColor2 (x, y+i);
								}
							}
						}
					}
				}
				if (y > 0) {
					if (map [x, y - 1] == (int)DungeonGenerator.types.Empty && map [x, y] == (int)DungeonGenerator.types.Road) {
						int r = (int)Random.Range (4, 10);
						for (int n = World.floorHeight - r; n <= World.floorHeight-3; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x, n, y-i] = th.GetWallColor (x, n);
								World.blocks [x, n, y-i] = th.GetColor (th.wall);
							}
						}
						for (int n = World.floorHeight-3; n <= World.floorHeight; n++) {
							for (int i = 0; i < 3; i++) {
								World.blocks [x, n, y-i] = th.GetColor (th.wall);
//								World.blocks [x, n, y-i] = th.GetWallColor (x, n);
							}
						}
						for (int n = World.floorHeight; n <= World.floorHeight+2; n++) {
							for (int i = 0; i < 6; i++) {
//								World.blocks [x, n, y-i] = th.GetWallColor (x, n);
								World.blocks [x, n, y-i] = th.GetColor (th.wall);
							}
							if(n == World.floorHeight+2) {
								for (int i = 0; i < 6; i++) {
//									World.blocks [x, n, y-i] = th.GetGroundColor2 (x, y-i);
									World.blocks [x, n, y-i] = th.GetColor (th.floor);
								}
							}
						}
					}
				}
			}
		}
	}
		// TBD: make meshes ("grounds") on all rooms.
		// TBD: Make borders on roads
		// TBD: Make walls on rooms except where door is.


//	public void MapHandler2(){
//		int f = World.floorHeight;
//
//
//		bool[,] cellmap = new bool[width, height];
//		cellmap = initialiseMap(cellmap);
//		for(int i=0; i<numberOfSteps; i++){
//			cellmap = doSimulationStep(cellmap);
//		}
//
//		// TBD: Flood fill to check largest chunk 
//
//		// Increase the map.
//		int size = 15;
//		World.width = (height * size) / World.chunkSize;
//		World.depth = (width * size) / World.chunkSize;
//		World.CreateChunks ();
//
//		TextureHandler th = (TextureHandler)GameObject.FindObjectOfType<TextureHandler> ();
//		for(int x = 3; x < width-3; x++) {
//			for(int y = 3; y < height-3; y++) {
//				if(cellmap[x,y] == false) {
//					for (int x1 = x * size; x1 < x * size + size; x1++) {
//						for (int y1 = y * size; y1 < y * size + size; y1++) {
//							World.AddBlock (x1, World.floorHeight, y1, 10000);
//						}
//					}
//				}
//			}
//		}
////							if(n > max - 2 && Random.value > 0.95) {
////								CreateLight (x, n, y);
////							}
//
//		// Texture the walls and floor
//		max = World.floorHeight - 10;
//        for (int x = 1; x < World.width*World.chunkSize - 1; x++) {
//            for (int y = 1; y < World.depth*World.chunkSize - 1; y++) {
//                if(World.blocks[x,f,y] != 0) {
//                   int color = th.GetGroundColor (x, y);
//                    World.blocks [x, f, y] = color;
//
//                    if(World.blocks[x+1,f,y] == 0) {
//						int from = 0; // (int)Random.Range(0,f);
//						for(int n = from; n < max; n++) {
//                            World.blocks [x-3, n, y] = th.GetWallColor(n,y);
//                        }
//						for(int n = max; n < World.floorHeight+5; n++) {
//							int col = th.GetWallColor2 (n,y);
//							if (n == World.floorHeight + 4) {
//								col = th.GetGroundColor2 (x, y);
//								World.blocks [x, n, y] = col;
//								col = th.GetGroundColor2 (x-1, y);
//								World.blocks [x - 1, n, y] = col;
//								col = th.GetGroundColor2 (x-2, y);
//								World.blocks [x - 2, n, y] = col;
//								col = th.GetGroundColor2 (x-3, y);
//								World.blocks [x - 3, n, y] = col;
//							} else {
//								World.blocks [x, n, y] = col;
//								World.blocks [x - 1, n, y] = col;
//								World.blocks [x - 2, n, y] = col;
//								World.blocks [x - 3, n, y] = col;
//							}
//                        }
//                    }
//                    if(World.blocks[x-1,f,y] == 0) {
//						int from = 0; // (int)Random.Range(0,f);
//                        for(int n = from; n < max; n++) {
//							World.blocks [x, n, y] = th.GetWallColor(n,y);
//                        }
//                    }
//                    if(World.blocks[x,f,y+1] == 0) {
//						int from = 0; // (int)Random.Range(0,f);
//                        for(int n = from; n < max; n++) {
//                           World.blocks [x, n, y] = th.GetWallColor(x,n);
//                        }
//                    }
//                    if(World.blocks[x,f,y-1] == 0) {
//						int from = 0; // (int)Random.Range(0,f);
//                        for(int n = from; n < max; n++) {
//							World.blocks [x, n, y] = th.GetWallColor(x,n);
//                        }
//                    }
//
//					// TBD: Fix this.
//                    if(World.blocks[x+1,f,y-1] == 0) {
//						int from = 0; // (int)Random.Range(0,f);
//                        for(int n = from; n < max; n++) {
//							//World.blocks [x, n, y] = th.GetWallColor(y, n);
//                        }
//                    }
//                    if(World.blocks[x-1,f,y+1] == 0) {
//                        int from = (int)Random.Range(0,f);
//                        for(int n = from; n < max; n++) {
//							//World.blocks [x, n, y] = th.GetWallColor(n, x);
//                        }
//                    }
//                    if(World.blocks[x+1,f,y+1] == 0) {
//                        int from = (int)Random.Range(0,f);
//                        for(int n = from; n < max; n++) {
//                         //   World.blocks [x, n, y] = th.GetWallColor(x,n);
//                        }
//                    }
//                    if(World.blocks[x-1,f,y-1] == 0) {
//                        int from = (int)Random.Range(0,f);
//                        for(int n = from; n < max; n++) {
//                          //  World.blocks [x, n, y] = th.GetWallColor(x,n);
//                        }
//                    }
//                }
//            }
//        }
//
//		// Just place 3 boxes randomly to test
////        AddBox(150, 150, 10);
////        AddBox(150, 350, 10);
////        AddBox(350, 350, 10);
////
////        AddTower(330, 350, 30, 5);
////        AddTower(310, 250, 40, 5);
//	}

//    public void AddCarpet(int x_, int y_, int h) {
//		int size = 32;
//		TextureHandler th = (TextureHandler)GameObject.FindObjectOfType<TextureHandler> ();
//        for(int x = x_; x <= x_ + size; x++) {
//            int xd = x - x_;
//            for(int y = 0; y <= size*2; y++) {
//                World.blocks[x, h, y+y_] = th.GetCarpetColor(xd, y, size);
//            }
//        }
//    }

	private void CreateLight(int x, int y, int z) {
		// Create particle flow
		GameObject myFire = GameObject.Instantiate(GameObject.Find("BlueFire"));
		myFire.transform.localPosition = new Vector3(x, y, z);
		myFire.GetComponent<ParticleSystem> ().Play ();
		GameObject fireLight = GameObject.Instantiate(GameObject.Find("firelight"));
		fireLight.transform.localPosition = new Vector3(x, y+2, z);
	}

	private void CreateBridgeLight(int x, int y, int z) {
		// Create particle flow
		GameObject myFire = GameObject.Instantiate(GameObject.Find("BridgeFire"));
		myFire.transform.localPosition = new Vector3(x, y, z);
		myFire.GetComponent<ParticleSystem> ().Play ();
//		GameObject fireLight = GameObject.Instantiate(GameObject.Find("BridgeLight"));
//		fireLight.transform.localPosition = new Vector3(x, y+2, z);
//		fireLight.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
	}
//
//	public void AddTower(int x_, int z_, int h_, int size) {
//		TextureHandler th = (TextureHandler)GameObject.FindObjectOfType<TextureHandler> ();
//        for(int x = x_; x <= x_ + size; x++) {
//            int xd = x - x_;
//            for(int z = 0; z <= size; z++) {
//				for (int h = 0; h <= h_; h++) {
//					World.blocks [x, h + z, z_ + size] = th.GetBoxColor (xd, z, size);
//					World.blocks [x_ + size, h + z, x] = th.GetBoxColor (xd, z, size);
//					World.blocks [x, h + z, z_] = th.GetBoxColor (xd, z, size);
//					World.blocks [x_, h + z, x] = th.GetBoxColor (xd, z, size);
//					World.blocks [x, h + size, z + z_] = th.GetBoxColor (xd, z, size);
//				}
//            }
//        }
//
//    }

//    public void AddBox(int x_, int y_, int h) {
//        int size = 16;
//		TextureHandler th = (TextureHandler)GameObject.FindObjectOfType<TextureHandler> ();
//        for(int x = x_; x <= x_ + size; x++) {
//            int xd = x - x_;
//            for(int y = 0; y <= size; y++) {
//                World.blocks[x, h+y, y_+size] = th.GetBoxColor(xd, y, size);
//                World.blocks[x_+size, h+y, x] = th.GetBoxColor(xd, y, size);
//                World.blocks[x, h+y, y_] = th.GetBoxColor(xd, y, size);
//                World.blocks[x_, h+y, x] = th.GetBoxColor(xd, y, size);
//                World.blocks[x, h+size, y+y_] = th.GetBoxColor(xd, y, size);
//            }
//        }
//
//    }
}

