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

        label.text = StructureDatabase.GetData(Action.What).displayName;
        categoryLabel.text = (BuildingType)Category + "";
        toggle.isOn = ActionSelecter.actionList[ListIndex][Action];

    }

    public void ToggleAction(bool b) {

        ActionSelecter.actionList[ListIndex][Action] = b;

    }

}
