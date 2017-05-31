using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBackground : MonoBehaviour {

    public Transform bg;
    public float tile_spacing;

	// Use this for initialization
	void Start () {
        for (float x = -10; x <= 10; x+=tile_spacing) {
            for (float y = -4; y <= 4; y+=tile_spacing) {
                Instantiate(bg, new Vector3(x, y, 5), Quaternion.identity);
            }
        }
	}
}
