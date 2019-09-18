using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CameraSave {

    public Float3d position, rotation;

    public CameraSave(CameraController cc) {

        position = new Float3d(cc.transform.position);
        rotation = new Float3d(cc.transform.eulerAngles);

    }

}

public class CameraController : MonoBehaviour {

    public float lowerClamp = 4;
    public float upperClamp = 24;
    public float startDist = 10;
    public float cameraSpeedBase = .5f;
    public float CameraSpeed { get { return mapCamera.orthographicSize / startDist * cameraSpeedBase; } }
    public bool StopMovement { get; set; }

    public Button zoomOut;
    public Button zoomIn;
    
    public Camera mapCamera;
    public WorldController worldController;

    Vector3 up { get { return new Vector3(CameraSpeed, 0, CameraSpeed); } }
    Vector3 down { get { return new Vector3(-CameraSpeed, 0, -CameraSpeed); } }
    Vector3 left { get { return new Vector3(-CameraSpeed, 0, CameraSpeed); } }
    Vector3 right { get { return new Vector3(CameraSpeed, 0, -CameraSpeed); } }

    public void Load(CameraSave cc) {

        transform.position = cc.position.GetVector3();
        transform.eulerAngles = cc.rotation.GetVector3();

    }

    void Start() {

        mapCamera.orthographicSize = startDist;

    }

    // Update is called once per frame
    void Update() {

        if (!StopMovement) {
            MoveCamera();
            RotateCamera();
            ZoomIncrementally();
            UpdateZoomButtons();
            ZoomSmoothly();
        }

    }

    void ZoomIncrementally() {

		if (InputController.GetPositiveInputDown("ZoomButton"))
			Zoom(5);
		if (InputController.GetNegativeInputDown("ZoomButton"))
			Zoom(-5);

		//if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals))
        //if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
    }
    void RotateCamera() {

		if(InputController.GetPositiveInputDown("Rotate"))
			RotateCamera(90);
		if(InputController.GetNegativeInputDown("Rotate"))
			RotateCamera(-90);

	}
    void MoveCamera() {

		if (InputController.GetPositiveInput("Vertical"))
			transform.Translate(up);
		if (InputController.GetNegativeInput("Vertical"))
			transform.Translate(down);
		
		if (InputController.GetPositiveInput("Horizontal"))
			transform.Translate(right);
		if (InputController.GetNegativeInput("Horizontal"))
			transform.Translate(left);
		
		ClampCamera();

	}

	void ClampCamera() {

		Node worldSize = worldController.Map.size;
		int sizex = worldSize.x;
		int sizey = worldSize.y;
		float currentx = transform.position.x;
		float currenty = transform.position.z;
		if (currentx < 0)
			transform.position = new Vector3(0, 0, currenty);
		if (currentx > sizex)
			transform.position = new Vector3(sizex, 0, currenty);
		if (currenty < 0)
			transform.position = new Vector3(currentx, 0, 0);
		if (currenty > sizey)
			transform.position = new Vector3(currentx, 0, sizey);

	}

    public void ZoomSmoothly() {

		Zoom(Camera.main.orthographicSize * InputController.GetAxis("Mousewheel"));

	}

    public void Zoom(float f) {
        mapCamera.orthographicSize -= f;
        mapCamera.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, lowerClamp, upperClamp);
    }

    public void RotateCamera(float deg) {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += deg;
        transform.eulerAngles = rotation;
    }

    void UpdateZoomButtons() {

        zoomIn.interactable = mapCamera.orthographicSize != lowerClamp;
        zoomOut.interactable = mapCamera.orthographicSize != upperClamp;

    }

    public void MoveCameraTo(float x, float y) {

        transform.position = new Vector3(x, 0, y);

    }

}