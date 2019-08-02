using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour {

	public Text tooltipTB;
    public GameObject tooltipGO;

	public static Text tooltipText;
	
	// Use this for initialization
	void Start() {
		tooltipText = tooltipTB;
        tooltipText.text = "";
    }

    // Update is called once per frame
    void Update() {
        tooltipGO.transform.position = Input.mousePosition + new Vector3(0, 6, 0);

        //if text is empty, make bg invisible
        tooltipGO.SetActive(!string.IsNullOrEmpty(tooltipText.text));
    }

    public static void SetText(string s) {
        tooltipText.text = s;
        SetColor(Color.white);
    }

    public static void SetColor(Color c){
        tooltipText.color = c;
    }
	
}
