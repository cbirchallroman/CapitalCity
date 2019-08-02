using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ActionSelecterControllerSave {

    public List<Dictionary<Action, bool>> actionList { get; set; }

    public ActionSelecterControllerSave(ActionSelecterController a) {

        actionList = a.actionList;

    }

}

public class ActionSelecterController : MonoBehaviour {

    public TextAsset actionsXML;
    public List<Dictionary<Action, bool>> actionList { get; set; }
    public ActionController actionController;
    public List<GameObject> categoryMenus;
    public GameObject enableMenuGrid;
	public GameObject actionsGrid;
	public MenuController actionsMenu;

    public void Load(ActionSelecterControllerSave a) {

        actionList = a.actionList;

    }

	public void FreshActions() {

        //create list of actiondicts and make an index for current position
        actionList = new List<Dictionary<Action, bool>>();
        int categoryIndex = 0;

        //load document and list of category nodes
        XmlDocument doc = new XmlDocument();
        using (StringReader s = new StringReader(actionsXML.text))
            doc.Load(s);
        XmlNodeList categoryList = doc.GetElementsByTagName("Category");

        //load each category
        foreach (XmlNode cat in categoryList) {

            //load children and generate new dictionary at index
            XmlNodeList children = cat.ChildNodes;
            actionList.Add(new Dictionary<Action, bool>());

            //for each child node, generate an action and add it to the dictionary as true
            foreach (XmlNode thing in children) {
				
				Action act = new Action(thing.InnerText);
				actionList[categoryIndex][act] = true;

            }

            //next index
            categoryIndex++;
            

        }

    }

    public void LoadActionEnablers() {

        for (int x = 0; x < actionList.Count; x++) {

            Dictionary<Action, bool> dict = actionList[x];

            //for each action in dictionary
            foreach (Action a in dict.Keys) {

                GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ActionEnabler"));
                go.transform.SetParent(enableMenuGrid.transform);
                go.transform.localScale = new Vector3(1, 1, 1);

                ActionEnabler ae = go.GetComponent<ActionEnabler>();
                ae.Action = a;
                ae.Category = x;
                ae.ActionSelecter = this;
                ae.ListIndex = x;

            }

        }

    }

	public void ShowActions(string bt) {

		foreach (Transform child in actionsGrid.transform)
			Destroy(child.gameObject);

		int index = (int)Enums.categoryDict[bt];
		Dictionary<Action, bool> dict = actionList[index];

		//for each action in dictionary
		foreach (Action a in dict.Keys) {

			//if action is enabled, create the button
			if (!dict[a])
				continue;

			//instantiate object and set parent to category menu
			GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ActionButton"));
			go.transform.SetParent(actionsGrid.transform);
			go.transform.localScale = new Vector3(1, 1, 1);

			//set action and actioncontroller for button
			ActionButton ab = go.GetComponent<ActionButton>();
			ab.act = a;
			ab.actionController = actionController;
		}
		
		actionsMenu.OpenMenu();
		//((RectTransform)(actionsGrid.transform)).offsetMin = new Vector2(0, 0);

	}
	
}
