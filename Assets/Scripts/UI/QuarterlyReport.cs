using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuarterlyReport : MonoBehaviour {

    public Quarter Q { get; set; }
    public Image background;
    public Text time;
    public Text balance;

	public Float3d winterColor = new Float3d(83, 202, 233);
	public Float3d springColor = new Float3d(0, 186, 0);
	public Float3d summerColor = new Float3d(200, 200, 0);
	public Float3d autumnColor = new Float3d(255, 165, 54);

	[Header("Credit")]
    public Text construction;
    public Text foodImports;
    public Text goodImports;
    public Text resourceImports;
    public Text wages;
    public Text credit;

    [Header("Debit")]
    public Text foodExports;
    public Text foodSales;
    public Text goodExports;
    public Text goodSales;
    public Text resourceExports;
    public Text debit;

    public void PrintReport(Quarter q) {

        Q = q;

        time.text = Q.ToString();

        UpdateReport();

        Season season = (Season)q.season;
        if (season == Season.Winter)
            background.color = new Color(winterColor.X / 255, winterColor.Y / 255, winterColor.Z / 255);
        else if (season == Season.Spring)
            background.color = new Color(springColor.X / 255, springColor.Y / 255, springColor.Z / 255);
		else if (season == Season.Summer)
            background.color = new Color(summerColor.X / 255, summerColor.Y / 255, summerColor.Z / 255);
		else if (season == Season.Autumn)
            background.color = new Color(autumnColor.X / 255, autumnColor.Y / 255, autumnColor.Z / 255);

	}

    public void UpdateReport() {

        bool pos_bal = Q.Balance >= 0;
        float bal = pos_bal ? Q.Balance : Q.Balance * -1;
        balance.text = pos_bal ? MoneyController.symbol + bal.ToString("n2") : "-" + MoneyController.symbol + bal.ToString("n2");
        balance.color = pos_bal ? Color.white : new Color(185f / 255f, 0, 0);

        construction.text = Q.construction > 0 ? "-" + MoneyController.symbol + Q.construction.ToString("n2") : null;
        foodImports.text = Q.foodImports > 0 ? "-" + MoneyController.symbol + Q.foodImports.ToString("n2") : null;
        goodImports.text = Q.goodImports > 0 ? "-" + MoneyController.symbol + Q.goodImports.ToString("n2") : null;
        resourceImports.text = Q.resourceImports > 0 ? "-" + MoneyController.symbol + Q.resourceImports.ToString("n2") : null;
        wages.text = Q.wages > 0 ? "-" + MoneyController.symbol + Q.wages.ToString("n2") : null;
        credit.text = "-" + MoneyController.symbol + Q.Credit.ToString("n2");

        foodExports.text = Q.foodExports > 0 ? MoneyController.symbol + Q.foodExports.ToString("n2") : null;
        foodSales.text = Q.foodSales > 0 ? MoneyController.symbol + Q.foodSales.ToString("n2") : null;
        goodExports.text = Q.goodExports > 0 ? MoneyController.symbol + Q.goodExports.ToString("n2") : null;
        goodSales.text = Q.goodSales > 0 ? MoneyController.symbol + Q.goodSales.ToString("n2") : null;
        resourceExports.text = Q.resourceExports > 0 ? MoneyController.symbol + Q.resourceExports.ToString("n2") : null;
        debit.text = MoneyController.symbol + Q.Debit.ToString("n2");

    }

}
