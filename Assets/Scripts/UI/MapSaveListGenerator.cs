using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapSaveListGenerator : MonoBehaviour {
    
    public string fileLocation;
    public Mode mode;
    public GameObject grid;
    public Scrollbar sb;
    public bool canDeleteFiles = true;
    public bool waitForDirectory = false;

    void Start () {

        if (!waitForDirectory)
            LoadSaves();

    }

    public void LoadSaves() {

        //generate buttons for individual saves

        string datapath = Application.persistentDataPath + "/";

        if (!Directory.Exists(datapath + fileLocation))
            Directory.CreateDirectory(datapath + fileLocation);

        foreach (string f in Directory.GetFiles(datapath + fileLocation)) {

            string fileName = Path.GetFileNameWithoutExtension(f);

            GameObject go = Instantiate(UIObjectDatabase.GetUIElement("SavedGame"));
            go.transform.parent = grid.transform;
            go.transform.localScale = new Vector3(1, 1, 1);
            go.name = "Button_" + f;

            LoadGameFileButton b = go.GetComponent<LoadGameFileButton>();
            b.Datapath = new GameFile(mode, fileLocation, fileName, false);
            b.Deletable = canDeleteFiles;

        }

        //reset scrollbar to top
        sb.value = 1;

    }
}
