using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkplaceListItem : MonoBehaviour {

	public Text buildingName;
	public Text wagesPerHour;
	public Text workers;
	public Text wagesPerMonth;
	public Workplace Building { get; set; }

	public void Start() {

		UpdateLabels();

	}

	private void Update() {

		if (Building == null)
			Destroy(this);
		UpdateLabels();

	}

	public void UpdateLabels() {

		buildingName.text = Building.DisplayName;
		wagesPerHour.text = MoneyController.symbol + Building.baseWages.ToString("n2") + "/hr";
		workers.text = Building.WorkerList.Count + "/" + Building.workersMax + " Proles";
		wagesPerMonth.text = MoneyController.symbol + Building.WagesOverall.ToString("n2") + "/mo";

	}

	public void IncreaseWages() {
		
		Building.baseWages += 0.05f;

	}

	public void DecreaseWages() {

		if(Building.baseWages > 0)
			Building.baseWages -= 0.05f;

	}

}
