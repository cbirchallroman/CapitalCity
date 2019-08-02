using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class CampaignSelectMenu : MonoBehaviour {


    public TextAsset database;
    public string levelFilesLocation;
    public List<CampaignUI> scenarioLists;

    // Use this for initialization
    void Start() {

        if (!CampaignManager.loadedCampaigns)
            LoadProgress();
        LoadCampaignUI();

    }

    void LoadProgress() {

        ProgressSave cs = SaveLoadProgress.GetGameData();
        if(cs != null)
            Difficulty.dLevel = cs.difficulty;
        Dictionary<string, Campaign> campaigns = FreshCampaigns();

        //load fresh campaigns if file doesn't exist
        if (cs != null) {

            Dictionary<string, int> d = cs.savedCampaigns.GetDictionary();

            //read each saved campaign progress
            foreach (string str in d.Keys) {

                //if we don't have this campaign anymore, keep going
                if (!campaigns.ContainsKey(str)) {

                    Debug.LogError("Campaign in file no longer exists!");
                    continue;

                }

                //set campaign progress to whatever's saved
                campaigns[str].spot = d[str];

            }

        }

        //load campaigns into manager
        CampaignManager.LoadCampaigns(campaigns);

        //save new campaign progress file
        if (cs == null)
            SaveLoadProgress.SaveCampaign();

    }

    Dictionary<string, Campaign> FreshCampaigns() {

        Dictionary<string, Campaign> campaigns = new Dictionary<string, Campaign>();

        XmlDocument doc = new XmlDocument();

        using (StringReader s = new StringReader(database.text)) {
            doc.Load(s);
        }

        XmlNodeList struList = doc.GetElementsByTagName("Campaign");

        foreach (XmlNode stru in struList) {

            //make new campaign and read children nodes
            Campaign c = new Campaign();
            XmlNodeList children = stru.ChildNodes;

            foreach (XmlNode thing in children) {

                //read name of campaign
                if (thing.Name == "Name")
                    c.name = thing.InnerText;

                //check if all levels are unlocked or not
                else if (thing.Name == "AllUnlocked")
                    c.allUnlocked = bool.Parse(thing.InnerText);

                //read scenarios
                else if (thing.Name == "Levels") {

                    XmlNodeList levels = thing.ChildNodes;
                    foreach (XmlNode scnr in levels) {

                        GameFile gf = new GameFile(Mode.Construct, levelFilesLocation, scnr.InnerText);
                        c.levels.Add(gf);

                    }


                }

            }

            c.StartCampaign();
            campaigns[c.name] = c;


        }

        return campaigns;

    }

    void LoadCampaignUI() {

        foreach(CampaignUI cc in scenarioLists) {

            string n = cc.campaignName;

            if (!CampaignManager.HasCampaign(n)) {

                Dictionary<string, Campaign> campaigns = FreshCampaigns();
                if (campaigns.ContainsKey(n))
                    CampaignManager.campaigns[n] = campaigns[n];
                else
                    return;

            }
            
            cc.LoadScenarioButtons();

        }

    }

}
