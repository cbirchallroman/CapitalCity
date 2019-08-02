using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : MonoBehaviour {

    public Mode currentMode;
    public Mode modeOpen { get; set; }
    public TimeController timeController;

    public List<GameObject> constructionModeUI;
    public List<GameObject> editModeUI;
    public List<GameObject> combatModeUI;

    public void Start() {

        Mode m = SaveLoadMap.loadWorldMode;
        if (m != Mode.END)
            currentMode = m;
        modeOpen = Mode.END;
        UpdateMode();

    }

    private void Update() {

        UpdateMode();

        //pause if in edit mode
        if (currentMode == Mode.Edit)
            timeController.StopTime();

    }

    List<GameObject> justOpened;

    public void UpdateMode() {

        if (modeOpen != currentMode) {

            modeOpen = currentMode;

            justOpened = new List<GameObject>();

            //open construction UI
            OpenOrCloseModeUI(constructionModeUI, Mode.Construct);
            OpenOrCloseModeUI(editModeUI, Mode.Edit);
            OpenOrCloseModeUI(combatModeUI, Mode.Combat);
        }

    }

    void OpenOrCloseModeUI(List<GameObject> modeUI, Mode mode) {

        
        foreach (GameObject go in modeUI) {

            //if item should be opened, open it and add to list of items opened
            if (currentMode == mode) {
                go.SetActive(true);
                justOpened.Add(go);
            }

            //else if it should be closed but was just opened bc it's on another UI that is open, leave it open
            else if (!justOpened.Contains(go))
                go.SetActive(false);

        }

            

    }

}
