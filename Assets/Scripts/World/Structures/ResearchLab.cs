using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchLab : Workplace {

	public int ResearchPoints { get { return WorkersCount * WorkingDay / 2; } }

	public override void DoEveryMonth() {

		base.DoEveryMonth();
		research.IterateResearch(ResearchPoints);

	}

}
