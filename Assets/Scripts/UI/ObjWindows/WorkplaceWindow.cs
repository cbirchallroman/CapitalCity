using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkplaceWindow : ObjWindow {

	[Header("Workplace")]
	public Transform scheduleGrid;
	public Transform proleGrid;
	public Toggle toggle;
    public Slider hourSlider;
    public Text workers;
    public Text hours;
    public Text wagesOverall;
    public Text wagesPer;
	public Text noFinancial;
	public GameObject scheduleTogglePrefab;

    public override void Open() {

        base.Open();

        Workplace wp = (Workplace)obj;
        toggle.isOn = wp.ActiveBuilding;
		toggle.enabled = !(!wp.ActiveBuilding && !wp.ClosedByPlayer);
        hourSlider.value = wp.WorkingDay;
		LoadSchedule();

		UpdateProductivityPage();
		UpdateFinancialPage();
		CreateLaborPage();

		//set action to here
		wp.ProleEmploymentAction += CreateWorkerListElement;

	}

    public override void DoDuringUpdate() {

		base.DoDuringUpdate();
		UpdateProductivityPage();
		UpdateFinancialPage();

    }

	public override void UpdateOverviewPage() {

		base.UpdateOverviewPage();

		Workplace wp = (Workplace)obj;
		if (wp.ActiveBuilding) {
			workers.text = wp.WorkersCount + "/" + wp.workersMax + " proles employed";
			title.color = Color.white;
		}
		else {
			workers.text = "<color=yellow>Closed</color>";
			title.color = Color.yellow;
		}

	}

	public virtual void UpdateProductivityPage() {

		Workplace wp = (Workplace)obj;
		hours.text = "Working for " + wp.WorkingDay + " hours/day";

	}

	public virtual void UpdateFinancialPage() {

		Workplace wp = (Workplace)obj;

		bool productive = wp.Operational;

		wagesOverall.gameObject.SetActive(productive);
		wagesPer.gameObject.SetActive(productive);
		noFinancial.gameObject.SetActive(!productive);

		wagesOverall.text = "<b>Labor:</b> -" + MoneyController.symbol + wp.WagesOverall.ToString("n2") + "/month";
		wagesPer.text = "\t\t\t(-" + MoneyController.symbol + wp.WagesPerWorker.ToString("n2") + "/worker)";

	}

	public void CreateLaborPage() {

		Workplace wp = (Workplace)obj;
		
		//instantiate worker list
		foreach (Prole p in wp.WorkerList) {

			CreateWorkerListElement(p, wp);

		}

	}

	public void CreateWorkerListElement(Prole p, Workplace wp) {

		if (p == null)
			return;

		GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ProleInfo"));
		go.transform.SetParent(proleGrid);

		ProleInfo pi = go.GetComponent<ProleInfo>();
		pi.Employee = p;
		pi.WP = wp;

	}

	public void LoadSchedule() {

		Workplace wp = (Workplace)obj;

		for (int i = 0; i < 4; i++) {

			GameObject go = Instantiate(scheduleTogglePrefab);
			go.transform.SetParent(scheduleGrid);

			WorkplaceMonthScheduler wms = go.GetComponent<WorkplaceMonthScheduler>();
			wms.building = wp;
			wms.seasonIndex = i;

		}

	}

    public void PlayerToggleLabor(bool b) {
        
        Workplace wp = (Workplace)obj;
        wp.ClosedByPlayer = !b;
        wp.ToggleLabor(b);

    }

    public void ChangeWorkingDay(float i) {

        Workplace wp = (Workplace)obj;
        wp.WorkingDay = (int)i;

    }

}
