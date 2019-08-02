using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiplomacySave {

    public List<City> cities;

    public DiplomacySave(DiplomacyController dc) {

        cities = dc.cities;

    }

}


public class DiplomacyController : MonoBehaviour {

    public List<City> cities;
    public DiplomacyMenu diploScreen;
    public TradeController tradeController;
    public GameObject cityGrid;
    public GameObject noCities;

    public void Load(DiplomacySave dc) {

        cities = dc.cities;

    }

    private void Start() {

        if(noCities != null)
            noCities.SetActive(cities.Count == 0);
        
        foreach(City c in cities) {

            GameObject go = Instantiate(UIObjectDatabase.GetUIElement("CityButton"));
            go.transform.SetParent(cityGrid.transform);
            go.transform.localScale = new Vector3(1, 1, 1);

            CityButton cb = go.GetComponent<CityButton>();
            cb.city = c;
            cb.diplomacy = this;
            cb.diploScreen = diploScreen;

        }

    }

}
