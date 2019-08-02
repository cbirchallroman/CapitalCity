using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameFile {

    public string filelocation;
    public string filename;
    public Mode mode { get; set; }
    public bool loadFromResources { get; set; }

    public GameFile(Mode m, string l, string n) {

        mode = m;
        filelocation = l;
        filename = n;

    }

    public GameFile(Mode m, string l, string n, bool r) {

        mode = m;
        filelocation = l;
        filename = n;
        loadFromResources = r;

    }

}
