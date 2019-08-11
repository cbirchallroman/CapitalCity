using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PopulationSave {

	public int Working;
	public List<Prole> Proles;

	public PopulationSave(PopulationController pc) {

		Working = pc.Working;
		Proles = pc.Proles;

	}

}

public class PopulationController : MonoBehaviour {

	public List<Prole> Proles { get; set; }
	public TextAsset firstnameList;
	public TextAsset surnameList;
	public Text popLabel;

	public int Population { get { return Proles.Count; } }
	public int Working { get; set; }
	public int Unemployed { get { return Population - Working; } }
	public int UnemployedPercent { get { return (int)((float)Unemployed / Population * 100); } }
	public int EmployedPercent { get { return (int)((float)Working / Population * 100); } }
	
	public void Update() {

		if (Proles.Count == 0)
			popLabel.text = "0 Proles";
		else
			popLabel.text = Population + " Proles (" + UnemployedPercent + "%)";

	}

	public void Start() {
		
		Proles = new List<Prole>();

	}

	public void Load(PopulationSave pc) {

		Working = pc.Working;
		Proles = pc.Proles;

	}

	public void AddProle(Prole p) {

		Proles.Add(p);

	}

	public void RemoveProle(Prole p) {

		Proles.Add(p);

	}

	public void EmployProle(Prole p) {

		Working++;

	}

	public void UnemployProle(Prole p) {

		Working--;

	}

}
