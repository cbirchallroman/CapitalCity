using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour {

    public Action act;
    public ActionController actionController;
    public Image img;
    public Text desc;
	public Text priceLabel;
	public ObjWindow objWindow;

    public void Start() {

		if (StructureDatabase.HasData(act.What) && desc != null)
			desc.text = StructureDatabase.GetData(act.What).displayName;
		else if(desc != null)
			desc.text = act.What;

		if (StructureDatabase.HasData(act.What) && priceLabel != null) {
			int price = StructureDatabase.GetModifiedCost(act.What);
			priceLabel.text = MoneyController.symbol + "" + price;
		}
		else if(priceLabel != null)
			priceLabel.text = null;

		Sprite i = Resources.Load<Sprite>("ActionImgs/" + act.What);

		if (i != null && img != null)
			img.sprite = i;

	}

    public void SetAction() {
        
		if(act.Do == "open") {
			actionController.Do(act, -1, -1);
		}
		else {
			actionController.SetAction(act);
			actionController.constructionController.CloseMenu();
			actionController.editController.CloseMenu();
		}

		if (objWindow != null)
			objWindow.Close();
		
    }

}
