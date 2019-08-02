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
        currentQuarter = mc.currentQuarter;
        pastQuarters = mc.pastQuarters;

    }

}

public class MoneyController : MonoBehaviour {

    public Text moneyLabel;
    public Quarter currentQuarter;
    public QuarterList pastQuarters;
	public static char symbol = '₡';

	public float Money { get; set; }

    public void Load(MoneySave mc) {

        Money = mc.money;
        currentQuarter = mc.currentQuarter;
        pastQuarters = mc.pastQuarters;

    }

    public void Update() {

        if (moneyLabel == null)
            return;

        moneyLabel.text = symbol + Money.ToString("n2");
        moneyLabel.color = Money >= 0 ? Color.white : Color.red;

    }

    public void BeginNewQuarter(int s, int y) {
        
        pastQuarters.PushQuarter(currentQuarter);
        currentQuarter = new Quarter(s, y);

    }

    public void SpendOnWages(float m) {

        Money -= m;
        currentQuarter.wages += m;

    }

    public void SpendOnConstruction(float m) {

        Money -= m;
        currentQuarter.construction += m;

    }

    public void SpendOnMaintenance(float m) {

        Money -= m;
        currentQuarter.maintenance += m;

    }

    public void SpendOnImports(float m, ItemType type) {

        Money -= m;
        if (type == ItemType.Food)
            currentQuarter.foodImports += m;
        else if (type == ItemType.Good)
            currentQuarter.goodImports += m;
        else if (type == ItemType.Resource)
            currentQuarter.resourceImports += m;

    }

    public void GainFromLocalSales(float m, ItemType type) {

        Money += m;
        if (type == ItemType.Food)
            currentQuarter.foodSales += m;
        else if (type == ItemType.Good)
            currentQuarter.goodSales += m;

    }

    public void GainFromExports(float m, ItemType type) {

        Money += m;
        if (type == ItemType.Food)
            currentQuarter.foodExports += m;
        else if (type == ItemType.Good)
            currentQuarter.goodExports += m;
        else if (type == ItemType.Resource)
            currentQuarter.resourceExports += m;

    }

}
