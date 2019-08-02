using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveController : MonoBehaviour {

    public WorldController worldController;
    public GameObject overwriteMenu;
    public string directory;
    public string file { get; set; }
    public bool addedPrefix;

    private void Start() {

        if (!string.IsNullOrEmpty(CampaignManager.currentCampaign))
            directory = CampaignManager.currentCampaign;

    }

    public void ChangeFileName(string f) {

        file = f;
        addedPrefix = false;

    }

    public void AttemptSaveMap() {

        int lvl = CampaignManager.currentLevel;
        if (lvl != -1 && !addedPrefix) {
            file = "Chapter " + (lvl + 1) + " - " + file;
            addedPrefix = true;
        }
            

        if (string.IsNullOrEmpty(file)) {
            Debug.Log("name is empty");
            return;
        }
        else if (SaveLoadMap.FileExistsInFiles(directory, file)) {
            Debug.Log(file + " exists");
            overwriteMenu.SetActive(true);
            return;
        }
        else
            SaveMap();

    }

    public void SaveScenario() {

        SaveLoadMap.SaveScenario(worldController.gameObject, worldController.scenario.levelName);

    }

    public void SaveMap() {
        
        SaveLoadMap.SaveGame(worldController.gameObject, directory, file);

    }

}
