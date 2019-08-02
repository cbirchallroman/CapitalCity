using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PopulationSave {

	public int Working;
	public List<Adult> Proles;

	public PopulationSave(PopulationController pc) {

		Working = pc.Working;
		Proles = pc.Proles;

	}

}

public class PopulationController : MonoBehaviour {

	public List<Adult> Proles { get; set; }
	public TextAsset firstnameList;
	public TextAsset surnameList;
	public Text popLabel;

	public int Population { get { return Proles.Count; } }
	public int Working { get; set; }
	public int Unemployed { get { return Population - Working; } }
	public int UnemployedPercent { get { return (int)((float)Unemployed / Population * 100); } }
	public int EmployedPercent { get { return (int)((float)Working / Population * 100); } }

	public static string[] surnames, firstnames;
	
	public void Update() {

		if (Proles.Count == 0)
			popLabel.text = "0 Proles";
		else
			popLabel.text = Population + " Proles (" + UnemployedPercent + "%)";

	}

	public void Start() {
		
		Proles = new List<Adult>();

		//load surname database
		firstnames = firstnameList.text.Split('\n');
		surnames = surnameList.text.Split('\n');

	}

	public void Load(PopulationSave pc) {

		Working = pc.Working;
		Proles = pc.Proles;

	}

	public void AddProle(Adult p) {

		Proles.Add(p);

	}

	public void RemoveProle(Adult p) {

		Proles.Add(p);

	}

	public void EmployProle(Adult p) {

		Working++;

	}

	public void UnemployProle(Adult p) {

		Working--;

	}

	public static string GetRandomSurname() {

		int rand = Random.Range(0, surnames.Length);
		return surnames[rand];

	}

	public static string GetRandomFirstName() {

		int rand = Random.Range(0, firstnames.Length);  //note: masc names are even, femme names are odd
		return firstnames[rand];

	}

}
