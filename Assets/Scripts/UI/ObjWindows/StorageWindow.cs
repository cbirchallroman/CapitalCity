using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageWindow : WorkplaceWindow {

	[Header("Storage Building")]
    public Transform advancedInventoryGrid;
	//public Transform simpleInventoryGrid;
	//public GameObject noItems;
	public Text storageLabel;
	public Image storageBar;

    public override void Open() {

        base.Open();

        StorageBuilding sb = (StorageBuilding)obj;

		ItemType type = sb.typeStored;
		string[] items = new string[0];
		int length = 0;
		switch (type) {
			case ItemType.Food:
				length = (int)FoodType.END;
				items = new string[length];
				Enums.foodDict.Keys.CopyTo(items, 0);
				break;
			case ItemType.Good:
				length = (int)GoodType.END;
				items = new string[length];
				Enums.goodDict.Keys.CopyTo(items, 0);
				break;
			case ItemType.Resource:
				length = (int)ResourceType.END;
				items = new string[length];
				Enums.resourceDict.Keys.CopyTo(items, 0);
				break;
		}
		List<string> sorted = new List<string>(items);
		sorted.Sort();
		foreach (string item in sorted) {

			if (!ResourcesDatabase.ItemAllowed(item))
				continue;

			Node n = Enums.GetItemData(item);
			int index = n.x;

			////for simple grid
			//if (sb.Inventory[index] > 0) {

			//	GameObject g1 = Instantiate(storageitem_smpl);
			//	g1.transform.SetParent(simpleInventoryGrid.transform);

			//	StorageItem_smpl s = g1.GetComponent<StorageItem_smpl>();
			//	s.sb = sb;
			//	s.index = index;

			//	noItems.SetActive(false);

			//}

			//entry in advanced grid
			GameObject g2 = Instantiate(UIObjectDatabase.GetUIElement("StorageItem_adv"));
			g2.transform.SetParent(advancedInventoryGrid);

			StorageItem_adv si = g2.GetComponent<StorageItem_adv>();
			si.index = index;
			si.sb = sb;

		}

    }

	public override void UpdateOverviewPage() {
		base.UpdateOverviewPage();

		StorageBuilding sb = (StorageBuilding)obj;
		storageLabel.text = sb.TotalAmountStored() + "/" + sb.stockpile + " full";
		storageBar.fillAmount = (float)sb.TotalAmountStored() / sb.stockpile;

	}

}
