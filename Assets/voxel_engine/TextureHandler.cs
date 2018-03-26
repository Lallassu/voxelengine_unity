using UnityEngine;
using System.Collections;

public class TextureHandler : MonoBehaviour {
//	public Texture2D floor;
//	public Texture2D wall;
//	public Texture2D wall2;
//	public Texture2D box;
//	public Texture2D box2;
//	public Texture2D world1;
//	public Texture2D pillars;
//	public Texture2D fence;

	public Color32 floor;
	public Color32 wall;
	public Color32 pillars;
	public Color32 fence;
	public Color32 bridge;

	public int GetColor(Color32 type) {
		return (type.r & 0xFF) << 24 | (type.g & 0xFF) << 16 | (type.b & 0xFF) << 8;
	}


//	public int GetCarpetColor(int x, int y, int size) {
//		float y_span = y / floor.width;
//		float x_span = x / floor.height;
//
//		int xx = (int)(x - floor.height * x_span);
//		int yy = (int)(y - floor.width * y_span);
//
//		Color c = carpet.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}


//	public int GetBoxColor(int x, int y, int size) {
//		x = x*(box.width/size);
//		y = y*(box.height/size);
//		float y_span = y / box.width;
//		float x_span = x / box.height;
//
//		int xx = (int)(x - (x_span * box.height));
//		int yy = (int)(y - (y_span * box.width));
//
//		Color c = box.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
//
//	public int GetBoxColor2(int x, int y, int size) {
//		x = x*(box2.width/size);
//		y = y*(box2.height/size);
//		float y_span = y / box2.width;
//		float x_span = x / box2.height;
//
//		int xx = (int)(x - (x_span * box2.height));
//		int yy = (int)(y - (y_span * box2.width));
//
//		Color c = box2.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
//
//	public int GetGroundColor(int x, int y) {
//		float y_span = y / floor.width;
//		float x_span = x / floor.height;
//
//		int xx = (int)(x - floor.height * x_span);
//		int yy = (int)(y - floor.width * y_span);
//
//		Color c = floor.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
//
//	public int GetFenceColor(int x, int y) {
//		float y_span = y / fence.width;
//		float x_span = x / fence.height;
//
//		int xx = (int)(x - fence.height * x_span);
//		int yy = (int)(y - fence.width * y_span);
//
//		Color c = fence.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
//
//	public int GetGroundColor2(int x, int y) {
//		float y_span = y / wall2.width;
//		float x_span = x / wall2.height;
//
//		int xx = (int)(x - wall2.height * x_span);
//		int yy = (int)(y - wall2.width * y_span);
//
//		Color c = wall2.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
//
//	public int GetWallColor(int x, int y) {
//		float y_span = y / wall.width;
//		float x_span = x / wall.height;
//
//		int xx = (int)(x - wall.height * x_span);
//		int yy = (int)(y - wall.width * y_span);
//
//		Color c = wall.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
//
//	public int GetWallColor2(int x, int y) {
//		float y_span = y / wall2.width;
//		float x_span = x / wall2.height;
//
//		int xx = (int)(x - wall2.height * x_span);
//		int yy = (int)(y - wall2.width * y_span);
//
//		Color c = wall2.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
//
//
//	public Texture2D GetWorldMap() {
//		return world1;	
//	}
//
//	public int GetWorldColor(int x, int y) {
//		float y_span = y / world1.width;
//		float x_span = x / world1.height;
//
//		int xx = (int)(x - world1.height * x_span);
//		int yy = (int)(y - world1.width * y_span);
//
//		Color c = world1.GetPixel (xx, yy);
//		return ((int)(c.r * 255) & 0xFF) << 24 | ((int)(c.g * 255) & 0xFF) << 16 | ((int)(c.b * 255) & 0xFF) << 8;
//	}
}
