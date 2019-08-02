using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectDatabase : MonoBehaviour {

	public static GameObject GetUIElement(string name) {
		
		return Resources.Load<GameObject>("UI/" + name);

	}

}
