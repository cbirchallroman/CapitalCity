using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignUI : MonoBehaviour {
    
    public string campaignName;
    public GameObject grid;
    public Text label;
    public Scrollbar sb;
    public MapSaveListGenerator saveListGenerator;

    public void LoadScenarioButtons() {

        Campaign cmpgn = CampaignManager.campaigns[campaignName];

        //generate buttons for individual saves
        for (int x = 0; x < cmpgn.levels.Count; x++) {

            GameFile gf = cmpgn.levels[x];
            gf.loadFromResources = true;

            GameObject b = Instantiate(UIObjectDatabase.GetUIElement("Scenario"));
            b.transform.parent = grid.transform;
            b.transform.localScale = new Vector3(1, 1, 1);
            b.name = "Button_" + gf.filename;

            LoadScenarioButton sc = b.GetComponent<LoadScenarioButton>();
            sc.Datapath = gf;
            sc.CampaignName = campaignName;
            sc.Level = x;

            Button bttn = b.GetComponent<Button>();
            bttn.interactable = x <= cmpgn.spot;

        }

        label.text = campaignName;
        saveListGenerator.fileLocation = campaignName;
        saveListGenerator.LoadSaves();

        //reset scrollbar to top
        sb.value = 1;

    }

}
