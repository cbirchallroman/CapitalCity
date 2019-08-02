using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Campaign {

    public string name;
    public bool allUnlocked;
    public List<GameFile> levels = new List<GameFile>();

    public int spot;

    public void StartCampaign() {

        spot = 0;

        //unlock all levels if enabled
        if (allUnlocked) {

            spot = levels.Count;

        }

    }

    public void UnlockLevelAfter(int lvl) {

        if (lvl == spot && lvl < levels.Count)
            spot++;

        Debug.Log(Completed);

    }

    public override string ToString() {

        return name;

    }

    public bool Completed { get { return spot == levels.Count && !allUnlocked; } }

    public bool IsLevelAfter(int lvl) {

        return lvl < levels.Count - 1;

    }

}