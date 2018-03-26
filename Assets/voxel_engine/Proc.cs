using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using csDelaunay;

public class Proc {
    private static int[] wallColors = new int[] {
        (165 & 0xFF) << 24 | (123 & 0xFF) << 16 | (82 & 0xFF) << 8,
            (193 & 0xFF) << 24 | (189 & 0xFF) << 16 | (172 & 0xFF) << 8,
            (177 & 0xFF) << 24 | (82 & 0xFF) << 16 | (82 & 0xFF) << 8
    };
    private static int[] roofColors = new int[] {
        (205 & 0xFF) << 24 | (96 & 0xFF) << 16 | (96 & 0xFF) << 8,
            (107 & 0xFF) << 24 | (107 & 0xFF) << 16 | (107 & 0xFF) << 8,
            (177 & 0xFF) << 24 | (173 & 0xFF) << 16 | (156 & 0xFF) << 8,
            (220 & 0xFF) << 24 | (142 & 0xFF) << 16 | (59 & 0xFF) << 8
    };

    private static int[] flowerColors = new int[] {
        (212 & 0xFF) << 24 | (37 & 0xFF) << 16 | (169 & 0xFF) << 8,
            (237 & 0xFF) << 24 | (92 & 0xFF) << 16 | (63 & 0xFF) << 8,
            (2 & 0xFF) << 24 | (173 & 0xFF) << 16 | (210 & 0xFF) << 8,
            (2 & 0xFF) << 24 | (173 & 0xFF) << 16 | (149 & 0xFF) << 8,
            (210 & 0xFF) << 24 | (0 & 0xFF) << 16 | (128 & 0xFF) << 8,
            (225 & 0xFF) << 24 | (197 & 0xFF) << 16 | (67 & 0xFF) << 8
    };

    private static int streetLightColor = (131 & 0xFF) << 24 | (131 & 0xFF) << 16 | (131 & 0xFF) << 8;
    private static int sideWalkColor = (214 & 0xFF) << 24 | (209 & 0xFF) << 16 | (190 & 0xFF) << 8;
    private static int roadColor = (104 & 0xFF) << 24 | (104 & 0xFF) << 16 | (104 & 0xFF) << 8;
    private static int drainColor = (40 & 0xFF) << 24 | (40 & 0xFF) << 16 | (40 & 0xFF) << 8;
    private static int dirtColor1 = (128 & 0xFF) << 24 | (95 & 0xFF) << 16 | (62 & 0xFF) << 8;
    private static int dirtColor2 = (162 & 0xFF) << 24 | (134 & 0xFF) << 16 | (24 & 0xFF) << 8;
    private static int grassColor1 = (137 & 0xFF) << 24 | (179 & 0xFF) << 16 | (74 & 0xFF) << 8;
    private static int grassColor2 = (120 & 0xFF) << 24 | (159 & 0xFF) << 16 | (65 & 0xFF) << 8;
    private static int fruitColor = (225 & 0xFF) << 24 | (71 & 0xFF) << 16 | (71 & 0xFF) << 8;
    private static int treeColor = (137 & 0xFF) << 24 | (180 & 0xFF) << 16 | (75 & 0xFF) << 8;
    private static int stemColor = (143 & 0xFF) << 24 | (106 & 0xFF) << 16 | (70 & 0xFF) << 8;
    private static int doorColor = (153 & 0xFF) << 24 | (106 & 0xFF) << 16 | (70 & 0xFF) << 8;
    private static int flowerStemColor = (120 & 0xFF) << 24 | (190 & 0xFF) << 16 | (65 & 0xFF) << 8;

    private static int worldBorder = 30;

    public static List<GameObject> streetLights = new List<GameObject> ();
    public static List<GameObject> houseLights = new List<GameObject> ();
    public static List<Vector3> houses = new List<Vector3> ();
    public static List<Vector3> road = new List<Vector3> ();
    public static List<Vector3> drains = new List<Vector3> ();
    public static int houseCount = 0;


    public static void StreetLight(Vector3 pos) {
        int height = 30;

        for(int y = (int)pos.y; y < (int)pos.y+height; y++) {
            if (y < (int)pos.y + height/2) {
                for (int x = (int)pos.x; x < (int)pos.x + 3; x++) {
                    for (int z = (int)pos.z; z < (int)pos.z + 3; z++) {
                        if (World.IsWithinWorld (x, y, z)) {
                            World.blocks [x, y, z] = streetLightColor;
                        }
                    }
                }
            } else {
                if (World.IsWithinWorld ((int)pos.x, y, (int)pos.z)) {
                    World.blocks [(int)pos.x + 1, y, (int)pos.z + 1] = streetLightColor;
                }
            }
        }
        GameObject lightGameObject = new GameObject("StreetLight");
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.color = Color.yellow;
        lightComp.type = LightType.Point;
        lightComp.range = 70;
        lightComp.intensity = 2.9f;
        lightGameObject.transform.position = new Vector3((int)pos.x+1, (int)pos.y+height+4, (int)pos.z+1);
        streetLights.Add (lightGameObject);

        if(World.IsWithinWorld((int)pos.x+1, (int)pos.y+height, (int)pos.z+1)) {
            World.blocks [(int)pos.x+1, (int)pos.y+height, (int)pos.z+1] = (255 & 0xFF) << 24 | (239 & 0xFF) << 16 | (20 & 0xFF) << 8;
        }

        for(int n = -2; n <= 2; n++) {
            if(n == -2 || n == 2) {
                // Build height
                for(int h = 0; h < 6; h++) {
                    if (World.IsWithinWorld ((int)pos.x + n + 1, (int)pos.y + height - 1 +h, (int)pos.z + 1)) {
                        World.blocks [(int)pos.x + n + 1, (int)pos.y + height - 1 + h, (int)pos.z + 1] = streetLightColor;
                    }
                    if(World.IsWithinWorld((int)pos.x + 1, (int)pos.y+height-1 +h, (int)pos.z+n + 1)) {
                        World.blocks [(int)pos.x + 1, (int)pos.y + height - 1 +h, (int)pos.z+n + 1] = streetLightColor;
                    }
                }
            }
            if(World.IsWithinWorld((int)pos.x+n + 1, (int)pos.y+height-1, (int)pos.z + 1)) {
                World.blocks [(int)pos.x + n + 1, (int)pos.y + height - 1, (int)pos.z +1] = streetLightColor;
            }
            if(World.IsWithinWorld((int)pos.x + 1, (int)pos.y+height-1, (int)pos.z+n + 1)) {
                World.blocks [(int)pos.x + 1, (int)pos.y + height - 1, (int)pos.z+n + 1] = streetLightColor;
            }
        }
        // Plate above light
        for(int xx = -1; xx <= 3; xx++) {
            for(int zz = -1; zz <= 3; zz++) {
                if(World.IsWithinWorld((int)pos.x + xx, (int)pos.y+height + 5, (int)pos.z + zz )) {
                    World.blocks [(int)pos.x + xx, (int)pos.y + height + 5, (int)pos.z + zz] = streetLightColor;
                }
            }
        }
    }

    public static void Drain(Vector3 pos) {
        int size = 6;

//        if (Random.value > 0.8) {
//            Steam.Create (new Vector3(pos.x-size/2+1, 8, pos.z-size/2-1));
//        }

        for(int x = (int)pos.x - size/2; x < (int)pos.x + size/2 - 1; x++ ) {
            for (int z = (int)pos.z - size / 2; z < (int)pos.z + size / 2 - 1; z++) {
                if(World.IsWithinWorld(x,6, z)) {
                    if (z % 2 == 0 ) {
                        World.blocks [x, 6, z] = drainColor;
                    } else {
                        World.blocks [x, 6, z] = 0;
                    }
                }
            }
        }
    }

    public static void Flower(Vector4 pos, int flowerColor) {
        int height = (int)Random.Range (1, 5);

        // Stem
        for(int y = (int)pos.y; y < (int)pos.y+height; y++) {
            if (World.IsWithinWorld ((int)pos.x, y, (int)pos.z)) {
                World.blocks [(int)pos.x, y, (int)pos.z] = flowerStemColor;
            }
        }
        // Flower crown
        if (World.IsWithinWorld ((int)pos.x, (int)pos.y+height, (int)pos.z)) {
            World.blocks [(int)pos.x, (int)pos.y+height, (int)pos.z] = flowerColor;
        }
        if (World.IsWithinWorld ((int)pos.x+1, (int)pos.y+height, (int)pos.z)) {
            World.blocks [(int)pos.x+1, (int)pos.y+height, (int)pos.z] = flowerColor;
        }
        if (World.IsWithinWorld ((int)pos.x-1, (int)pos.y+height, (int)pos.z)) {
            World.blocks [(int)pos.x-1, (int)pos.y+height, (int)pos.z] = flowerColor;
        }
        if (World.IsWithinWorld ((int)pos.x, (int)pos.y+height, (int)pos.z-1)) {
            World.blocks [(int)pos.x, (int)pos.y+height, (int)pos.z-1] = flowerColor;
        }
        if (World.IsWithinWorld ((int)pos.x, (int)pos.y+height, (int)pos.z+1)) {
            World.blocks [(int)pos.x, (int)pos.y+height, (int)pos.z+1] = flowerColor;
        }
    }

    public static void Grass(Vector3 pos) {
        int height = (int)Random.Range (1, 5);
        int color = grassColor1;
        if(Random.value > 0.6) {
            color = grassColor2;
        }

        for(int y = (int)pos.y; y < (int)pos.y+height; y++) {
            if (World.IsWithinWorld ((int)pos.x, y, (int)pos.z)) {
                World.blocks [(int)pos.x, y, (int)pos.z] = color;
            }
        }
    }

    public static void Tree(Vector3 pos) {
        int stem_height = (int)Random.Range (10, 20);
        int branch_height = (int)Random.Range(20, 30);
        //		int thickness = (int)Random.Range (4, );
        int thickness = (int)Random.Range(4,10);

        // Create stem
        for(int y = (int)pos.y; y < (int)pos.y+stem_height; y++) {
            for (int x = (int)pos.x - (int)thickness / 4; x < (int)pos.x + (int)thickness / 4; x++) {
                for (int z = (int)pos.z - (int)thickness / 4; z < (int)pos.z + (int)thickness / 4; z++) {
                    if (World.IsWithinWorld (x, y, z)) {
                        World.blocks [x, y, z] = stemColor;
                    }
                }
            }
        }
        // Create branches
        // Apples or not.
        bool fruit = false;
        if(Random.value > 0.9) {
            fruit = true;
        }
        for(int y = (int)pos.y+stem_height; y < (int)pos.y+stem_height+branch_height; y++) {
            for(int x = (int)pos.x - (int)thickness/2;  x < (int)pos.x + (int)thickness/2; x++) {
                for (int z = (int)pos.z - (int)thickness / 2; z < (int)pos.z + (int)thickness / 2; z++) {
                    if (World.IsWithinWorld (x, y, z)) {
                        if ((z > (pos.z + thickness / 2) - 2 || z < pos.z -thickness/2 + 2 || x > (pos.x + thickness/2) - 2 || x < pos.x - thickness/2 + 2) && Random.value > 0.8f) {
                            if(fruit && Random.value > 0.8) {
                                World.blocks [x, y, z] = fruitColor; 
                            }
                            //continue;
                        }
                        World.blocks [x, y, z] = treeColor;
                    }
                }
            }
        }
    }


    public static void Landscape() {
        // Draw ground (dirt + grass layers)
        for(int x = 0; x < World.rWidth; x++) {
            for (int z = 0; z < World.rDepth; z++) {
                World.blocks [x, 0, z] = dirtColor1;
                World.blocks [x, 1, z] = dirtColor1;
                World.blocks [x, 2, z] = dirtColor1;
                World.blocks [x, 3, z] = dirtColor2;
                World.blocks [x, 4, z] = dirtColor2;
                World.blocks [x, 5, z] = dirtColor2;
                if (Random.value > 0.95) {
                    World.blocks [x, 6, z] = grassColor1;
                } else {
                    World.blocks [x, 6, z] = grassColor1;
                }
            }
        }

        // Voronoi generated streets
        int polygonNumber = 20;
        Dictionary<Vector2f, Site> sites;
        List<Edge> edges;
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++) {
            points.Add(new Vector2f(Random.Range(worldBorder,World.rWidth-worldBorder), Random.Range(worldBorder,World.rDepth-worldBorder)));
        }
        Rectf bounds = new Rectf(worldBorder/(int)Random.Range(2, 4),worldBorder/(int)Random.Range(2,4),World.rWidth-worldBorder*(int)Random.Range(2,4),World.rDepth-worldBorder*(int)Random.Range(2,4));
        Voronoi voronoi = new Voronoi(points,bounds,10);
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
        foreach (Edge edge in edges) {
            if (edge.ClippedEnds == null)
                continue;
            if (Random.value > 0.88)
                continue;
            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT]);
        }

        // Streetlights
        for(int x = 0; x < World.rWidth-1; x+=2) {
            for(int z = 0; z < World.rDepth-1; z+=2) {
                // Search for side of road
                if(World.blocks[x, 6, z] == grassColor1 &&
                   (World.blocks[x+1, 6, z] == roadColor || World.blocks[x, 6, z+1] == roadColor))
                {
                    if (streetLights.Count == 0) {
                        StreetLight (new Vector3 (x, 6, z));
                    } else {
                        bool place = true;
                        foreach (GameObject v in streetLights) {
                            // Check distance.
                            if (Vector3.Distance (new Vector3 (x, 6, z), v.transform.position) < 100) {
                                place = false;
                                break;
                            }
                        }
                        if(place) {
                            StreetLight (new Vector3 (x, 6, z));
                        }
                    }
                }
            }
        }

        // Houses
        while(houseCount < 3){
            int x = (int)Random.Range (50, World.rWidth - 50);
            int z = (int)Random.Range (50, World.rDepth - 50);
            if (World.blocks [x, 6, z] == grassColor1) {
                bool roadPlace = true;
                bool lightPlace = true;
                bool housePlace = true;
                int minDistance = 0;
                // Check road
                foreach (Vector3 r in road) {
                    int dist = (int)Vector3.Distance (r, new Vector3 (x, 6, z));
                    if (dist < 70) {
                        roadPlace = false;
                        break;
                    }
                    if (minDistance > dist) {
                        minDistance = dist;
                    }
                }
                if (!roadPlace || minDistance > 150) {
                    continue;
                }

                // Check lights
                foreach (GameObject l in streetLights) {
                    if (Vector3.Distance (l.transform.position, new Vector3 (x, 6, z)) < 20) {
                        lightPlace = false;
                        break;
                    }
                }
                if (!lightPlace) {
                    continue;
                }

                // Check houses
                foreach (Vector3 v in houses) {
                    if (Vector3.Distance (v, new Vector3 (x, 6, z)) < 100) {
                        housePlace = false;
                        break;
                    }
                }
                if (!housePlace) {
                    continue;
                }
                House (new Vector3 (x, 6, z));
                houseCount++;
            }
        }

        // Flood fill and check where to make sidewalks and where to plant trees/stones/flowers
        List<Vector3> grassPos = new List<Vector3>();
        List<Vector3> housePos = new List<Vector3>();
        List<Vector4> flowerPos = new List<Vector4>();

        for (int x = 0; x < World.rWidth; x += 3) {
            for (int z = 0; z < World.rDepth; z += 3) {
                List<Vector2> res = null;
                List<Vector2> all = null;
                bool bRes = FloodFill (out res, out all, new Vector2 (x, z), 6, grassColor1);
                if (bRes && res.Count > 5) {
                    foreach (Vector2 b in res) {
                        World.blocks [(int)b.x, 7, (int)b.y] = sideWalkColor;
                        if(Random.value < 0.0003) {
                            housePos.Add (new Vector3((int)b.x, 7, (int)b.y));
                        }
                    }
                } else {
                    foreach (Vector2 b in res) {
                        if (Random.value > 0.9999) {
                            // Trees
                            Tree (new Vector3 ((int)b.x, 6, (int)b.y));
                        } else {
                            if (Random.value < 0.001) {
                                // Grass
                                for (int x1 = 0; x1 < (int)Random.Range (1, 20); x1++) {
                                    for (int z1 = 0; z1 < (int)Random.Range (1, 20); z1++) {
                                        grassPos.Add (new Vector3 ((int)b.x + x1, 6, (int)b.y + z1));
                                    }
                                }
                            } else if (Random.value < 0.0005) {
                                // Flowers
                                int fColor = (int)Random.Range(0, flowerColors.Length);
                                for (int x1 = 0; x1 < (int)Random.Range (1, 20); x1 += 3) {
                                    for (int z1 = 0; z1 < (int)Random.Range (1, 20); z1 += 3) {
                                        if (Random.value > 0.5) {
                                            if (World.IsWithinWorld ((int)b.x + x1, 6, (int)b.y + z1)) {
                                                if (World.blocks [(int)b.x + x1, 6, (int)b.y + z1] != roadColor) {
                                                    flowerPos.Add (new Vector4 ((int)b.x + x1, 6, (int)b.y + z1, (float)flowerColors [fColor]));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Draw grass after ff so we don't include grass in the actuall ff.
        foreach(Vector3 g in grassPos) {
            Grass (g);
        }

        foreach(Vector4 g in flowerPos) {
            Flower (g, (int)g.w );
        }

        foreach(Vector3 v in drains) {
            Drain (v);
        }
        // Rocks

    }

    // Flood fill from given point in 2d space and check for borders. No borders => True else false.
    private static bool FloodFill(out List<Vector2> result, out List<Vector2> all, Vector2 start, int y, int color) {
        result = new List<Vector2>();
        all = new List<Vector2> ();
        Stack stack = new Stack();
        stack.Push(start);
        bool ret = true;

        if((World.blocks[(int)start.x, y, (int)start.y] & 0x40)  != 0) {
            return false;
        }

        while(stack.Count != 0) {
            Vector2 b = (Vector2)stack.Pop();
            if(!World.IsWithinWorld((int)b.x, y,(int)b.y)) {
                // Reached border!
                ret = false;
                continue;
            }
            if ((World.blocks [(int)b.x, y, (int)b.y]) == color) {
                result.Add (b);
                World.blocks [(int)b.x, y, (int)b.y] |= 0x40;

                stack.Push (new Vector2 (b.x, b.y + 1)); 
                stack.Push (new Vector2 (b.x, b.y - 1)); 
                stack.Push (new Vector2 (b.x + 1, b.y)); 
                stack.Push (new Vector2 (b.x - 1, b.y)); 
            }
        }
        return ret;
    }

    private static void DrawLine(Vector2f p0, Vector2f p1, int offset = 0) {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1-x0);
        int dy = Mathf.Abs(y1-y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx-dy;

        while (true) {
            for (int x = 0; x < 20; x++) {
                for (int z = 0; z < 20; z++) {
                    if (World.IsWithinWorld (x0 + offset +x, 6, y0 + offset + z)) {
                        World.blocks [x0 + offset + x, 6, y0 + offset + z] = roadColor;
                        if(x == 10 && z == 10) {
                            bool place = true;
                            foreach(Vector3 d in drains) {
                                if(Vector3.Distance(d, new Vector3(x0+offset+x, 6, y0+offset+z)) < 50) {
                                    place = false;
                                }
                            }
                            if (place) {
                                drains.Add (new Vector3 (x0 + offset + x, 6, y0 + offset + z));
                            }
                        }
                    }
                }
            }
            if(x0 == 0) {
                if (World.IsWithinWorld (x0, 6, y0)) {
                    road.Add (new Vector3 (x0, 6, y0));
                }
            }

            if (x0 == x1 && y0 == y1) break;
            int e2 = 4*err;
            if (e2 > -dy) {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx) {
                err += dx;
                y0 += sy;
            }
        }
    }

    public static void House(Vector3 pos) {
        int color = wallColors [(int)Random.Range (0, wallColors.Length)];
        int sizeWidth  = (int)Random.Range (20, 50);
        int sizeDepth = (int)Random.Range (20, 50);
        // Base height on where on map the house will be located (center => higher)
        int floor_height = 15;
        int plate_height = 2;

        int floors = (int)Random.Range (1, 2);
        if(pos.x > 200 && pos.x < World.rWidth - 200 && pos.z < World.rDepth - 200 && pos.z > 200) {
            floors = (int)Random.Range (3, 5);
        }


        // TBD: Check that the house isn't going to be higher than World.rHeight, then re-calc random values.

        // Generate ground for the house.
        for (int p = 0; p < plate_height; p++) {
            Plate (pos, sizeWidth, sizeDepth, (int)pos.y + p);
        }
        Plate (pos, sizeWidth+6, sizeDepth+6, (int)pos.y + 1);

        int windowWidthW = sizeWidth/3;
        int windowWidthD = sizeDepth/3;

        float createWindows1 = Random.value;
        float createWindows2 = Random.value;
        float createWindows3 = Random.value;
        float createWindows4 = Random.value;

        // Which side the doors should be on.
        int doors = (int)Random.Range (1, 4);


        // Generate floors
        for (int f = 0; f < floors; f++) {
            // Place light?
            if (Random.value > 0.7) {
                GameObject lightGameObject = new GameObject ("HouseLight");
                Light lightComp = lightGameObject.AddComponent<Light> ();
                lightComp.color = Color.yellow;
                lightComp.type = LightType.Point;
                lightComp.range = 50;
                lightComp.intensity = 2.0f;
                lightGameObject.transform.position = new Vector3 ((int)pos.x + 1, (int)pos.y + f*floor_height + floor_height/2, (int)pos.z);
                houseLights.Add (lightGameObject);
            }
            for (int fh = 0; fh < floor_height; fh++) {
                int y = (int)pos.y + fh + plate_height + f*floor_height;
                for (int x = (int)pos.x - sizeWidth / 2; x <= (int)pos.x + sizeWidth / 2; x++) {
                    int z1 = (int)pos.z - sizeDepth / 2;
                    int z2 = (int)pos.z + sizeDepth / 2;
                    if (World.IsWithinWorld (x, y, z1)) {
                        // TBD: Doors if (f == 0)
                        if (f == 0 && doors == 1) {
                            if (x > (int)pos.x - sizeWidth / 5 && x < (int)pos.x - sizeWidth / 5 + windowWidthW && fh > 1 && fh < floor_height - 2) {
                                if (World.IsWithinWorld (x, y, z1 - 1)) {
                                    World.blocks [x - 1, y, z1 - 1] = doorColor;
                                    //									if (fh == 1) {
                                    //										World.blocks [x + 1, y, z1 - 1] = sideWalkColor;
                                    //										World.blocks [x + 1, y - 1, z1 - 1] = sideWalkColor;
                                    //										World.blocks [x + 2, y - 2, z1 - 1] = sideWalkColor;
                                    //									}
                                }
                            } else {
                                World.blocks [x, y, z1 - 1] = color;
                            }
                            //Create window + window seal.
                        } else if (createWindows1 > 0.5) { // Might create windows for this side...
                            if (x > (int)pos.x - sizeWidth / 3 && x < (int)pos.x - sizeWidth / 3 + windowWidthW && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x, y, z1 - 1)) {
                                        World.blocks [x, y, z1 - 1] = sideWalkColor;
                                    }
                                }
                            } else if (x > (int)pos.x + sizeWidth / 3 - windowWidthW && x < (int)pos.x + sizeWidth / 3 && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x, y, z1 - 1)) {
                                        World.blocks [x, y, z1 - 1] = sideWalkColor;
                                    }
                                }
                            } else {
                                World.blocks [x, y, z1] = color;
                            }
                        } else {
                            World.blocks [x, y, z1] = color;
                        }
                    }
                    if (World.IsWithinWorld (x, y, z2)) {
                        // Create window + window seal.
                        if (createWindows2 > 0.5) { // Might create windows for this side...
                            if (x > (int)pos.x - sizeWidth / 3 && x < (int)pos.x - sizeWidth / 3 + windowWidthW && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x, y, z2 + 1)) {
                                        World.blocks [x, y, z2 + 1] = sideWalkColor;
                                    }
                                }
                            } else if (x > (int)pos.x + sizeWidth / 3 - windowWidthW && x < (int)pos.x + sizeWidth / 3 && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x, y, z2 + 1)) {
                                        World.blocks [x, y, z2 + 1] = sideWalkColor;
                                    }
                                }
                            } else {
                                World.blocks [x, y, z2] = color;
                            }
                        } else {
                            World.blocks [x, y, z2] = color;
                        }
                    }
                }
                for (int z = (int)pos.z - sizeDepth / 2; z <= (int)pos.z + sizeDepth / 2; z++) {
                    int x1 = (int)pos.x - sizeWidth / 2;
                    int x2 = (int)pos.x + sizeWidth / 2;
                    if (World.IsWithinWorld (x1, y, z)) {
                        if (createWindows3 > 0.5) { // Might create windows for this side...
                            if (z > (int)pos.z - sizeDepth / 3 && z < (int)pos.z - sizeDepth / 3 + windowWidthD && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x1 - 1, y, z)) {
                                        World.blocks [x1 - 1, y, z] = sideWalkColor;
                                    }
                                }
                            } else if (z > (int)pos.z + sizeDepth / 3 - windowWidthD && z < (int)pos.z + sizeDepth / 3 && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x1 - 1, y, z)) {
                                        World.blocks [x1 - 1, y, z] = sideWalkColor;
                                    }
                                }
                            } else {
                                World.blocks [x1, y, z] = color;
                            }
                        } else {
                            World.blocks [x1, y, z] = color;
                        }
                    }
                    if (World.IsWithinWorld (x2, y, z)) {
                        if (createWindows4 > 0.5) { // Might create windows for this side...
                            if (z > (int)pos.z - sizeDepth / 3 && z < (int)pos.z - sizeDepth / 3 + windowWidthD && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x2 + 1, y, z)) {
                                        World.blocks [x2 + 1, y, z] = sideWalkColor;
                                    }
                                }
                            } else if (z > (int)pos.z + sizeDepth / 3 - windowWidthD && z < (int)pos.z + sizeDepth / 3 && fh > 3 && fh < floor_height - 3) {
                                if (fh == 4) {
                                    if (World.IsWithinWorld (x2 + 1, y, z)) {
                                        World.blocks [x2 + 1, y, z] = sideWalkColor;
                                    }
                                }
                            } else {
                                World.blocks [x2, y, z] = color;
                            }
                        } else {
                            World.blocks [x2, y, z] = color;
                        }
                    }
                }
            }
            Plate (pos, sizeWidth, sizeDepth, (int)pos.y+floor_height*f+plate_height);
        }
        // 

        // Generate roof
        Roof (pos, sizeWidth, sizeDepth, (int)pos.y + floor_height*floors+plate_height, floors);

        // Make water drains
        for(int y = (int)pos.y+2; y < (int)pos.y+floor_height*floors+plate_height; y++) {
            if(World.IsWithinWorld((int)pos.x + sizeWidth / 2 , y, (int)pos.z + sizeDepth / 2 + 1)) {
                World.blocks [(int)pos.x + sizeWidth / 2, y, (int)pos.z + sizeDepth / 2 + 1] = drainColor;
            }
        }
        if(World.IsWithinWorld((int)pos.x + sizeWidth / 2,  (int)pos.y+2, (int)pos.z + sizeDepth / 2 + 2)) {
            World.blocks [(int)pos.x + sizeWidth / 2, (int)pos.y+2, (int)pos.z + sizeDepth / 2 + 2] = drainColor;
        }


    }

    public static void Roof(Vector3 pos, int sizeWidth, int sizeDepth, int y, int floors) {
        int color = roofColors [(int)Random.Range (0, roofColors.Length)];
        int borderHeight = (int)Random.Range (2, 6);

        int type = 0;
        if(floors > 2) {
            type = 0;
        } else {
            type = (int)Random.Range (0, 2); // Add types
        }
        // 
        if(type == 0) {
            // Regular roof with boundaries
            for(int x = (int)pos.x - sizeWidth / 2; x < (int)pos.x + sizeWidth/2 + 1; x++) {
                for(int z = (int)pos.z - sizeDepth/2; z < (int)pos.z + sizeDepth/2 + 1; z++) {
                    if(World.IsWithinWorld(x, y, z)) {
                        World.blocks [x, y, z] = color;
                    }
                    if(x < (int)pos.x - sizeWidth/2 + 3 || x > (int)pos.x+sizeWidth/2+1 - 3) {
                        for (int yy = 0; yy < borderHeight; yy++) {
                            if (World.IsWithinWorld (x, y+yy, z)) {
                                World.blocks [x, y+yy, z] = color;
                            }
                        }
                    }
                    if(z < (int)pos.z - sizeDepth/2 + 3 || z > (int)pos.z+sizeDepth/2+1 - 3) {
                        for (int yy = 0; yy < borderHeight; yy++) {
                            if (World.IsWithinWorld (x, y+yy, z)) {
                                World.blocks [x, y+yy, z] = color;
                            }
                        }
                    }
                }
            }
        } else if(type == 1) {
            // Add slope-style roof
            int rh = 0;
            for(int x = (int)pos.x - sizeWidth / 2 - 1; x < (int)pos.x + sizeWidth/2 + 2; x++) {
                if (x < (int)pos.x + 1) {
                    rh++;
                } else {
                    rh--;
                }
                for(int z = (int)pos.z - sizeDepth/2 - 1; z < (int)pos.z + sizeDepth/2 + 2; z++) {
                    if(World.IsWithinWorld(x, y+rh, z)) {
                        World.blocks [x, y+rh, z] = color;
                    }
                    if(x < (int)pos.x - sizeWidth/2 + 2 || x > (int)pos.x+sizeWidth/2+1 - 2) {
                        if (World.IsWithinWorld (x, y, z)) {
                            World.blocks [x, y, z] = color;
                        }
                    }
                    if((z > (int)pos.z - sizeDepth/2 && z <= (int)pos.z - sizeDepth/2 + 1) ||
                       (z > (int)pos.z+sizeDepth/2 - 2 && z <= (int)pos.z+sizeDepth/2 - 1)) 
                    {
                        for (int yy = 0; yy < rh; yy++) {
                            if (World.IsWithinWorld (x, y+yy, z)) {
                                World.blocks [x, y+yy, z] = color;
                            }
                        }
                    }
                }
            }
        } else if(type == 2) {
            // Add pyramid roof
            int rh = 0;
            for(int x = (int)pos.x - sizeWidth / 2 - 1; x < (int)pos.x + sizeWidth/2 + 2; x++) {
                if (x < (int)pos.x + 1) {
                    rh++;
                } else {
                    rh--;
                }
                for(int z = (int)pos.z - sizeDepth/2 - 1 +rh; z < (int)pos.z + sizeDepth/2 + 2 - rh; z++) {
                    if(World.IsWithinWorld(x, y+rh-1, z)) {
                        World.blocks [x, y+rh-1, z] = color;
                    }
                }
            }
            for(int z = (int)pos.z - sizeDepth/2 ; z < (int)pos.z + sizeDepth/2 + 1 ; z++) {
                if (z < (int)pos.z + 1) {
                    rh++;
                } else {
                    rh--;
                }
                for(int x = (int)pos.x - sizeWidth / 2 + rh - 3; x < (int)pos.x + sizeWidth/2 - rh + 3; x++) {
                    if(World.IsWithinWorld(x, y+rh-2, z)) {
                        World.blocks [x, y+rh-2, z] = color;
                    }
                }
            }
        } else if(type == 3) {
            // TBD: Add extra large roof (size += 2)
        } else if(type == 4) {
            // TBD: valve
        }
    }

    public static void Plate(Vector3 pos, int sizeWidth, int sizeDepth, int y) {
        for(int x = (int)pos.x - sizeWidth / 2; x < (int)pos.x + sizeWidth/2 + 1; x++) {
            for(int z = (int)pos.z - sizeDepth/2; z < (int)pos.z + sizeDepth/2 + 1; z++) {
                if(World.IsWithinWorld(x, y, z)) {
                    World.blocks [x, y, z] = sideWalkColor;
                    houses.Add (new Vector3 (x, y, z));
                }
            }
        }
    }
}
