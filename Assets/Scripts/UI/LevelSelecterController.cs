using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelecterController : MonoBehaviour {

    public void ChangeNewMapSize(float i) {
        i *= 25;
        SaveLoadMap.newWorldSize = new Node((int)i, (int)i);

    }

    public void NextScenario() {

        Campaign c = CampaignManager.campaigns[CampaignManager.currentCampaign];

        CampaignManager.currentCampaign = c.name;
        CampaignManager.currentLevel += 1;

        SaveLoadMap.file = c.levels[CampaignManager.currentLevel];
        SaveLoadMap.LoadLevel(Mode.Construct);

    }

    public void LoadTerrainEditor() {

        SaveLoadMap.LoadLevel(Mode.Edit);

    }

    public void LoadCity() {

        SaveLoadMap.LoadLevel(Mode.Construct);

    }

    public void MainMenu() {


        SceneManager.LoadSceneAsync("Scenes/MainMenu_Mobile");

    }

    public void QuitGame() {

        Application.Quit();

    }

}
