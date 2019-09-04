using UnityEngine;
using System.Collections;

public class TiledStructure : Structure {

    [Header("Tiled Structure")]
    public GameObject fourway;
    public GameObject threeway;
    public GameObject twoway;
    public GameObject oneway;
    public GameObject corner;
    public GameObject noway;

    public bool NotJustPlaced { get; set; }
    public int Neighbors { get; set; }

    public void Update() {
        UpdateTiling();
		if (!groundColorSet)
			SetGroundColor();
	}

    public void UpdateTiling() {
        int newNeighbors = FindNeighbors();
        if (Neighbors != newNeighbors && !(newNeighbors == 0 && !NotJustPlaced)) {
            Neighbors = newNeighbors;
            NotJustPlaced = true;
            UpdateGraphic();
        }
    }

    public virtual bool NeighborCondition(int a, int b) { return true; }

    public virtual int FindNeighbors() {

        int n = 0;
        if (NeighborCondition(X - 1, Y))
            n += 1;
        if (NeighborCondition(X, Y + 1))
            n += 2;
        if (NeighborCondition(X + 1, Y))
            n += 4;
        if (NeighborCondition(X, Y - 1))
            n += 8;
        return n;
        
    }

    public void UpdateGraphic() {
        
        int newNeighbors = FindNeighbors();

        if (threeway == null || corner == null || fourway == null || twoway == null || oneway == null || noway == null)
            return;
        threeway.SetActive(false);
        corner.SetActive(false);
        fourway.SetActive(false);
        twoway.SetActive(false);
        oneway.SetActive(false);
        noway.SetActive(false);

        //Cross roads
        if (newNeighbors == 15) {
            fourway.SetActive(true);
            transform.eulerAngles = new Vector3(0, Random.Range(0, 4) * 90, 0);
        }

        //T-shaped roads
        else if (newNeighbors == 11) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (newNeighbors == 13) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (newNeighbors == 14) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 270, 0);
        }
        else if (newNeighbors == 7) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        //Corner roads
        else if (newNeighbors == 9) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (newNeighbors == 12) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 270, 0);
        }
        else if (newNeighbors == 6) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (newNeighbors == 3) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }

        //Straight roads
        else if (newNeighbors == 10) {
            twoway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (newNeighbors == 5) {
            twoway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        //End roads
        else if (newNeighbors == 1) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (newNeighbors == 2) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (newNeighbors == 4) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (newNeighbors == 8) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 270, 0);
        }

        //default
        else {
            noway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

}
