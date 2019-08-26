using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour {

	public Transform meshParent;

	public GameObject lushMesh;
	public GameObject grassMesh;
	public GameObject mudMesh;
	public GameObject sandMesh;
	public GameObject waterMesh;

	List<GameObject> lushHolder = new List<GameObject>();
	List<GameObject> grassHolder = new List<GameObject>();
	List<GameObject> mudHolder = new List<GameObject>();
	List<GameObject> sandHolder = new List<GameObject>();
	List<GameObject> waterHolder = new List<GameObject>();

	public void CombineMeshes(List<GameObject> tiles) {

		List<CombineInstance> lush = new List<CombineInstance>();
		List<CombineInstance> grass = new List<CombineInstance>();
		List<CombineInstance> mud = new List<CombineInstance>();
		List<CombineInstance> sand = new List<CombineInstance>();
		List<CombineInstance> water = new List<CombineInstance>();

		CombineInstance combine = new CombineInstance();
		
		int meshListCounter = 0;

		//for each tile, grab their meshfilter and add according to their material type
		foreach(GameObject go in tiles) {

			go.SetActive(false);
			//go.GetComponent<Tile>().id = meshListCounter;

			MeshFilter mf = go.GetComponent<MeshFilter>();
			string material = mf.GetComponent<MeshRenderer>().material.name.Replace(" (Instance)", "");

			combine.mesh = mf.mesh;
			combine.transform = mf.transform.localToWorldMatrix;

			if (material == "Lush")
				lush.Add(combine);
			else if (material == "Grass")
				grass.Add(combine);
			else if (material == "Mud")
				mud.Add(combine);
			else if (material == "Sand")
				sand.Add(combine);
			else if (material == "Water")
				water.Add(combine);

		}

		CombineMesh(lush, lushMesh, lushHolder);
		CombineMesh(grass, grassMesh, grassHolder);
		CombineMesh(mud, mudMesh, mudHolder);
		CombineMesh(sand, sandMesh, sandHolder);
		CombineMesh(water, waterMesh, waterHolder);

	}

	void CombineMesh(List<CombineInstance> dataList, GameObject holderObj, List<GameObject> holderList) {

		Mesh newMesh = new Mesh();
		newMesh.CombineMeshes(dataList.ToArray());

		GameObject meshHolder = Instantiate(holderObj, Vector3.zero, Quaternion.identity);
		meshHolder.transform.SetParent(meshParent);
		meshHolder.GetComponent<MeshFilter>().mesh = newMesh;
		holderList.Add(meshHolder);

	}

}
