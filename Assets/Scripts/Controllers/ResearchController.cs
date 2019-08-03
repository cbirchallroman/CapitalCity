using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSave {

	public int researchCounter;
	public Technology currentTech;

	public ResearchSave(ResearchController rc) {

		researchCounter = rc.researchCounter;
		currentTech = rc.currentTech;

	}

}

public class ResearchController : MonoBehaviour {
	
	public int researchCounter;
	public Technology currentTech;

	//possible variables: machineBonus, hygieneBonus

	public void Load(ResearchSave rc) {

		researchCounter = rc.researchCounter;
		currentTech = rc.currentTech;

	}

	//buildings should iterate once a month
	public void IterateResearch(int points) {

		if (currentTech == null)
			return;

		researchCounter += points;
		if (researchCounter >= currentTech.cost)
			FinishResearch();

	}

	void FinishResearch() {

		currentTech = null;

	}

}
