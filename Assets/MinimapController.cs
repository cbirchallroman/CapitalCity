using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour {

    public WorldController world;

    public void Start() {

        Node size = world.Map.size;
        float x = size.x / 2 - .5f;
        float y = size.x - 3 * size.x / 25;
        float z = size.x / 2 - .5f;

        transform.position = new Vector3(x, y, z);

    }

}
