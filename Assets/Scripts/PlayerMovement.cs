using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	
	
	//float verticalRotation = 0.0f;
	//Vector3 speed = Vector3.zero;
	CharacterController cc;
	float movementSpeed = 10.0f;
	//float slowDown=0.8f;
	float lookScale = 2.0f;
	int cameraView = 0;

	
	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		cc = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {

		float mouseX = Input.GetAxis ("Mouse X");
		float mouseY = Input.GetAxis ("Mouse Y");
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");
		float roll = Input.GetAxis ("Roll");
		float jump = Input.GetAxis ("Jump");
		
		transform.Rotate (-lookScale*mouseY, lookScale*mouseX,-roll);
		
		cc.Move (movementSpeed*transform.forward * vertical*Time.deltaTime + transform.right * horizontal*movementSpeed*Time.deltaTime
		         +movementSpeed*transform.up*jump*Time.deltaTime);

		/*
		float mouseSpeed = 8.0f;
		float UDRange = 85.0f;
		
		float rotY = Input.GetAxis ("Mouse X");
		transform.Rotate (0,mouseSpeed*rotY,0);
		
		verticalRotation -= Input.GetAxis ("Mouse Y") * mouseSpeed;
		verticalRotation = Mathf.Clamp (verticalRotation, -UDRange, UDRange);
		Camera.main.transform.localRotation = Quaternion.Euler (verticalRotation, 0, 0);
		
		//movement
		
		float forwardSpeed = Input.GetAxis("Vertical");
		float sideSpeed = Input.GetAxis("Horizontal");
		
		
		if (forwardSpeed != 0.0f || sideSpeed != 0.0f) {
			speed += forwardSpeed*Camera.main.transform.forward * movementSpeed * Time.deltaTime +
				Camera.main.transform.right * sideSpeed *movementSpeed* Time.deltaTime;
		} else {
			speed = speed*slowDown*Time.deltaTime;
		}
		
		//speed = transform.rotation * speed;

		cc.Move(speed * Time.deltaTime);

        */

		if (Input.GetButtonDown ("Terminate")) {

			GameObject model = GameObject.Find("Model");
			if(model.GetComponent<ModelActions>().GetThreaded ()) {
				model.GetComponent<ModelActions>().threadRunning=false;
				model.GetComponent<ModelActions>().modelThread.Abort();
			}
			Application.Quit();
		}

		if (Input.GetButtonDown ("Fire1")) {
			GameObject model = GameObject.Find("Model");
			model.GetComponent<ModelActions>().pushObject(0.000000000001f,transform.position + Camera.main.transform.forward, Camera.main.transform.forward*0.02f);
		}

		if (Input.GetButton ("Fire2")) {
			// have model pause
			GameObject.Find("Model").GetComponent<ModelActions>().Pause(true);
			// create "projection object"
			GameObject.Find("Projection").GetComponent<ProjectionActions>().show=true;
			// allow variables to be modified
		}
		if (Input.GetButtonUp ("Fire2")) {
			// destroy projection object
			GameObject.Find("Projection").GetComponent<ProjectionActions>().show=false;
			//create new object in model
			// unpause model 
			GameObject.Find("Model").GetComponent<ModelActions>().Pause(false);
		}

		/*
		// adjust model object scales
		if (true) {
			ModelActions model = GameObject.Find("Model").GetComponent<ModelActions>();
			for(int j=0;j<model.nb;j++) {
				float dx = 0.1f*(model.x[j]-Camera.main.transform.position).magnitude;
				dx = Mathf.Pow(dx,0.5f);
				dx = Mathf.Max(dx,0.1f);
				model.bodyGos[j].transform.localScale = new Vector3(dx,dx,dx);
			}
		}
		*/


		// this doesnt wrok at all this sucks make it better
		Vector3 camShift = new Vector3 (0.0f,0.0f,-25.0f);

		if (Input.GetButtonDown ("Camera")) {
			if(cameraView==0) {
				cameraView=1;
				Camera.main.transform.localPosition += camShift;
				//Camera.main.transform.Rotate(-45.0f,0f,0f);
			} else {
				cameraView=0;
				Camera.main.transform.localPosition -= camShift;
				//Camera.main.transform.Rotate(45.0f,0f,0f);

			}
		}

		
	}
	
}
