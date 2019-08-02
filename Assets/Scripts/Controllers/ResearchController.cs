using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchControllerSave {

	public int researchPoints;

	public ResearchControllerSave(ResearchController rc) {

		researchPoints = rc.researchPoints;

	}

}

public class ResearchController : MonoBehaviour {

	public int researchPoints;
	public int researchCounter;
	public Technology currentTech;

	//possible variables: machineBonus, hygieneBonus

	public void Load(ResearchControllerSave rc) {

		researchPoints = rc.researchPoints;

	}

	public void AddResearchPoints(int p) {

		researchPoints += p;

	}

	public void SubtractResearchPoints(int p) {

		researchPoints -= p;

	}

	public void IterateResearch() {

		researchCounter += researchPoints;
		if (researchCounter >= currentTech.cost)
			FinishResearch();

	}

	void FinishResearch() {

		currentTech = null;

	}

}
