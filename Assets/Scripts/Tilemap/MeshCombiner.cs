using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshCombiner : MonoBehaviour {

	public int vertexLimit = 10000;

	public Transform meshParent;

	public GameObject lushMeshPrefab;
	public GameObject grassMeshPrefab;
	public GameObject mudMeshPrefab;
	public GameObject sandMeshPrefab;
	public GameObject waterMeshPrefab;

	List<GameObject> lushHolder = new List<GameObject>();
	List<GameObject> grassHolder = new List<GameObject>();
	List<GameObject> mudHolder = new List<GameObject>();
	List<GameObject> sandHolder = new List<GameObject>();
	List<GameObject> waterHolder = new List<GameObject>();

	List<GameObject> waitingToAdd = new List<GameObject>();
	List<GameObject> waitingToRemove = new List<GameObject>();

	public void CreateTilemapMesh(List<GameObject> tiles) {

		CombineInstance combine = new CombineInstance();

		List<CombineInstance> lush = new List<CombineInstance>();
		List<CombineInstance> grass = new List<CombineInstance>();
		List<CombineInstance> mud = new List<CombineInstance>();
		List<CombineInstance> sand = new List<CombineInstance>();
		List<CombineInstance> water = new List<CombineInstance>();

		int verticesSoFar = 0;
		int meshListCounter = 0;

		//for each tile, grab their meshfilter and add according to their material type
		foreach(GameObject go in tiles) {

			Tile tile = go.GetComponent<Tile>();
			tile.id = meshListCounter;

			//hide original tile object
			go.SetActive(false);

			MeshFilter mf = go.GetComponent<MeshFilter>();
			string material = mf.GetComponent<MeshRenderer>().material.name.Replace(" (Instance)", "");

			combine.mesh = mf.mesh;
			combine.transform = mf.transform.localToWorldMatrix;

			if (tile.terrainType == Terrain.Lush)
				lush.Add(combine);
			else if (tile.terrainType == Terrain.Grass)
				grass.Add(combine);
			else if (tile.terrainType == Terrain.Mud)
				mud.Add(combine);
			else if (tile.terrainType == Terrain.Sand)
				sand.Add(combine);
			else if (tile.terrainType == Terrain.Water)
				water.Add(combine);

			verticesSoFar += mf.mesh.vertexCount;

			if(verticesSoFar >= vertexLimit) {

				CombineMesh(lush, lushMeshPrefab, lushHolder);
				CombineMesh(grass, grassMeshPrefab, grassHolder);
				CombineMesh(mud, mudMeshPrefab, mudHolder);
				CombineMesh(sand, sandMeshPrefab, sandHolder);
				CombineMesh(water, waterMeshPrefab, waterHolder);

				lush.Clear();
				grass.Clear();
				mud.Clear();
				sand.Clear();
				water.Clear();

				verticesSoFar = 0;
				meshListCounter++;

			}

		}

		CombineMesh(lush, lushMeshPrefab, lushHolder);
		CombineMesh(grass, grassMeshPrefab, grassHolder);
		CombineMesh(mud, mudMeshPrefab, mudHolder);
		CombineMesh(sand, sandMeshPrefab, sandHolder);
		CombineMesh(water, waterMeshPrefab, waterHolder);

	}

	List<GameObject> GetMeshHolderOfType(Terrain type) {

		if (type == Terrain.Lush)
			return lushHolder;
		else if (type == Terrain.Grass)
			return grassHolder;
		else if (type == Terrain.Mud)
			return mudHolder;
		else if (type == Terrain.Sand)
			return sandHolder;
		else if (type == Terrain.Water)
			return waterHolder;
		return null;

	}

	void CombineMesh(List<CombineInstance> dataList, GameObject holderObj, List<GameObject> holderList) {

		Mesh newMesh = new Mesh();
		newMesh.CombineMeshes(dataList.ToArray());

		GameObject meshHolder = Instantiate(holderObj, Vector3.zero, Quaternion.identity);
		meshHolder.transform.SetParent(meshParent);
		meshHolder.GetComponent<MeshFilter>().mesh = newMesh;
		meshHolder.AddComponent<MeshCollider>();
		holderList.Add(meshHolder);

	}

	const float tolerance = 0.0001f;

	void RemoveMeshFromCombinedMesh(Transform combinedMesh, Transform objToRemove, Vector3 moveVec = default(Vector3)) {


		//get data from combined mesh
		MeshFilter combinedMF = combinedMesh.GetComponent<MeshFilter>();
		List<Vector3> currentVertices = combinedMF.mesh.vertices.ToList();
		List<int> currentTriangles = combinedMF.mesh.triangles.ToList();

		//get data from mesh that we are removing
		Mesh meshToRemove = objToRemove.GetComponent<MeshFilter>().mesh;
		Vector3[] verticesToRemove = meshToRemove.vertices;
		int numOfVerticesToRemove = verticesToRemove.Length;

		//position to begin from
		int minVerticePos = 0;

		Vector3 firstVertPosToRemove = objToRemove.transform.TransformPoint(verticesToRemove[0]);
		firstVertPosToRemove = combinedMesh.InverseTransformPoint(firstVertPosToRemove) + moveVec;

		for (int i = 0; i < currentVertices.Count; i++) {

			if ((currentVertices[i] - firstVertPosToRemove).sqrMagnitude < tolerance) {
				minVerticePos = i;
				break;
			}

		}

		currentVertices.RemoveRange(minVerticePos, numOfVerticesToRemove);

		int minTrianglePos = 0;
		bool hasFoundStart = false;
		int firstTrianglePos = meshToRemove.triangles[0] + minVerticePos;

		int upperLimit = minVerticePos + numOfVerticesToRemove;

		for (int i = 0; i < currentTriangles.Count; i++) {
			int currentTrianglePos = currentTriangles[i];

			if (currentTrianglePos == firstTrianglePos && !hasFoundStart) {
				hasFoundStart = true;
				minTrianglePos = i;
			}

			//Change which vertices the triangles are being built from
			//We only need to shift the triangles that come after the triangles we remove
			if (currentTrianglePos >= upperLimit) {
				currentTriangles[i] = currentTrianglePos - numOfVerticesToRemove;
			}
		}

		//Remove the triangles we dont need
		currentTriangles.RemoveRange(minTrianglePos, meshToRemove.triangles.Length);

		combinedMF.mesh.Clear();
		combinedMF.mesh.vertices = currentVertices.ToArray();
		combinedMF.mesh.triangles = currentTriangles.ToArray();
		combinedMF.mesh.RecalculateNormals();

	}


	//public void RemoveTile(GameObject go) {

	//	//remove from add list if it's there
	//	waitingToAdd.Remove(go);

	//	//add to remove list
	//	waitingToRemove.Add(go);

	//	//hide tile
	//	HideTile(go);

	//}

	//void HideTile(GameObject go) {

	//	Tile tile = go.GetComponent<Tile>();
	//	int meshIndex = tile.id;
	//	if (meshIndex == -1) {
	//		go.SetActive(false);
	//		return;
	//	}
	//	Transform combinedMesh = GetMeshHolderOfType(tile.terrainType)[meshIndex].transform;
	//	MoveVerticesOutOfTheWay(combinedMesh, go.transform, new Vector3(0, 0, 0));

	//}

	public void ReplaceTile(GameObject oldTileObj, GameObject newTileObj) {

		Tile oldTile = oldTileObj.GetComponent<Tile>();
		int meshIndex = oldTile.id;

		if (meshIndex != -1) {

			//we just need to get the transform of the larger mesh that the tile is part of
			//	and remove this object's mesh from the larger mesh
			MeshFilter combinedMesh = GetMeshHolderOfType(oldTile.terrainType)[meshIndex].GetComponent<MeshFilter>();
			RemoveMeshFromCombinedMesh(combinedMesh.transform, oldTileObj.transform, new Vector3(0, 0, 0));

			//destroy old tile gameobject
			Destroy(oldTileObj);

			//now add new tile to where the old one previously was
			MeshFilter newTileMF = newTileObj.GetComponent<MeshFilter>();
			Tile newTile = newTileObj.GetComponent<Tile>();
			AddMeshToCombinedMesh(newTileMF, combinedMesh);

			//hide original new tile and set its ID
			newTileObj.SetActive(false);
			newTile.id = meshIndex;

		}

	}

	void AddMeshToCombinedMesh(MeshFilter objToAdd, MeshFilter combinedMesh) {
		
		CombineInstance[] combined = new CombineInstance[2];

		combined[0].mesh = objToAdd.mesh;
		combined[0].transform = objToAdd.transform.localToWorldMatrix;

		combined[1].mesh = combinedMesh.mesh;
		combined[1].transform = combinedMesh.transform.localToWorldMatrix;
		
		Mesh newMesh = new Mesh();
		newMesh.CombineMeshes(combined);
		
		combinedMesh.mesh = newMesh;

	}

	

}
