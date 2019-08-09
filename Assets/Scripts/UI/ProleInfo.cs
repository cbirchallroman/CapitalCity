using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProleInfo : MonoBehaviour {
	
	public Text employeeName;
	public Workplace WP { get; set; }
	public Prole Employee { get; set; }

	private void Start() {

		if (Employee == null)
			return;

		employeeName.text = Employee.FullName;
		employeeName.gameObject.SetActive(true);

	}

	private void Update() {

		if (Employee == null) {
			Destroy(gameObject);
			return;
		}

		if (Employee.workNode == null)
			Destroy(gameObject);

	}

	public void GoToHouse() {

		Node homeNode = Employee.homeNode;

		if (homeNode == null)
			return;

		GameObject go = WP.world.Map.GetBuildingAt(homeNode);
		WP.world.MoveMainCameraTo(go.GetComponent<Obj>());
		go.GetComponent<Obj>().OpenWindow();

	}

	public void FireEmployee() {

		Employee.QuitWork();
		Destroy(gameObject);

	}

}
