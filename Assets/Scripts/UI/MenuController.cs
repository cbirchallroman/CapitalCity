using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public bool pauseWhenOpen;
    public bool dontOpenWhenPaused;
    public bool closeIfAlreadyOpen = true;
    public bool stopMovement = true;
    public bool rightClickToClose;
    public GameObject menu;
    public TimeController timeController;
    bool hasTime { get { return timeController != null; } }
    public ModeController modeController;
    bool hasMode { get { return modeController != null; } }
    public CameraController cameraController;
    bool hasCamera { get { return cameraController != null; } }
    public List<Mode> openOnModes;

    public GameObject mainPage;
    public List<GameObject> pages;
	public List<Toggle> tabs;

	public List<GameObject> hideUI;
    public Dictionary<GameObject, bool> wasVisible = new Dictionary<GameObject, bool>();

    public bool wasPaused { get; set; }
    public bool wasOpen { get; set; }
    
    public void OpenMenu() {

        if (mainPage != null)
            OpenMenu(mainPage);
    }

    private void Update() {

        if (rightClickToClose && InputController.GetPositiveInputDown("ClickObject"))
            CloseMenu();
        if (Time.timeScale == 0 && dontOpenWhenPaused)
            CloseMenu();
		if (pauseWhenOpen && hasTime && menu.activeSelf)
			timeController.StopTime();

	}

	public void OpenTab(Toggle tab) {

		foreach (Toggle t in tabs)
			t.isOn = false;

		tab.isOn = true;

	}

    public void OpenMenu(GameObject page) {

        if (Time.timeScale == 0 && dontOpenWhenPaused)
            return;

        //only open if correct mode
        if(hasMode) if (!openOnModes.Contains(modeController.currentMode))
            return;

        if (hasCamera && stopMovement) cameraController.StopMovement = true;

        //if menu was already open
        if (menu.activeSelf) {

            wasOpen = true;
            
        }

        //if should close when button is pressed again, and the intended page is not the main page
        if (page.activeSelf && closeIfAlreadyOpen && page != mainPage) {

            CloseMenu();
            return;

        }

        //close all pages but desired page
        foreach (GameObject go in pages)
            go.SetActive(false);
        page.SetActive(true);

        //check if the game was paused before
        if(!wasOpen && hasTime)
            wasPaused = timeController.IsPaused;

        //if the game must be paused while open, pause it
        if (pauseWhenOpen && hasTime)
            timeController.StopTime();
            
        menu.SetActive(true);
		menu.transform.SetAsLastSibling();

        TooltipController.SetText("");

        if (hasMode) if (!openOnModes.Contains(modeController.currentMode))
            return;

        foreach (GameObject go in hideUI) {

            //add to dictionary which says if it was active or inactive when reached
            if (!wasOpen)
                wasVisible[go] = go.activeSelf;

            go.SetActive(false);

        }

	}

    public void CloseMenu() {

        //if the game was supposed to be paused but was not paused before, start playing
        if (timeController != null && pauseWhenOpen && (!wasPaused || wasOpen)) {
            timeController.StartTime();
        }

        //otherwise reset setting
        else
            wasPaused = false;
        
        if (hasCamera && stopMovement) cameraController.StopMovement = false;

        //close all pages
        foreach (GameObject go in pages)
            go.SetActive(false);

        menu.SetActive(false);

        foreach (GameObject go in hideUI)
            if (go == null)
                Debug.Log(name);
            else if (wasVisible.ContainsKey(go)) {
                if (wasVisible[go])
                    go.SetActive(true);
            }
            
                

        wasOpen = false;

    }

}
