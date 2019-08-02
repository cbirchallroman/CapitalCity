using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {

    World mapdata;
    int sizex;
    int sizey;

    public Texture2D terrainTiles;
    public int tileResolution = 256;

    public void BuildMesh(World w) {

        mapdata = w;

        sizex = (int)mapdata.size.x;
        sizey = (int)mapdata.size.y;
		
		int numTiles = sizex * sizey;
		int numTris = numTiles * 2;
		
		int vsize_x = sizex + 1;
		int vsize_y = sizey + 1;
		int numVerts = vsize_x * vsize_y;
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, y;
		for(y=0; y < vsize_y; y++) {
			for(x=0; x < vsize_x; x++) {

                float elevation = 0;

                vertices[ y * vsize_x + x ] = new Vector3( x, elevation / 2, y );
				normals[ y * vsize_x + x ] = Vector3.up;
				uv[ y * vsize_x + x ] = new Vector2( (float)x / vsize_x, (float)y / vsize_y );


			}
		}
		
		for(y=0; y < sizey; y++) {
			for(x=0; x < sizex; x++) {
				int squareIndex = y * sizex + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = y * vsize_x + x + 		   0;
				triangles[triOffset + 1] = y * vsize_x + x + vsize_x + 0;
				triangles[triOffset + 2] = y * vsize_x + x + vsize_x + 1;
				
				triangles[triOffset + 3] = y * vsize_x + x + 		   0;
				triangles[triOffset + 4] = y * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 5] = y * vsize_x + x + 		   1;
			}
		}
		
		
		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;

        BuildTexture();
		
	}

    Dictionary<int, Color[]> ChopTextures() {

        int numTilesPerRow = terrainTiles.width / tileResolution;
        int numTilesPerColumn = terrainTiles.height / tileResolution;

        Dictionary<int, Color[]> tiles = new Dictionary<int, Color[]>();
        int index = 0;

        for(int x = 0; x < numTilesPerRow; x++)
            for(int y = 0; y < numTilesPerColumn; y++) {
                
                Color[] p = terrainTiles.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
                tiles[index] = p;
                index++;

            }

        return tiles;

    }

    void BuildTexture() {

        int texwidth = (sizex+1) * tileResolution;
        int texheight = (sizey+1) * tileResolution;
        
        Texture2D texture = new Texture2D(texwidth, texheight);
        Dictionary<int, Color[]> tiles = ChopTextures();

        for (int x = 0; x < sizex; x++)
            for (int y = 0; y < sizey; y++){

                int xcoord = x * tileResolution;
                int ycoord = y * tileResolution;

                int terrain = mapdata.terrain[x, y];
                texture.SetPixels(xcoord, ycoord, tileResolution, tileResolution, tiles[terrain]);

            }
        
        texture.Apply();
        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterials[0].mainTexture = texture;

    }

    public void PaintTile(int x, int y) {

        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        Texture2D texture = (Texture2D)mesh_renderer.sharedMaterials[0].mainTexture;
        Dictionary<int, Color[]> tiles = ChopTextures();

        int xcoord = x * tileResolution;
        int ycoord = y * tileResolution;

        int terrain = mapdata.terrain[x, y];
        texture.SetPixels(xcoord, ycoord, tileResolution, tileResolution, tiles[terrain]);

        texture.Apply();

    }
	
}
