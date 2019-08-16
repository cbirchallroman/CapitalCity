using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundToggler : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

		GetComponent<Image>().enabled = transform.GetSiblingIndex() % 2 == 1;

	}


}
