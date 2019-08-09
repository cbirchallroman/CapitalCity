using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ScenarioSave {

    public int Prosperity, level;
    public int[] HousingLevels;
    public bool openedVictory;
    public float timeDelta;
    public string campaign;

	public ScenarioGoals goals;

    public ScenarioSave(ScenarioController s) {
        
        openedVictory = s.openedVictory;
        timeDelta = s.timeDelta;

        HousingLevels = s.HousingLevels;

        Prosperity = s.Prosperity;

        level = CampaignManager.currentLevel;
        campaign = CampaignManager.currentCampaign;

		goals = s.goals;

    }

}

[System.Serializable]
public class ScenarioGoals {

	public string levelName;

	[TextArea]
	public string levelDesc;
	public int housingAmount;
	public int housingLevel = 1;
	public int population;
	public int prosperity;
	public List<string> storageGoals;

}

public class ScenarioController : MonoBehaviour {

    public Button nextLvlButton;
	
    public WorldController worldController;

    public MenuController scenarioMenu;
    public GameObject infoPage;
    public GameObject victoryPage;
    public GameObject bankruptPage;
    public bool openedVictory { get; set; }

	[Header("Scenario Goals")]
	public ScenarioGoals goals;



	public void changeName(string s) { goals.levelName = s; }
	public void changeDesc(string s) { goals.levelDesc = s; }

    public void changeHousingGoalAmount(string i) { goals.housingAmount = int.Parse(i); }
    public void changeHousingGoalLevel(string i) { goals.housingLevel = int.Parse(i); }
    public int[] HousingLevels { get; set; }
    public bool HasHouseGoal { get { return goals.housingAmount > 0 && goals.housingLevel > 0; } }
    public float HousingProgress { get { //return Mathf.Clamp((float)HousingLevels[housingGoalLevel - 1] / housingGoalAmount, 0 , 1);
            return 0;
        } }
    public string HousingGoalToString() {

        if (!HasHouseGoal)
            return null;
        return goals.housingAmount + " houses reach Level " + goals.housingLevel;

    }

    public void changePopulationGoal(string i) { goals.population = int.Parse(i); }
    public bool HasPopGoal { get { return goals.population != 0; } }
    public float PopulationProgress { get { return Mathf.Clamp((float)worldController.population.Population / goals.population, 0, 1); } }
    public string PopulationToString() {

        if (!HasPopGoal)
            return null;
        return "Population of " + goals.population;

    }

    public void changeProsperityGoal(string i) { goals.population = int.Parse(i); }
    public int ProsperityIncreaseRate { get { return 2; } }
    public int Prosperity { get; set; }
    public bool HasProspGoal { get { return goals.prosperity != 0; } }
    public float ProsperityProgress { get { return Mathf.Clamp((float)Prosperity / goals.prosperity, 0, 1); } }
    public string ProsperityToString() {

        if (!HasProspGoal)
            return null;
        return "Prosperity rating of " + goals.prosperity;

    }

    public bool HasStorageGoals { get { return goals.storageGoals.Count != 0; } }
    public string StorageGoalToString(int i) {

        if (i >= goals.storageGoals.Count)
            Debug.LogError("Index " + i + " in storage goals list out of bounds");

        ItemOrder io = new ItemOrder(goals.storageGoals[i]);
        return "Have " + io.amount + " " + io.name + " in storage";

    }

    public void Load(ScenarioSave s) {
		
        openedVictory = s.openedVictory;
        timeDelta = s.timeDelta;

        HousingLevels = s.HousingLevels;

        Prosperity = s.Prosperity;

		goals = s.goals;

        if (CampaignManager.currentLevel == -1) {

            CampaignManager.currentLevel = s.level;
            CampaignManager.currentCampaign = s.campaign;

        }


    }

    private void Start() {

        HousingLevels = new int[20];

        if (CampaignManager.currentLevel != -1)
            scenarioMenu.OpenMenu(infoPage);

    }

    public float timeDelta { get; set; }

    private void Update() {


        timeDelta += Time.deltaTime;

        if (timeDelta > TimeController.DaysInAYear) {

            timeDelta = 0;
            UpdateProsperity();

        }

        CheckVictory();

        nextLvlButton.interactable = CampaignManager.HasNextLevel();

    }

    public void CheckVictory() {

        if (GoalsComplete() && !openedVictory) {

            Victory();

        }

    }

    public bool PopulationComplete { get { return worldController.population.Population >= goals.population; } }
    public bool ProsperityComplete { get { return Prosperity >= goals.prosperity; } }
    public bool HousingComplete { get { if (goals.housingLevel == 0) return false; return HousingLevels[goals.housingLevel - 1] >= goals.housingAmount; } }
    public bool StorageGoalsComplete{ get {

            foreach (string s in goals.storageGoals) {

                ItemOrder io = new ItemOrder(s);

                if (!worldController.HasGood(io))
                    return false;

            }
                
            return true;

        } }

    public bool GoalsComplete() {

        if (CampaignManager.currentLevel == -1)
            return false;

        if (!PopulationComplete)
            return false;

        if (!ProsperityComplete)
            return false;

        if (!HousingComplete)
            return false;

        if (!StorageGoalsComplete)
            return false;

        return true;

    }
    
    void UpdateProsperity() {


        GameObject[] objs = GameObject.FindGameObjectsWithTag("House");
        int prosperityCap = 0;
        int numOfHouses = 0;

        foreach (GameObject go in objs) {

            House h = go.GetComponent<House>();
            if (h == null)
                Debug.LogError(go.name + " is not a house");
            prosperityCap += h.prosperityRating;
            numOfHouses++;

        }

        if (numOfHouses == 0) {

            Prosperity = 0;
            return;

        }
            

        prosperityCap /= numOfHouses;

        //if cap is greater than current, add 2
        if (prosperityCap > Prosperity)
            Prosperity += ProsperityIncreaseRate;

        //if unemployment is less than 5%, add 1
        if (worldController.population.UnemployedPercent < 5)
            Prosperity++;

        //else if it's more than 15%, subtract 1
        else if (worldController.population.UnemployedPercent > 5)
            Prosperity--;

    }

    public void Victory() {

        Debug.Log("Beat " + CampaignManager.currentCampaign + " - Chapter " + CampaignManager.currentLevel);

        openedVictory = true;
        CampaignManager.UnlockNextLevel();
        scenarioMenu.OpenMenu(victoryPage);

    }

    public void AddHouseLevel(int lvl) {

        if (HousingLevels.Length <= lvl)
            return;
        HousingLevels[lvl]++;

    }

    public void RemoveHouseLevel(int lvl) {

        if (HousingLevels.Length <= lvl)
            return;
        HousingLevels[lvl]--;

    }

}
