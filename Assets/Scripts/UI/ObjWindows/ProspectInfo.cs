using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProspectInfo : MonoBehaviour {

	public Jobcentre jobcentre { get; set; }
	public Prole prospect { get; set; }

	public Text nameLabel;
	public Text phyLabel;
	public Text intLabel;
	public Text emoLabel;

	public Button accept;
	public Button reject;

	public Image countdown;

	private void Start() {

		UpdateLabels();

	}

	void Update() {

		UpdateLabels();
		if (!jobcentre.Prospects.Contains(prospect))
			Destroy(this);

	}

	void UpdateLabels() {

		nameLabel.text = prospect.FullName + " (" + prospect.yearsOld + ")";
		phyLabel.text = prospect.physique + "";
		intLabel.text = prospect.intellect + "";
		emoLabel.text = prospect.emotion + "";
		countdown.fillAmount = prospect.WaitTimePercent();

	}

	public void Accept() {

		jobcentre.AcceptProspect(prospect);
		accept.interactable = false;
		reject.interactable = true;
		countdown.color = Color.green;

	}

	public void Reject() {

		jobcentre.RejectProspect(prospect);
		accept.interactable = true;
		reject.interactable = false;
		countdown.color = Color.red;

	}

}
