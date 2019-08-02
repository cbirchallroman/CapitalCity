using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ProgressSave {

    public DictContainer<string, int> savedCampaigns;
    public DifficultyLevel difficulty;

    public ProgressSave() {

        Dictionary<string, int> d = new Dictionary<string, int>();
        foreach (string str in CampaignManager.campaigns.Keys)
            d[str] = CampaignManager.campaigns[str].spot;
        savedCampaigns = new DictContainer<string, int>(d);
        difficulty = Difficulty.dLevel;

    }

}

public class CampaignManager {


    public static Dictionary<string, Campaign> campaigns;
    public static bool loadedCampaigns;
    public static string currentCampaign = null;
    public static int currentLevel = -1;

    public static void LoadCampaigns(Dictionary<string, Campaign> c) {

        campaigns = c;
        loadedCampaigns = true;

    }

    public static void UnlockNextLevel() {

        if (currentCampaign == null || currentLevel == -1)
            return;

        campaigns[currentCampaign].UnlockLevelAfter(currentLevel);
        SaveLoadProgress.SaveCampaign();

    }

    public static bool HasNextLevel() {

        if (currentCampaign == null || currentLevel == -1)
            return false;

        return campaigns[currentCampaign].IsLevelAfter(currentLevel);
        
    }

    public static bool HasCampaign(string s) {

        return campaigns.ContainsKey(s);

    }

}