using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjWindow : MonoBehaviour {

    public Obj obj;
    public Text title;
    public Text description;
    public WorldController WorldController { get; set; }
    public TimeController TimeController { get; set; }
    public bool WasPaused { get; set; }

	bool wait;

    public virtual void Open() {

        title.text = obj.DisplayName.ToUpper();
        transform.localPosition = new Vector3(0, 0, 0);
		//TimeController = WorldController.timeController;
		//TimeController.StopTime();
		//WorldController.cameraController.StopMovement = true;
		//WorldController.MoveObjViewCamera(obj);

		UpdateOverviewPage();

	}

    private void Update() {

        DoDuringUpdate();

    }

    public virtual void DoDuringUpdate() {

		UpdateOverviewPage();


		//if (!TimeController.IsPaused)
		//	Debug.Log("Window is open when time is unpaused");

		//if (InputController.GetPositiveInputDown("ClickObject") && wait)
		//	Close();

		if (obj == null)
			Close();

		if (!wait)
			wait = true;

	}

	public void CameraToBuilding() {

		WorldController.MoveMainCameraTo(obj);

	}

	public virtual void UpdateOverviewPage() {

		if (description != null)
			description.text = obj.GetDescription();

	}

    public void Close() {
		
        Destroy(gameObject);

    }

	private void OnDestroy() {

		if (obj != null)
			obj.CloseWindow();

	}

}
