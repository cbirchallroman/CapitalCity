using UnityEngine;
using System.Collections;

public class ConstructionSite : Structure {

    [Header("Construction Site")]
    public int timeToConstructMax;
    public int TimeToConstruct { get; set; }
    public string blueprint;
    public int[] ingredientMax;
    public int[] IngredientAmount { get; set; }
    public string[] ingredients;
    public bool Constructing { get; set; }

    public bool HaveEnough(int a) { return IngredientAmount[a] >= ingredientMax[a]; }
    public int IngredientNeeded(int a) { return ingredientMax[a] - IngredientAmount[a]; }

    public override void Activate() {

        base.Activate();

        IngredientAmount = new int[ingredientMax.Length];

    }

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (!ActiveSmartWalker) {

            if (Constructing)
                ConstructionTimer();

            else {

                if (HaveEnough())
                    BeginConstruction();
                else
                    GetMaterials();

            }


        }

    }

    public void ConstructionTimer() {

        if (TimeToConstruct > 0)
            TimeToConstruct--;

        else
            FinishConstruction();

    }

    public void GetMaterials() {

        for (int a = 0; a < ingredientMax.Length; a++)
            if (!HaveEnough(a)) {

                ItemOrder io = new ItemOrder(IngredientNeeded(a), ingredients[a]);

                SpawnGetter(io);

                return;

            }

    }

    public bool HaveEnough() {

        for (int a = 0; a < ingredientMax.Length; a++)
            if (!HaveEnough(a))
                return false;
        return true;

    }

    public void BeginConstruction() {

        for (int a = 0; a < ingredientMax.Length; a++)
            IngredientAmount[a] -= ingredientMax[a];
        Constructing = true;
        TimeToConstruct = timeToConstructMax;

    }

    public override void ReceiveItem(ItemOrder io) {

        for (int a = 0; a < ingredientMax.Length; a++)
            if (ingredients[a] == io.GetItemName())
                IngredientAmount[a] += io.amount;

    }

    public void FinishConstruction() {

        world.Demolish(X, Y);
        world.SpawnStructure(blueprint, X, Y, transform.rotation.y);

    }

}
