using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator {

	public enum types {	
		Empty,
		Player,
		Portal,
		Room,
		Road,
		FloodFill,
	}

	public int height = 512;
	public int width = 512;
	public int maxRoomIterations = 50;
	public int roadSize = 10;
	public int maxRoomSize = 10;
	public int minRoomSize = 10;

	private int[,] map;
	private int[,] map2;

	private List<int[]> spawns = new List<int[]>();
	private List<int[]> doors = new List<int[]>();
	private List<KeyValuePair<int,int[]>> pillars = new List<KeyValuePair<int, int[]>>();
	private List<int[]> lights = new List<int[]>();
	private List<int[]> bridgeLights = new List<int[]>();
	private List<int[]> bridgeFence = new List<int[]>();

	public List<int[]> GetBridgeFence() {
		return bridgeFence;
	}


	public List<int[]> GetLights() {
		return lights;
	}

	public List<int[]> GetBridgeLights() {
		return bridgeLights;
	}

	public List<KeyValuePair<int, int[]>> GetPillars() {
		return pillars;
	}

	public int[,] Generate(int w, int h, int _maxRoomIterations, int _roadSize, int _maxRoomSize, int _minRoomSize) {
		width = w;
		height = h;
		roadSize = _roadSize;
		maxRoomIterations = _maxRoomIterations;
		maxRoomSize = _maxRoomSize;
		minRoomSize = _minRoomSize;

		map = new int[width, height];
		InitMap ();
		CreateRooms ();
		CreateRoads ();
		map2 = (int[,]) map.Clone ();

		bool ok = false;

		// TBD: FloodFill and get the largest part. Then remove the other parts. So we don't have to regenerate whole map.
		// TBD: Make roads fit better towards the rooms.

		// World;
		List<KeyValuePair<int, List<int[]>>> world = new List<KeyValuePair<int, List<int[]>>>();

		while(!ok) {
			int start_y = 0;
			int start_x = 0;
			for(int x = 0; x < width; x++) {
				for( int y = 0; y < height; y++) {
					if(map2[x, y] == (int)types.Room || map2[x, y] == (int)types.Road) {
						start_x = x;
						start_y = y;
						break;
					}
				}
				if(start_x != 0) {
					break;
				}
			}

			KeyValuePair<int, List<int[]>> res = FloodFill(start_x,start_y);
			if(res.Key == 0) {
				ok = true;
			} else {
				world.Add(new KeyValuePair<int, List<int[]>>(res.Key, res.Value));
			}
		}

		int largest = 0;
		foreach(KeyValuePair<int, List<int[]>> e in world) {
			if(e.Key > largest) {
				largest = e.Key;
			}
		}
		foreach(KeyValuePair<int, List<int[]>> e in world) {
			if(e.Key == largest) {
				continue;
			}
			Debug.Log("BEFORE: "+lights.Count);
			foreach (int[] l in e.Value) {
				map [l [0], l [1]] = (int)types.Empty;
				// Remove pillars/lights/bridges that are not included in world.
				pillars.RemoveAll (i => i.Value [0] == l [0] && i.Value [1] == l [1]);
				lights.RemoveAll (i => i [0] == l [0] && i [1] == l [1]-1);
				bridgeLights.RemoveAll (i => i [0] == l [0] && i [1] == l [1]);
				bridgeFence.RemoveAll (i => i [0] == l [0] && i [1] == l [1]);
				doors.RemoveAll (i => i [0] == l [0] && i [1] == l [1]);
			}
			Debug.Log("LIGHTS AFTER: "+lights.Count);
		}

		// Spawn ();
		return map;
	}

	private void InitMap() {
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++){
				map [x, y] = (int)types.Empty;
			}
		}
	}

	private void CreateRooms() {
		for (int i = 0; i < maxRoomIterations; i++) {
			int rx = (int)Random.Range (maxRoomSize/2+20, width - maxRoomSize/2-20);
			int ry = (int)Random.Range (maxRoomSize/2+20, height - maxRoomSize/2-20);

			int size_w = (int)Random.Range (minRoomSize, maxRoomSize);
			int size_h = (int)Random.Range (minRoomSize, maxRoomSize);

			bool free = true;

			for (int x = rx - (int)(size_w / 1.1); x < rx + (int)(size_w / 1.1); x++) {
				for (int y = ry - (int)(size_h / 1.1); y < ry + (int)(size_h / 1.1); y++) {
					if (x > 0 && x < width && y > 0 && y < height) {
						if (map [x, y] == (int)types.Room) {
							free = false;
							break;
						}
					}
				}
				if (!free) {
					break;
				}
			}

			if (free) {
				// TBD: Don't add on thoose who are removed.
				pillars.Add(new KeyValuePair<int, int[]>(size_h-8, new int[]{rx-size_w/2+4, ry, -90}));
				pillars.Add(new KeyValuePair<int, int[]>(size_h-8, new int[]{rx+size_w/2-4, ry, 90}));
				pillars.Add(new KeyValuePair<int, int[]>(size_w-8, new int[]{rx, ry-size_h/2+4, 180}));
				pillars.Add(new KeyValuePair<int, int[]>(size_w-8, new int[]{rx, ry+size_h/2-4, 0}));

				// lights
				lights.Add(new int[]{rx+size_w/2, ry-size_h/2});
				lights.Add(new int[]{rx+size_w/2, ry+size_h/2});
				lights.Add(new int[]{rx-size_w/2, ry+size_h/2});
				lights.Add(new int[]{rx-size_w/2, ry-size_h/2});


				for(int x = rx - (int)(size_w/2); x < (int)(rx + size_w/2); x++) {
					for(int y = (int)(ry - size_h/2); y < (int)(ry + size_h/2); y++) {
						map[x, y] = (int)types.Room;
						if(x == rx && y == ry) {
							spawns.Add(new int[]{x,y});
						}
						if(x == rx && y == (int)(ry-size_h/2) ) {
							map[x, y] = (int)types.Road;
							doors.Add(new int[]{x,y});
						}
						if(x == rx && y == (int)(ry+size_h/2-1) ) {
							map[x, y] = (int)types.Road;
							doors.Add(new int[]{x,y});
							}
						if(y == ry && x == rx-(int)(size_w/2)) {
							map [x, y] = (int)types.Road;
							doors.Add(new int[]{x,y});
						}
						if(y == ry && x == rx+(int)(size_w/2-1)) {
							map[x, y] = (int)types.Road;
							doors.Add(new int[]{x,y});
						}
					}
				}

			}
		}
	}

	private void CreateRoads() {
		foreach (var p in doors) {
			int count = 1;
			List<int[]> list = new List<int[]> ();
			bool roadFail = false;
			while (map [p [0] + count, p [1]] == 0 && p [0] + count > 1 && p [0] + count < width - 1) {
				// Check if we can make a broad road.
				for (int yy = p [1] - roadSize / 2; yy < p [1] + roadSize / 2; yy++) {
					if (yy > 1 && yy < height) {
						// Do not hit room our existing road.
						if (map [p [0] + count, yy] == (int)types.Room || map [p [0] + count, yy] == (int)types.Road) {
							roadFail = true;
							break;
						}
					}
				}
				if (roadFail) {
					break;
				}
				list.Add (new int[]{p [0] + count, p [1]});
				count++;
			}
			if (!roadFail) {
				for (int yy = p [1] - roadSize / 2; yy < p [1] + roadSize / 2; yy++) {
					if (yy > 1 && yy < height) {
						if (map [p [0] + count, yy] == (int)types.Empty) {
							roadFail = true;
							break;
						}
					}
				}
				if (!roadFail && count > 5) {
					for (int yy = p [1] - roadSize / 2; yy < p [1] + roadSize / 2; yy++) {
						for (int i = 0; i < 10; i++) {
							if (Random.Range(0,i) < 3) {
								map [p[0]+count+i, p [1]] = (int)types.Road;
								map [p[0]-i, p [1]] = (int)types.Road;
							}
						}
					}
					int q = 0;
					foreach (var l in list) {
						if (q++ == 40) {
							q = 0;
							bridgeLights.Add (new int[]{ l [0], l[1] + roadSize/2 + 3});
							bridgeLights.Add (new int[]{ l [0], l[1] - roadSize/2 - 3});
						} else {
							bridgeFence.Add (new int[]{ l [0], l[1] - roadSize/2 - 3});
							bridgeFence.Add (new int[]{ l [0], l[1] + roadSize/2 + 3});
						}
						for (int yy = l [1] - roadSize / 2; yy < l [1] + roadSize / 2; yy++) {
							map [l [0], yy] = (int)types.Road;
						}
					}
				}
			}

			count = 1;
			list.Clear ();
			roadFail = false;
			while(map[p[0], p[1]+count] == (int)types.Empty && p[1]+count > 1 && p[1]+count < height- 1) {
				for(int xx = p[0]-roadSize/2; xx < p[0]+roadSize/2; xx++) {
					if(xx > 1 && xx < width) {
						if(map[xx, p[1]+count] == (int)types.Room || map[xx, p[1]+count] == (int)types.Road) {
							roadFail = true;
							break;
						}
					}
				}
				if(roadFail) {
					break;
				}
				list.Add (new int[]{p[0], p [1]+count});
				count++;
			}
			if(!roadFail) {
				for(int xx = p[0]-roadSize/2; xx < p[0]+roadSize/2; xx++) {
					if(xx > 1 && xx < width) {
						if(map[xx, p[1]+count] == (int)types.Empty) {
							roadFail = true;
							break;
						}
					}
				}
				if (!roadFail && count > 5) {
					for (int xx = p [0] - roadSize / 2; xx < p [0] + roadSize / 2; xx++) {
						for (int i = 0; i < 10; i++) {
							if (Random.Range(0,i) < 3) {
								map [xx, p [1] + count + i] = (int)types.Road;
								map [xx, p [1] - i] = (int)types.Road;
							}
						}
					}
					int q = 0;
					foreach (var l in list) {
						if (q++ == 40) {
							q = 0;
							bridgeLights.Add (new int[]{ l [0] - roadSize / 2 - 3, l [1]});
							bridgeLights.Add (new int[]{ l [0] + roadSize / 2 + 3, l [1]});
						} else {
							bridgeFence.Add (new int[]{ l [0] - roadSize / 2 - 3, l [1]});
							bridgeFence.Add (new int[]{ l [0] + roadSize / 2 + 3, l [1]});
						}
						for (int xx = l [0] - roadSize / 2; xx < l [0] + roadSize / 2; xx++) {
							map [xx, l [1]] = (int)types.Road;
						}
					}
				}
			}
		}
		foreach(var d in doors) {
			map[d[0], d[1]] = (int)types.Room;
		}
	}

	private KeyValuePair<int, List<int[]>> FloodFill(int start_x, int start_y) {
		Stack<int[]> stack = new Stack<int[]> ();
		stack.Push(new int[]{start_x, start_y});
		int[] p;
		List<int[]> list = new List<int[]> ();
		int count = 0;

		while(stack.Count > 0) {
			p = stack.Pop ();
			list.Add (p);
			if(map2[p[0],p[1]] == (int)types.Room || map2[p[0], p[1]] == (int)types.Road) {
				map2[p[0], p[1]] = (int)types.FloodFill;
				count++;
				if(p[0]+1 > 0 && p[0]+1 < width) {
					stack.Push(new int[]{p[0]+1, p[1]});
				}
				if(p[0]-1 > 0 && p[0]-1 < width) {
					stack.Push(new int[]{p[0]-1, p[1]});
				}
				if(p[1]-1 > 0 && p[1]-1 < height) {
					stack.Push(new int[]{p[0], p[1]-1});
				}
				if(p[1]+1 > 0 && p[1]+1 < height) {
					stack.Push(new int[]{p[0], p[1]+1});
				}
			}
		}
		return new KeyValuePair<int, List<int[]>>(count, list);
	}

	private void Spawn() {
		int[] player = new int[]{};
		int[] portal = new int[]{ };

		int max_dist = 0;
		foreach (var p in spawns) {
			foreach (var p2 in spawns) {
				int dist = (int)Mathf.Sqrt( Mathf.Pow(p2[0]-p[0], 2)+Mathf.Pow(p2[1]-p[1], 2));
				if(dist > max_dist) {
					max_dist = dist;
					player = new int[]{p[0], p[1]};
					portal = new int[]{p2[0], p2[1]};
				}
			}
		}
		map[player[0], player[1]] = (int)types.Player;
		map[portal[0], portal[1]] = (int)types.Portal;
	}
}
