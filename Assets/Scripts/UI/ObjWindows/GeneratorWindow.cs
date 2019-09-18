using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorWindow : WorkplaceWindow {

	[Header("Generator")]
	public Image clock;
    public Text prodTime;
    public Text productivity;
	public Text progress;
    public Text valueCreated;
    public Text surplusValue;
    public Text superProfits;

	public override void UpdateOverviewPage() {

		base.UpdateOverviewPage();

		Generator g = obj.GetComponent<Generator>();

		progress.text = (int)g.PercentDone + "% complete";
		clock.fillAmount = g.PercentDone / 100.0f;

	}

	public override void UpdateProductivityPage() {

		base.UpdateProductivityPage();

		Generator g = obj.GetComponent<Generator>();

		prodTime.text = "This " + g.DisplayName + " will produce " + g.stockpile + " " + g.product + " every " + (g.ActualProductionCycle > 0 ? g.ActualProductionCycle : g.BaseProductionCycle) + " days.";
		productivity.text = "It operates at " + g.RelativeProductivity + "% productivity compared to other buildings like it.";

	}

	public override void UpdateFinancialPage() {

		base.UpdateFinancialPage();

		Generator g = obj.GetComponent<Generator>();

		bool productive = g.Operational;

		valueCreated.gameObject.SetActive(productive);
		surplusValue.gameObject.SetActive(productive);
		superProfits.gameObject.SetActive(productive);

		valueCreated.text = "<b>Value:</b>\t" + MoneyController.symbol + g.ValueProduced.ToString("n2");
		surplusValue.text = "\t\t(" + MoneyController.symbol + g.SurplusValue.ToString("n2") + " surplus value)";

		superProfits.gameObject.SetActive(g.SuperProfits > 0);
		if (g.SuperProfits > 0)
			superProfits.text = "\t\t\t(" + MoneyController.symbol + g.SuperProfits.ToString("n2") + " super profits)";

	}

}
