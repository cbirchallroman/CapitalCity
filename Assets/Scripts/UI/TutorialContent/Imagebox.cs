using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Imagebox : MonoBehaviour {

	public Image box;

	public void SetImage(string line) {
		
		box.sprite = Resources.Load<Sprite>("TutorialImages/" + line);
		//box.sprite = Resources.Load<Sprite>("Sprites/beef");
		box.SetNativeSize();

	}

}
