using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : MonoBehaviour {

	public Text box;

	public void SetText(string content) {

		box.text = content;

	}

}
