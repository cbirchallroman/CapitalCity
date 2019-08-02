using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenEscapeMenu : MonoBehaviour {

    public MenuController escapeMenu;
    public ModeController modeController;
    public Mode onMode;
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape) && onMode == modeController.currentMode)
            escapeMenu.OpenMenu();

	}

}
