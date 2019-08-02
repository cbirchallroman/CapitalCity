using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGameFileButton : MonoBehaviour {
    
    public GameFile Datapath { get; set; }
    public bool Deletable { get; set; }
    public Button deleteFile;
    public Text t;

    private void Update() {
        
        t.text = Datapath.filename;
        deleteFile.interactable = Deletable;
        if (!SaveLoadMap.FileExistsInFiles(Datapath.filelocation, Datapath.filename))
            Destroy(gameObject);

    }

    public virtual void LoadGame() {

        CampaignManager.currentCampaign = null;
        CampaignManager.currentLevel = -1;

        Debug.Log("Current campaign: " + CampaignManager.currentCampaign);

        SaveLoadMap.file = Datapath;
        SaveLoadMap.LoadLevel(Datapath.mode);

    }

    public void DeleteFile() {

        SaveLoadMap.DeleteFile(Datapath.filelocation, Datapath.filename);
        Destroy(gameObject);

    }

}
