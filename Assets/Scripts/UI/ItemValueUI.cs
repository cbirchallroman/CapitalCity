using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemValueUI : MonoBehaviour {

	public Image itemSprite;
	public Text itemLabel;
	public Text daysWorldWide;
	public Text daysLocal;
	public Text valueLabel;
	public GameObject ingredientGrid;
	public GameObject ingredient;
	public string ItemName { get; set; }
	public TooltipController Tooltip { get; set; }

	private void Start() {

		itemLabel.text = ItemName;

		Sprite spr = ResourcesDatabase.GetSprite(ItemName);
		if (spr != null)
			itemSprite.sprite = spr;

		foreach (string s in ResourcesDatabase.GetIngredients(ItemName)) {

			GameObject go = Instantiate(ingredient);
			go.transform.SetParent(ingredientGrid.transform);

			string[] d = s.Split(' ');

			IngredientInfo ing = go.GetComponent<IngredientInfo>();
			ing.Info = s;
			ing.LoadSprite(d[1]);

			//set sprite of go's image to whatever the ingredient is

		}

	}

	private void Update() {

        float baseDays = ResourcesDatabase.GetBaseDays(ItemName);
        float localProductivity = ProductivityController.GetAverageProductivityHere(ItemName);
        float globalProductivity = ProductivityController.GetAverageProductivityEverywhere(ItemName);
        
		daysWorldWide.text = Mathf.RoundToInt(baseDays / globalProductivity) + " days";
		daysLocal.text = localProductivity > 0 ? "(" + Mathf.RoundToInt(baseDays / localProductivity) + " days here)" : "(??? days here)";

		ItemOrder io = new ItemOrder(100, ItemName);
		
		valueLabel.text = MoneyController.symbol + io.ExchangeValue().ToString("n2");

	}

}
