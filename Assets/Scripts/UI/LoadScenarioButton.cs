using UnityEngine;
using System.Collections;

public class LoadScenarioButton : LoadGameFileButton {

    public string CampaignName { get; set; }
    public int Level { get; set; }

    private void Update() {

        t.text = "Chapter " +  (Level + 1) + " - " + Datapath.filename;

    }

    public override void LoadGame() {

        CampaignManager.currentCampaign = CampaignName;
        CampaignManager.currentLevel = Level;

        SaveLoadMap.file = Datapath;
        SaveLoadMap.LoadLevel(Datapath.mode);

    }

}
