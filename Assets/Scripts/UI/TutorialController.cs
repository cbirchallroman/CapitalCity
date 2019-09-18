using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {

	public Transform contentGrid;
	public Text title;
	public int tutorialNum = -1;
	public MenuController tutorialMenu;
	public GameObject openTutorial;

	bool hasTutorial { get { return tutorialNum != -1; } }

	private void Start() {

		LoadTutorialContents(tutorialNum);
		openTutorial.SetActive(hasTutorial);

	}

	public void LoadTutorialContents(int num) {

		if (!hasTutorial)
			return;

		string[] contents = ((TextAsset)Resources.Load("TutorialData/" + num)).text.Split('\n');

		//first line of the file is the title
		title.text = "TUTORIAL: " + contents[0];

		//TAKE EACH ITEM FROM ARRAY AND TURN INTO A CONTENT ELEMENT, THEN SET PARENT TO CONTENTGRID
		for(int i = 1; i < contents.Length; i++) {

			string line = contents[i];

			if (line[0] == '#')
				CreateHeaderBox(line);
			else if (line[0] == '!')
				CreateImageBox(line);
			else
				CreateTextbox(line);

		}

		//we don't necessarily want the tutorial to open automatically, but this is good to test
		OpenTutorial();

	}

	void CreateHeaderBox(string line) {
		
		line = line.Substring(1, line.Length - 1);   //remove the # character from the beginning

		GameObject go = Instantiate((GameObject)Resources.Load("TutorialUI/Header"));
		go.transform.SetParent(contentGrid);

		Textbox tb = go.GetComponent<Textbox>();
		tb.SetText(line);
		
	}

	void CreateTextbox(string line) {

		GameObject go = Instantiate((GameObject)Resources.Load("TutorialUI/Textbox"));
		go.transform.SetParent(contentGrid);

		Textbox tb = go.GetComponent<Textbox>();
		tb.SetText(line);

	}

	void CreateImageBox(string line) {

		line = line.Substring(1, line.Length - 1);   //remove the ! character from the beginning

		GameObject go = Instantiate((GameObject)Resources.Load("TutorialUI/Image"));
		go.transform.SetParent(contentGrid);

		Imagebox tb = go.GetComponent<Imagebox>();
		tb.SetImage(line);

	}

	public void OpenTutorial() {

		tutorialMenu.OpenMenu();

	}

	public void CloseTutorial() {

		tutorialMenu.CloseMenu();

	}

}
