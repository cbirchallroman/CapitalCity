using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkplaceMonthScheduler : MonoBehaviour {

	public Text label;
	public Toggle toggle;
	public Workplace building;
	public int seasonIndex;

	public void Start() {

		label.text = (Season)seasonIndex + "";
		toggle.isOn = building.ActiveSchedule[seasonIndex];

	}

	public void SetSchedule(bool open) {

		building.ActiveSchedule[seasonIndex] = open;
		if (building.ActiveBuilding != building.ActiveSchedule[building.time.Seasons])
			building.ToggleLabor(building.ActiveSchedule[building.time.Seasons]);

	}

}
