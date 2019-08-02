using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageItem_adv : MonoBehaviour {

    public StorageBuilding sb { get; set; }
    public int index { get; set; }
	public Image itemSprite;
    public Text itemLabel;
    public Text willAcceptLabel;
	public Button accept;
	public Button fill;
	public Text[] texts;

	void Start() {

        UpdateLabel();
        UpdateAcceptButton();

		accept.gameObject.SetActive(sb.WillGet[index]);
		fill.gameObject.SetActive(!sb.WillGet[index]);
		Sprite spr = ResourcesDatabase.GetSprite(Enums.GetItemName(index, sb.typeStored));
		if (spr != null)
			itemSprite.sprite = spr;

	}

	private void Update() {

		UpdateLabel();

	}

	void UpdateLabel() {

        string s = sb.Inventory[index] + " " + Enums.GetItemName(index, sb.typeStored);

        itemLabel.text = s;

    }

    //switch from 0 to 1/4 to 1/2 to 3/4 to 1. if over 1, go to 0
    public void ChangeAccept() {

        sb.WillAccept[index] += .25f;
        if (sb.WillAccept[index] > 1)
            sb.WillAccept[index] = 0;

        UpdateAcceptButton();

    }

    void UpdateAcceptButton() {

        float fl = sb.WillAccept[index];
		int c = (int)(4f * fl);

		switch (c) {
			case 0:
				willAcceptLabel.text = "N/A";
				break;
			case 1:
				willAcceptLabel.text = "1/4";
				break;
			case 2:
				willAcceptLabel.text = "1/2";
				break;
			case 3:
				willAcceptLabel.text = "3/4";
				break;
			case 4:
				willAcceptLabel.text = "ALL";
				break;
		}

		foreach (Text t in texts)
			t.color = c == 0 ? Color.yellow : Color.white;

    }

    public void SetGet(bool b) {

        sb.WillGet[index] = b;
		accept.gameObject.SetActive(sb.WillGet[index]);
		fill.gameObject.SetActive(!sb.WillGet[index]);

	}

}
