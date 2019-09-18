using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinanceMenu : MonoBehaviour {

    public GameObject grid;
    public QuarterlyReport CurrentQuarter { get; set; }
    public MoneyController moneyController;

    public void LoadFinancialReports() {

        DeleteFinancialReports();

		GameObject g = Instantiate(UIObjectDatabase.GetUIElement("QuarterlyReport"));
		QuarterlyReport currentReport = g.GetComponent<QuarterlyReport>();
		currentReport.PrintReport(moneyController.CurrentQuarter);
		g.transform.SetParent(grid.transform);
		CurrentQuarter = currentReport;

		Quarter[] prev = moneyController.pastQuarters.quarters;
        for (int i = moneyController.pastQuarters.currentIndex - 1; i > -1; i--) {

            Quarter q = prev[i];
            GameObject go = Instantiate(UIObjectDatabase.GetUIElement("QuarterlyReport"));
            QuarterlyReport qr = go.GetComponent<QuarterlyReport>();
            qr.PrintReport(q);

            go.transform.SetParent(grid.transform);

        }

    }

    private void Update() {

        if(CurrentQuarter != null)
            CurrentQuarter.UpdateReport();

    }

    public void DeleteFinancialReports() {

        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

    }

}
