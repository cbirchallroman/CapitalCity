using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketValueUI : MonoBehaviour {
	
	public GameObject grid;
	public ProductivityController productivity;
	public TooltipController tooltip;

	// Use this for initialization
	void Start () {
		
		List<string> items = Enums.GetAllItems();

		int even = 0;
		foreach (string item in items) {

			GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ItemValueUI"));
			go.transform.SetParent(grid.transform);

			ItemValueUI iv = go.GetComponent<ItemValueUI>();
			iv.ItemName = item;
			iv.Tooltip = tooltip;

			go.GetComponent<Image>().enabled = even == 1;
			even = even == 0 ? 1 : 0;

		}

	}
	
}
