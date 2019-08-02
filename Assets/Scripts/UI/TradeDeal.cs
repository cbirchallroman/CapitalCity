using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeDeal : MonoBehaviour {

    public ItemOrder order;
    public TradeController trade;
    public DiplomacyMenu diplo;
    public GameObject makeDeal;
    public GameObject endDeal;
	public Image image;
    public Text desc;

    private void Start() {

		string itemName = order.GetItemName();
        desc.text = order.amount + " " + itemName + " for " + MoneyController.symbol + order.ExchangeValue().ToString("n2");
		Sprite spr = ResourcesDatabase.GetSprite(itemName);
		if (spr != null)
			image.sprite = spr;

		UpdateButton(trade.ContainsDeal(order));

    }

    void UpdateButton(bool open) {

        makeDeal.SetActive(!open);
		endDeal.SetActive(open);

	}

    public void MakeDeal() {

        diplo.OpenTrade(order);
        UpdateButton(true);

    }

    public void EndDeal() {

        diplo.CloseTrade(order);
        UpdateButton(false);

    }

}
