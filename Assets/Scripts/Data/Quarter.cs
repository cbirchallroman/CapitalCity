using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quarter  {

    public int season;
    public int year;

    public float wages;
    public float construction;
    public float foodImports;
    public float goodImports;
    public float resourceImports;
    public float maintenance;
    
    public float foodSales;
    public float foodExports;
    public float goodSales;
    public float goodExports;
    public float resourceExports;

    public float TotalImports { get { return foodImports + goodImports + resourceImports; } }
    public float TotalExports { get { return foodExports + goodExports + resourceExports; } }
    public float TotalSales { get { return foodSales + goodSales; } }
    public float Credit { get { return wages + construction + TotalImports; } }
    public float Debit { get { return TotalSales + TotalExports; } }
    public float Balance { get { return Debit - Credit; } }

    public Quarter(int s, int y) {
		
        season = s;
        year = y;

    }

    public override string ToString() {
        return (Season)season + " " + year;
    }

}

[System.Serializable]
public class QuarterList {

    public int currentIndex = 0;
    public static int maxSize = 3;

    public Quarter[] quarters = new Quarter[maxSize];

    public void PushQuarter(Quarter q) {

        if(currentIndex >= maxSize) {
            for (int i = 0; i < quarters.Length - 1; i++)
                quarters[i] = quarters[i + 1];
            quarters[maxSize - 1] = q;
            
        }
        else {
            quarters[currentIndex] = q;
            currentIndex++;
        }

    }

}