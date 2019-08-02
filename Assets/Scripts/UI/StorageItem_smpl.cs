using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageItem_smpl : MonoBehaviour {

    public StorageBuilding sb { get; set; }
    public int index { get; set; }
    public Text itemLabel;

    void Start() {

        UpdateLabel();

    }

    void UpdateLabel() {

        string s = sb.Inventory[index] + " " + Enums.GetItemName(index, sb.typeStored);

        itemLabel.text = s;

    }

}
