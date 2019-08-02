using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFreshMap : MonoBehaviour {

    public Mode mode;

    public void FreshCity() {

        CampaignManager.currentCampaign = null;
        CampaignManager.currentLevel = -1;

        SaveLoadMap.LoadLevel(mode);

    }

}
