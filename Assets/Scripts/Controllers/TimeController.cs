using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TimeSave {

    public float timeDelta;
    public int days, weeks, months, years;

    public TimeSave(TimeController tc) {
        
        timeDelta = tc.TimeDelta;
        days = tc.Days;
        weeks = tc.Weeks;
        months = tc.Months;
        years = tc.Years;

    }

}

public class TimeController : MonoBehaviour {

    public Text timeLabel;
    public Image clockSpin;
    public Image clockColor;
	public FinanceMenu finances;
    public MoneyController money;
	public ResearchController research;

    public bool IsPaused { get; set; }

    public float TimeDelta { get; set; }
    public int Days { get; set; }
    public int Weeks { get; set; }
    public int Months { get; set; }
    public int Seasons { get; set; }
    public int Years { get; set; }

    public static int DayTime { get { return 1; } }
    public static int WeekTime { get { return 8; } }	//make 8 to double time
    public static int MonthTime { get { return 4; } }
    public static int SeasonTime { get { return 3; } }
    public static int YearTime { get { return 4; } }

    public static int DaysInAMonth { get { return WeekTime * MonthTime; } }
    public static int DaysInASeason { get { return DaysInAMonth * SeasonTime; } }
    public static int DaysInAYear { get { return DaysInASeason * YearTime; } }

    public float TimeScale { get; set; }
    public float MaxSpeed { get { return 5f; } }
    public float SpeedUnit { get { return MaxSpeed / 10; } }
    public float StartSpeed { get { return 8 * SpeedUnit; } }

	public int startingYear = 1790;
    public int CurrentDay { get { return Days + WeekTime * Weeks + 1; } }
    public Month CurrentMonth { get { return (Month)(Months + Seasons * SeasonTime); } }
    public Season CurrentSeason { get { return (Season)Seasons; } }
    public int CurrentYear { get { return Years + startingYear; } }

    public void UpdateTimeLabel() {

        if(timeLabel != null)
            timeLabel.text = ToString();
        if (clockSpin != null)
            clockSpin.fillAmount = .5f * (TimeDelta + (Days + Weeks * WeekTime + Months * MonthTime * WeekTime)) / DaysInASeason;
        //if (clockSpin != null)
        //    clockSpin.fillAmount = (TimeDelta + (Days + Weeks * WeekTime + Months * MonthTime * WeekTime)) / DaysInASeason;

    }

    void UpdateClockColor(){

        if (clockColor == null)
            return;

        Color c;
        c = new Color(1, 1, 1);

        if (CurrentSeason == Season.Winter)
            c = new Color(0, 0, 1);
        else if (CurrentSeason == Season.Spring)
            c = new Color(0, 1, 0);
        else if (CurrentSeason == Season.Summer)
            c = new Color(1, 1, 0);
        else if (CurrentSeason == Season.Autumn)
            c = new Color(1, 0, 0);

        clockColor.color = c;

    }

    public override string ToString() {
        return CurrentDay + " " + CurrentMonth + "\n" + CurrentYear;
    }

    public void Load(TimeSave w) {

        TimeDelta = w.timeDelta;
        Days = w.days;
        Weeks = w.weeks;
        Months = w.months;
        Years = w.years;
        finances.LoadFinancialReports();

    }

    void Start() {
        //set starting timescale
        TimeScale = StartSpeed;
        Time.timeScale = TimeScale;

        //update label
        UpdateTimeLabel();
        UpdateClockColor();
        finances.LoadFinancialReports();
    }

    void Update () {

        //if (!IsPaused)
        //    Time.timeScale = timescale;

        TimeDelta += Time.deltaTime;

        //update label
        UpdateTimeLabel();

        if (TimeDelta >= DayTime) {
            
            //convert timedelta to day
            TimeDelta = 0;
            Days++;

            //convert days to week
            if(Days >= WeekTime) {

                Days = 0;
                Weeks++;
            }

            //convert weeks to month
            if (Weeks >= MonthTime) {

                Weeks = 0;
                Months++;
				//research.IterateResearch();
				//labor.PayWages();

			}

            //convert months to season
            if (Months >= SeasonTime) {

                Months = 0;
                Seasons++;
                UpdateClockColor();
                if(Seasons < YearTime)
                    money.BeginNewQuarter(Seasons, CurrentYear);
                finances.LoadFinancialReports();
				ProductivityController.UpdateProductivities();

            }

            //convert seasons to year
            if (Seasons >= YearTime) {

                Seasons = 0;
                Years++;
                money.BeginNewQuarter(Seasons, CurrentYear);
                finances.LoadFinancialReports();
                UpdateClockColor();
                

            }

        }

	}

    public void StopTime() {
        Time.timeScale = 0;
        IsPaused = true;
    }
    public void StartTime() {
		Time.timeScale = TimeScale;
        IsPaused = false;
    }
    public void ChangeTimeScale(float t) {
        TimeScale = t / 10 * MaxSpeed;
        if(!IsPaused)
            Time.timeScale = TimeScale;
    }

}
