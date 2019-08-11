using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobcentreWindow : ObjWindow {

	[Header("Jobcentre")]
	public Transform prospectGrid;
	public Toggle highPhy;
	public Toggle highInt;
	public Toggle highEmo;

	public override void Open() {

		base.Open();

		Jobcentre jc = (Jobcentre)obj;
		UpdateOverviewPage();

		highPhy.isOn = jc.HireHighPhy;
		highInt.isOn = jc.HireHighInt;
		highEmo.isOn = jc.HireHighEmo;

	}

	public override void UpdateOverviewPage() {

		base.UpdateOverviewPage();

		Jobcentre jc = (Jobcentre)obj;

		//update prospect list ONLY if there's different # of prospects than before
		if (prospectGrid.childCount == jc.Prospects.Count)
			return;

		foreach (Transform child in prospectGrid)
			Destroy(child.gameObject);

		int even = 0;
		//instantiate worker list
		foreach (Prole p in jc.Prospects) {

			if (p == null)
				continue;

			GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ProspectInfo"));
			go.transform.SetParent(prospectGrid);

			ProspectInfo pi = go.GetComponent<ProspectInfo>();
			pi.prospect = p;
			pi.jobcentre = jc;

			go.GetComponent<Image>().enabled = even == 1;

			even = even == 0 ? 1 : 0;

		}

	}

	public void ToggleHighPhy(bool b) {

		Jobcentre jc = (Jobcentre)obj;
		jc.HireHighPhy = b;

	}

	public void ToggleHighInt(bool b) {

		Jobcentre jc = (Jobcentre)obj;
		jc.HireHighInt = b;

	}

	public void ToggleHighEmo(bool b) {

		Jobcentre jc = (Jobcentre)obj;
		jc.HireHighEmo = b;

	}

}
