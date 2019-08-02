using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientInfo : MonoBehaviour {
	
	public string Info { get; set; }

	public void LoadSprite(string s) {

		Sprite spr = ResourcesDatabase.GetSprite(s);
		if (spr != null)
			GetComponent<Image>().sprite = spr;

	}

	public void ShowTooltip() {

		TooltipController.SetText(Info);

	}

	public void HideTooltip() {

		TooltipController.SetText(null);

	}

}
