using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MoneySave {

    public float money;
    public Quarter currentQuarter;
    public QuarterList pastQuarters;

    public MoneySave(MoneyController mc) {

        money = mc.Money;
        currentQuarter = mc.CurrentQuarter;
        pastQuarters = mc.pastQuarters;

    }

}

public class MoneyController : MonoBehaviour {

    public Text moneyLabel;
    public Quarter CurrentQuarter { get; set; }
    public QuarterList pastQuarters;
	public static char symbol = '₡';

	public float Money { get; set; }

    public void Load(MoneySave mc) {

        Money = mc.money;
        CurrentQuarter = mc.currentQuarter;
        pastQuarters = mc.pastQuarters;

    }

    public void Update() {

        if (moneyLabel == null)
            return;

        moneyLabel.text = symbol + Money.ToString("n2");
        moneyLabel.color = Money >= 0 ? Color.white : Color.red;

    }

	public void FreshStartingQuarter(int s, int y) {

		CurrentQuarter = new Quarter(s, y);

	}

    public void BeginNewQuarter(int s, int y) {
        
        pastQuarters.PushQuarter(CurrentQuarter);
        CurrentQuarter = new Quarter(s, y);

    }

    public void SpendOnWages(float m) {

        Money -= m;
        CurrentQuarter.wages += m;

    }

    public void SpendOnConstruction(float m) {

        Money -= m;
        CurrentQuarter.construction += m;

    }

    public void SpendOnMaintenance(float m) {

        Money -= m;
        CurrentQuarter.maintenance += m;

    }

    public void SpendOnImports(float m, ItemType type) {

        Money -= m;
        if (type == ItemType.Food)
            CurrentQuarter.foodImports += m;
        else if (type == ItemType.Good)
            CurrentQuarter.goodImports += m;
        else if (type == ItemType.Resource)
            CurrentQuarter.resourceImports += m;

    }

    public void GainFromLocalSales(float m, ItemType type) {

        Money += m;
        if (type == ItemType.Meal)
            CurrentQuarter.foodSales += m;
        else if (type == ItemType.Good)
            CurrentQuarter.goodSales += m;

    }

    public void GainFromExports(float m, ItemType type) {

        Money += m;
        if (type == ItemType.Food)
            CurrentQuarter.foodExports += m;
        else if (type == ItemType.Good)
            CurrentQuarter.goodExports += m;
        else if (type == ItemType.Resource)
            CurrentQuarter.resourceExports += m;

    }

}
