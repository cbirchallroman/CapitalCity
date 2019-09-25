using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProleInfo : MonoBehaviour {
	
	public Text employeeName;
	public Text relevantScore;
	public Image relevantAbility;
	public Workplace WP { get; set; }
	public Prole Employee { get; set; }

	private void Start() {

		if (Employee == null)
			return;

		UpdateLabels();

	}

	private void Update() {

		if (Employee == null) {
			Destroy(gameObject);
			return;
		}

		if (Employee.workNode == null)
			Destroy(gameObject);

		UpdateLabels();

	}

	void UpdateLabels() {

		employeeName.text = Employee.FullName + " (" + Employee.yearsOld + ")";
		relevantScore.text = Employee.GetLaborScore(WP.laborType) + "";
		relevantAbility.sprite = Resources.Load<Sprite>("Sprites/" + WP.laborType);

	}

	public void GoToHouse() {

		Node homeNode = Employee.homeNode;

		if (homeNode == null)
			return;

		Structure go = WP.world.GetBuildingAt(homeNode);
		WP.world.MoveMainCameraTo(go.GetComponent<Obj>());
		go.GetComponent<Obj>().OpenWindow();

	}

	public void FireEmployee() {

		Employee.QuitWork();
		Destroy(gameObject);

	}

}
