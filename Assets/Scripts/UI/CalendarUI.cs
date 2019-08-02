using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarUI : MonoBehaviour {

    public Image seasonClock;
    public Text seasonLabel;
    public Text timeLabel;
    public TimeController time;

    private void Update() {

        seasonClock.fillAmount = (float)time.Months / TimeController.SeasonTime + 
            (float)time.Weeks / TimeController.MonthTime / TimeController.SeasonTime + 
            (float)(time.Days + 1) / TimeController.WeekTime / TimeController.MonthTime / TimeController.SeasonTime;
        seasonLabel.text = time.CurrentSeason + "";
        timeLabel.text = time.ToString();

    }

}
