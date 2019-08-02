using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkplaceList : MonoBehaviour {
	
	public GameObject grid;

	public void CreateWorkplaceElement(Workplace w) {

		GameObject go = Instantiate(UIObjectDatabase.GetUIElement("WorkplaceListItem"));
		go.transform.SetParent(grid.transform);

		WorkplaceListItem element = go.GetComponent<WorkplaceListItem>();
		element.Building = w;

	}

}
