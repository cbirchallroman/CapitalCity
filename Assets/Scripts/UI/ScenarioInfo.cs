using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioInfo : MonoBehaviour {
	
    public GameObject gridThing;
    public ScenarioController scenario;
    public Text scenarioName;
    public Text scenarioDescription;
    
    void Start () {
        
        scenarioName.text = scenario.levelName;
        scenarioDescription.text = scenario.levelDesc;

        CreateGoalList();

    }

    void CreateGoalList() {

        //add housing goal to level info
        if (scenario.HasHouseGoal) {

            GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ScenarioGoal"));

            ScenarioGoal sg = go.GetComponent<ScenarioGoal>();
            sg.goalDesc.text = scenario.HousingGoalToString();
            sg.toggle.isOn = scenario.HousingComplete;
            sg.transform.SetParent(gridThing.transform);
            sg.transform.localScale = new Vector3(1, 1, 1);

        }

        //add pop goal
        if (scenario.HasPopGoal) {

            GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ScenarioGoal"));

            ScenarioGoal sg = go.GetComponent<ScenarioGoal>();
            sg.goalDesc.text = scenario.PopulationToString();
            sg.toggle.isOn = scenario.PopulationComplete;
            sg.transform.SetParent(gridThing.transform);
            sg.transform.localScale = new Vector3(1, 1, 1);

        }

        //add prosperity goal
        if (scenario.HasProspGoal) {

            GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ScenarioGoal"));

            ScenarioGoal sg = go.GetComponent<ScenarioGoal>();
            sg.goalDesc.text = scenario.ProsperityToString();
            sg.toggle.isOn = scenario.ProsperityComplete;
            sg.transform.SetParent(gridThing.transform);
            sg.transform.localScale = new Vector3(1, 1, 1);

        }

        if (scenario.HasStorageGoals) {

            List<string> storageGoals = scenario.storageGoals;

            for(int i = 0; i < storageGoals.Count; i++) {

                ItemOrder io = new ItemOrder(storageGoals[i]);

                GameObject go = Instantiate(UIObjectDatabase.GetUIElement("ScenarioGoal"));

                ScenarioGoal sg = go.GetComponent<ScenarioGoal>();
                sg.goalDesc.text = scenario.StorageGoalToString(i);
                sg.toggle.isOn = scenario.worldController.HasGood(io);
                sg.transform.SetParent(gridThing.transform);
                sg.transform.localScale = new Vector3(1, 1, 1);

            }

        }

    }

    void DeleteGoalList() {

        Transform gridTransform = gridThing.transform;

        for(int i = 0; i < gridTransform.childCount; i++) {

            GameObject g = gridTransform.GetChild(i).gameObject;
            GameObject.Destroy(g);

        }

    }

    public void UpdateGoalList() {

        DeleteGoalList();
        CreateGoalList();

    }

}
