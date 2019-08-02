using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomModel : MonoBehaviour {

	public GameObject[] models;
	public int Model { get; set; }
	public int Rotation { get; set; }

	public void NewModel() {

		Model = Random.Range(0, models.Length);
		for (int i = 0; i < models.Length; i++)
			models[i].SetActive(i == Model);

		Rotation = Random.Range(0, 4);
		models[Model].transform.localRotation = Quaternion.Euler(new Vector3(-90, Rotation * 90, 0));

	}

	public void LoadModel(int m, int r) {

		Model = m;
		for (int i = 0; i < models.Length; i++)
			models[i].SetActive(i == Model);


		Rotation = r;
		models[Model].transform.localRotation = Quaternion.Euler(new Vector3(-90, Rotation * 90, 0));

	}

}
