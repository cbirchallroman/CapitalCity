using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadMap {

    public static string scenariosDirectory = "Assets/Resources/Scenarios";
    public static string fileType = ".alu";

    public static Mode loadWorldMode = Mode.END;
    public static GameFile file;

    //stuff for creating a new game
    public static Node newWorldSize = new Node(24, 24);

    public static void SaveGame(GameObject go, string dir, string filename) {

        string datapath = Application.persistentDataPath + "/";

        if (!Directory.Exists(datapath + dir))
            Directory.CreateDirectory(datapath + dir);

        WorldProgressSave savedGame = new WorldProgressSave(go);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream strm = File.Create(Path.Combine(datapath + dir, filename + fileType));
        bf.Serialize(strm, savedGame);
        strm.Close();
        Debug.Log(filename + " saved!");

    }

    public static void SaveScenario(GameObject go, string filename) {

        if (!Directory.Exists(scenariosDirectory))
            Directory.CreateDirectory(scenariosDirectory);

        //BasicWorldSave savedGame = new BasicWorldSave(go);
        WorldProgressSave savedGame = new WorldProgressSave(go);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream strm = File.Create(Path.Combine(scenariosDirectory, filename + ".bytes"));
        bf.Serialize(strm, savedGame);
        strm.Close();
        Debug.Log(filename + " saved!");

    }

    public static void LoadLevel(Mode m) {

        loadWorldMode = m;
        SceneManager.LoadSceneAsync("Scenes/City");

    }

    public static bool FileExistsInFiles(string d, string f) {

        string datapath = Application.persistentDataPath + "/";
        return File.Exists(Path.Combine(datapath + d, f + fileType));

    }

    public static BasicWorldSave GetGameData() {

        if (file == null)
            return null;

        //keep current selected world
        string fl = file.filelocation;
        string fn = file.filename;
        bool res = file.loadFromResources;

        //reset current selected world
        file = null;

        if (res)
            return GetScenarioFromResources(fl, fn);
            //return GetSavedGameFromResources(fl, fn);

        return GetSavedFromFiles(Application.persistentDataPath + "/" + fl, fn);

    }

    public static WorldProgressSave GetSavedFromFiles(string dir, string name) {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream strm = File.Open(Path.Combine(dir, name + fileType), FileMode.Open);
        WorldProgressSave savedGame = (WorldProgressSave)bf.Deserialize(strm);
        strm.Close();
        return savedGame;

    }

    public static WorldProgressSave GetSavedGameFromResources(string dir, string name) {
        
        TextAsset file = Resources.Load<TextAsset>(dir + "/" + name);
        MemoryStream str = new MemoryStream(file.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        WorldProgressSave savedGame = (WorldProgressSave)bf.Deserialize(str);
        str.Close();
        return savedGame;

    }

    public static BasicWorldSave GetScenarioFromResources(string dir, string name) {

        Debug.Log("this");

        TextAsset file = Resources.Load<TextAsset>(dir + "/" + name);
        MemoryStream str = new MemoryStream(file.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        BasicWorldSave savedGame = (BasicWorldSave)bf.Deserialize(str);
        str.Close();
        return savedGame;

    }

    public static void DeleteFile(string dir, string name) {
        
        File.Delete(Application.persistentDataPath + "/" + dir + "/" + name + fileType);

    }

}

public class SaveLoadProgress {

    //stuff for loading/saving
    public static string saveFile = "player.prg";

    //stuff for creating a new game
    public static Node newWorldSize = new Node(25, 25);

    public static void SaveCampaign() {

        string datapath = Application.persistentDataPath;

        ProgressSave savedGame = new ProgressSave();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Path.Combine(datapath, saveFile));
        bf.Serialize(file, savedGame);
        file.Close();
        Debug.Log("Player progress saved!");
        
    }

    public static bool FileExistsInFiles() {

        string datapath = Application.persistentDataPath;
        return File.Exists(Path.Combine(datapath, saveFile));

    }

    public static ProgressSave GetGameData() {

        string datapath = Application.persistentDataPath;

        if (!FileExistsInFiles())
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream str = File.Open(Path.Combine(datapath, saveFile), FileMode.Open);
        ProgressSave savedGame = (ProgressSave)bf.Deserialize(str);
        str.Close();

        Debug.Log("Player progress loaded!");

        return savedGame;

    }

}