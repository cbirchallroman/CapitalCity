using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour {

    public Text desc;
    public Button view;
    public Notification Event { get; set; }
    public CameraController Camera { get; set; }
    public NotificationController Controller { get; set; }
    public int Index { get; set; }
    public MenuController CityMenu { get; set; }
    public MenuController NotificationMenu { get; set; }

    public void PrintEvent(bool b) {

        desc.text = b ? Event.ToString() : Event.desc;
        view.gameObject.SetActive(Event.x != -1 && Event.y != -1);

    }

    public void CloseNotification() {

        Controller.Events.RemoveAt(Index);
        Destroy(gameObject);

    }

    public void ViewEvent() {

        if (Event.x == -1 || Event.y == -1)
            Debug.LogError("Should not be able to center camera on this event");
        Camera.MoveCameraTo(Event.x, Event.y);
        CityMenu.CloseMenu();
        NotificationMenu.CloseMenu();

    }

}
