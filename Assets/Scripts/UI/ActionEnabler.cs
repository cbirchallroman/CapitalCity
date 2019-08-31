using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionEnabler : MonoBehaviour {

    public int ListIndex { get; set; }
    public int Category { get; set; }
    public Action Action { get; set; }
    public ActionSelecterController ActionSelecter { get; set; }

    public Text label;
    public Text categoryLabel;
    public Toggle toggle;

    private void Start() {

		if (Action.Do.Contains("place"))
			label.text = StructureDatabase.GetData(Action.What).displayName;
		else if (Action.Do.Contains("open"))
			label.text = Action.What + " Category";
		else
			label.text = Action.What;
        categoryLabel.text = (BuildingType)Category + "";
        toggle.isOn = ActionSelecter.actionList[ListIndex][Action];

    }

    public void ToggleAction(bool b) {

        ActionSelecter.actionList[ListIndex][Action] = b;

    }

}
