using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationController : MonoBehaviour {

    public NotificationUI[] buttons = new NotificationUI[(int)NotificationType.END];
    public List<Notification> Events { get; set; }
    public GameObject eventListGrid;
    public CameraController cameraController;
    public MenuController cityMenu;
    public MenuController notificationMenu;

    int bannerDisplayTime;

    public void Start() {

        CloseBanners();

    }

    float timeDelta = 0;
    private void Update() {

        timeDelta += Time.deltaTime;

        if(timeDelta >= 1) {

            timeDelta = 0;
            UpdateBanner();

        }

    }

    public void FreshEvents() {

        Events = new List<Notification>();

    }

	public void LoadEvents(List<Notification> save) {

		Events = save;

	}

    public void NewNotification(Notification n) {

        if (eventListGrid == null)
            return;

        Events.Add(n);

        GameObject go = Instantiate(UIObjectDatabase.GetUIElement("NotificationListItem"));
        go.transform.SetParent(eventListGrid.transform);
        go.transform.SetAsFirstSibling();
        go.transform.localScale = new Vector3(1, 1, 1);

        NotificationUI listitem = go.GetComponent<NotificationUI>();
        listitem.gameObject.SetActive(true);
        listitem.Controller = this;
        listitem.Event = n;
        listitem.Index = Events.Count - 1;
        listitem.PrintEvent(true);
        listitem.Camera = cameraController;
        listitem.CityMenu = cityMenu;
        listitem.NotificationMenu = notificationMenu;

        NotificationUI banner = buttons[(int)n.type];
        notificationMenu.OpenMenu(banner.gameObject);
        banner.NotificationMenu = notificationMenu;
        banner.CityMenu = cityMenu;
        banner.Event = n;
        banner.Camera = cameraController;
        banner.PrintEvent(false);
        bannerDisplayTime = 24;
        UpdateBanner();

    }

    public void UpdateBanner() {

        if (bannerDisplayTime > 0)
            bannerDisplayTime--;

        else if(notificationMenu.menu.activeSelf)
            CloseBanners();


    }

    public void CloseBanners() {

        if (buttons == null)
            return;
        notificationMenu.CloseMenu();

    }

    public void ClearNotifications() {

        foreach(Transform child in eventListGrid.transform) {
			
            NotificationUI n = child.gameObject.GetComponent<NotificationUI>();
            n.CloseNotification();

        }

    }

}

[System.Serializable]
public class Notification {

    public NotificationType type;
    public string desc;
    public string Date { get { return month + ", Year " + year; } }
    public int x, y, year;
    public Month month;

    public Notification(NotificationType t, string d, TimeController time) : this(t, d, -1, -1, time) { }

    public Notification(NotificationType t, string d, int a, int b, TimeController time) {

        type = t;
        desc = d;
        x = a;
        y = b;
        month = time.CurrentMonth;
        year = time.CurrentYear;

    }

    public override string ToString() {

        return desc + " (" + Date + ")";

    }

}