using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ScenarioGoalsSave {

    public int Prosperity, level, populationGoal, prosperityGoal, housingGoalAmount, housingGoalLevel;
    public int[] HousingLevels;
    public bool openedVictory;
    public float timeDelta;
    public string campaign, levelName, levelDesc;
    public List<string> storageGoals;

    public ScenarioGoalsSave(ScenarioController s) {
        
        populationGoal = s.populationGoal;
        openedVictory = s.openedVictory;
        timeDelta = s.timeDelta;

        housingGoalAmount = s.housingGoalAmount;
        housingGoalLevel = s.housingGoalLevel;

        HousingLevels = s.HousingLevels;

        Prosperity = s.Prosperity;
        prosperityGoal = s.prosperityGoal;

        levelName = s.levelName;
        levelDesc = s.levelDesc;

        storageGoals = s.storageGoals;

        level = CampaignManager.currentLevel;
        campaign = CampaignManager.currentCampaign;

    }

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

    [TextArea]
    public string levelName;
    public void changeName(string s) { levelName = s; }
    public string levelDesc;
    public void changeDesc(string s) { levelDesc = s; }

    public int housingGoalAmount;
    public void changeHousingGoalAmount(string i) { housingGoalAmount = int.Parse(i); }
    public int housingGoalLevel = 1;
    public void changeHousingGoalLevel(string i) { housingGoalLevel = int.Parse(i); }
    public int[] HousingLevels { get; set; }
    public bool HasHouseGoal { get { return housingGoalAmount > 0 && housingGoalLevel > 0; } }
    public float HousingProgress { get { //return Mathf.Clamp((float)HousingLevels[housingGoalLevel - 1] / housingGoalAmount, 0 , 1);
            return 0;
        } }
    public string HousingGoalToString() {

        if (!HasHouseGoal)
            return null;
        return housingGoalAmount + " houses reach Level " + housingGoalLevel;

    }

    public int populationGoal;
    public void changePopulationGoal(string i) { populationGoal = int.Parse(i); }
    public bool HasPopGoal { get { return populationGoal != 0; } }
    public float PopulationProgress { get { return Mathf.Clamp((float)worldController.population.Population / populationGoal, 0, 1); } }
    public string PopulationToString() {

        if (!HasPopGoal)
            return null;
        return "Population of " + populationGoal;

    }

    public int prosperityGoal;
    public void changeProsperityGoal(string i) { populationGoal = int.Parse(i); }
    public int ProsperityIncreaseRate { get { return 2; } }
    public int Prosperity { get; set; }
    public bool HasProspGoal { get { return prosperityGoal != 0; } }
    public float ProsperityProgress { get { return Mathf.Clamp((float)Prosperity / prosperityGoal, 0, 1); } }
    public string ProsperityToString() {

        if (!HasProspGoal)
            return null;
        return "Prosperity rating of " + prosperityGoal;

    }

    public List<string> storageGoals;
    public bool HasStorageGoals { get { return storageGoals.Count != 0; } }
    public string StorageGoalToString(int i) {

        if (i >= storageGoals.Count)
            Debug.LogError("Index " + i + " in storage goals list out of bounds");

        ItemOrder io = new ItemOrder(storageGoals[i]);
        return "Have " + io.amount + " " + io.name + " in storage";

    }

    public void Load(ScenarioGoalsSave s) {

        populationGoal = s.populationGoal;
        openedVictory = s.openedVictory;
        timeDelta = s.timeDelta;

        housingGoalAmount = s.housingGoalAmount;
        housingGoalLevel = s.housingGoalLevel;

        HousingLevels = s.HousingLevels;

        Prosperity = s.Prosperity;
        prosperityGoal = s.prosperityGoal;

        storageGoals = s.storageGoals;

        levelName = s.levelName;
        levelDesc = s.levelDesc;

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

    public bool PopulationComplete { get { return worldController.population.Population >= populationGoal; } }
    public bool ProsperityComplete { get { return Prosperity >= prosperityGoal; } }
    public bool HousingComplete { get { if (housingGoalLevel == 0) return false; return HousingLevels[housingGoalLevel - 1] >= housingGoalAmount; } }
    public bool StorageGoalsComplete{ get {

            foreach (string s in storageGoals) {

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
