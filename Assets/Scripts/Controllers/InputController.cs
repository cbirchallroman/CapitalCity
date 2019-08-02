using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public static class InputController {


	public static bool GetPositiveInputDown(string name) {

		return CrossPlatformInputManager.GetButtonDown(name) && CrossPlatformInputManager.GetAxis(name) > 0;

	}

	public static bool GetNegativeInputDown(string name) {

		return CrossPlatformInputManager.GetButtonDown(name) && CrossPlatformInputManager.GetAxis(name) < 0;

	}

	public static bool GetPositiveInput(string name) {

		return CrossPlatformInputManager.GetButton(name) && CrossPlatformInputManager.GetAxis(name) > 0;

	}

	public static bool GetNegativeInput(string name) {

		return CrossPlatformInputManager.GetButton(name) && CrossPlatformInputManager.GetAxis(name) < 0;

	}

	public static float GetAxis(string name) {

		return CrossPlatformInputManager.GetAxisRaw(name);

	}

}
