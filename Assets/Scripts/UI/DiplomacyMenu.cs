using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DiplomacyMenu : MonoBehaviour {
    
    public DiplomacyController diplomacyController;
    public TradeController tradeController;

    public MenuController cityMenu;
	public MenuController diploMenu;
    public GameObject cityPage;
    public GameObject exportsGrid;
    public GameObject importsGrid;

    public Text cityName;

    City CurrentCity { get; set; }
    List<ItemOrder> exports;
    List<ItemOrder> imports;

    public void OpenTrade(ItemOrder io) {

        if (CurrentCity == null)
            return;

        if (tradeController.ContainsDeal(io))
            Debug.LogError("Order already open");

		tradeController.OpenDeal(io);
        //tradeController.PrintOrders();

    }

    public void CloseTrade(ItemOrder io) {

        if (CurrentCity == null)
            return;

        if (CurrentCity != io.city)
            Debug.LogError("Ending deal with the wrong city");
    
        tradeController.TradeOrders.Remove(io);
        //tradeController.PrintOrders();

    }

    public void OpenCity(City c) {

        CloseCity();
        CurrentCity = c;

        if (c == null)
            Debug.LogError("Tried to open null city");

        cityName.text = CurrentCity.name;
        cityMenu.OpenMenu(cityPage);
		diploMenu.OpenMenu();

        exports = c.GetPossibleExports();
        foreach(ItemOrder export in exports) {

			//don't allow trade if item is not whitelisted
			if (!ResourcesDatabase.ItemAllowed(export.GetItemName()))
				continue;

			GameObject deal = Instantiate(UIObjectDatabase.GetUIElement("TradeDeal"));
            deal.transform.SetParent(exportsGrid.transform);

            TradeDeal td = deal.GetComponent<TradeDeal>();
            td.order = export;
            td.trade = tradeController;
            td.diplo = this;

        }

        imports = c.GetPossibleImports();
        foreach (ItemOrder import in imports) {

			//don't allow trade if item is not whitelisted
			if (!ResourcesDatabase.ItemAllowed(import.GetItemName()))
				continue;

			GameObject deal = Instantiate(UIObjectDatabase.GetUIElement("TradeDeal"));
            deal.transform.SetParent(importsGrid.transform);

            TradeDeal td = deal.GetComponent<TradeDeal>();
            td.order = import;
            td.trade = tradeController;
            td.diplo = this;

        }

    }

    public void CloseCity() {

        CurrentCity = null;
        foreach (Transform child in exportsGrid.transform) if(child.GetComponent<TradeDeal>() != null)
            Destroy(child.gameObject);
        foreach (Transform child in importsGrid.transform) if (child.GetComponent<TradeDeal>() != null)
                Destroy(child.gameObject);
		diploMenu.CloseMenu();

    }

}
