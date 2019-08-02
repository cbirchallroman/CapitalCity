using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityButton : MonoBehaviour {

    public City city { get; set; }
    public DiplomacyController diplomacy { get; set; }
    public DiplomacyMenu diploScreen { get; set; }
    public Text nameLabel;

    public void Start() {

        nameLabel.text = city.name;

    }

    public void OpenCity() {

        diploScreen.OpenCity(city);

    }

}
