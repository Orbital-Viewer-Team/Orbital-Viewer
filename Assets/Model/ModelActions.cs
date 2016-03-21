using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading;

public class ModelActions : MonoBehaviour {

	public int nb=10;
	int nbmax=100;
	public Vector3 [] x;
	Vector3 [] v;
	Vector3 [] a;
	public float [] m;
	public float rv;
	public Vector3 sight; 

	public GameObject bodyPRE;
	public GameObject [] bodyGos;

	public float G = 2.941e-4f;

	float threadDT=0.001f;
	public Thread modelThread;

	public bool threadRunning=true;
	public bool stepFree=true;
	public bool stepRunning = false;
	public float t = 0.0f;

	bool threaded = true;
	bool fastrun=true;

	bool paused = false;

	static readonly object _locker = new object();

	public bool GetThreaded() {
		return threaded;
	}

	public void pushObject(float mass, Vector3 position, Vector3 velocity) {
		lock(_locker) {
			stepFree=false;
			// would a lock be more efficient here?
			while(stepRunning) {} // wait for step to finish to avoid race condition
			// grow array if needed
			if(nb==nbmax) {
				nbmax *= 2;
				Vector3 [] temp_x = new Vector3[nbmax];
				Vector3 [] temp_v = new Vector3[nbmax];
				float [] temp_m = new float[nbmax];
				GameObject [] temp_g = new GameObject[nbmax];
				for(int j=0;j<nb;j++) temp_x[j]=x[j];
				for(int j=0;j<nb;j++) temp_v[j]=v[j];
				for(int j=0;j<nb;j++) temp_m[j]=m[j];
				for(int j=0;j<nb;j++) temp_g[j]=bodyGos[j];
				x = temp_x;
				v = temp_v;
				m = temp_m;
				bodyGos = temp_g;
				a = new Vector3[nbmax];
			}
		     
			// add new object
			x[nb]=position;
			v[nb]=velocity;
			m[nb]=mass;
			bodyGos[nb] = (GameObject) Instantiate (bodyPRE,x[nb],Quaternion.identity);
			// adjust to conserve momentum
			float netMass = 0.0f;
			for(int j=0;j<nb;j++) netMass += m[j];
			for(int j=0;j<nb;j++) v[j] -= mass/netMass*velocity;
			//update number of objects
			nb++;
			stepFree=true;
		}


	}

	public void popObject(int j) {
		lock(_locker) {
			stepFree=false;
			// would a lock be more efficient here?
			while(stepRunning) {} // wait for step to finish to avoid race condition
			// grow array if needed



			GameObject temp = bodyGos[j];
			for(int k=j;k<nb-1;k++) {
				x[k]=x[k+1];
				v[k]=v[k+1];
				m[k]=m[k+1];
				bodyGos[k]=bodyGos[k+1];
			}
			nb--;

			Destroy (temp);
			stepFree=true;
		}
		
		
	}

	// Use this for initialization
	void Start () {
		// initialize problem
		x = new Vector3[nbmax];
		v = new Vector3[nbmax];
		a = new Vector3[nbmax];
		m = new float[nbmax];
		bodyGos = new GameObject[nbmax];

		float mstar = 1.0f;
		// sun
		x [0] = Vector3.zero;
		v [0] = Vector3.zero;
		a [0] = Vector3.zero;
		m [0] = mstar; // solar mass

		// mercury venus earth mars
		float r = 0.4644f;
		int iplan = 1;
		x [iplan] = new Vector3 (r,0f,0f);
		float vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 0.0552734f/332948.6f;

		r = 0.7282313f;
		iplan = 2;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 0.814996f/332948.6f;

		r = 1.0f;
		iplan = 3;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 1.0f/332948.6f;

		r = 1.7f;
		iplan = 4;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 0.107447f/332948.6f;

		//Jupiter
		r = 5.203f;
		iplan = 5;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 317.8f/332948.6f;

		//Jupiter
		r = 5.203f;
		iplan = 5;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 317.8f/332948.6f;


		//saturn
		r = 9.52f;
		iplan = 6;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 95f/332948.6f;

		//Uranus
		r = 19.2f;
		iplan = 7;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 14.5f/332948.6f;

		//neptune
		r = 30f;
		iplan = 8;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 17.1f/332948.6f;

		//pluto
		r = 39f;
		iplan = 9;
		x [iplan] = new Vector3 (r,0f,0f);
		vp = Mathf.Sqrt (G * mstar / r);
		v [iplan] = new Vector3 (vp*x[iplan].z/r,0f,-vp*x[iplan].x/r);
		a [iplan] = Vector3.zero;
		m [iplan] = 0.0025f/332948.6f;




		for(int j=0;j<nb;j++) {
			bodyGos[j] = (GameObject) Instantiate (bodyPRE,x[j],Quaternion.identity);
		}

		bodyGos [0].GetComponent<Renderer> ().material.color = Color.yellow;
		bodyGos [0].transform.localScale *= 5.0f;
		bodyGos [1].GetComponent<Renderer> ().material.color = Color.red;
		bodyGos [2].GetComponent<Renderer> ().material.color = Color.yellow;
		bodyGos [3].GetComponent<Renderer> ().material.color = Color.blue;
		bodyGos [4].GetComponent<Renderer> ().material.color = Color.red;
		//new
		bodyGos [5].GetComponent<Renderer> ().material.color = Color.gray;
		bodyGos [5].transform.localScale *= 2.0f;
		bodyGos [6].GetComponent<Renderer> ().material.color = Color.cyan;
		bodyGos [7].GetComponent<Renderer> ().material.color = Color.green;
		bodyGos [8].GetComponent<Renderer> ().material.color = Color.magenta;
		bodyGos [9].GetComponent<Renderer> ().material.color = Color.red;


		/*
		for(int j=0;j<n;j++) {
			x[j] = Random.insideUnitSphere*50.0f;
			v[j] = Random.insideUnitSphere*15.0f;
			a[j] = Vector3.zero;
			m[j] = 10000.0f/(float)n;
			bodyGos[j] = (GameObject) Instantiate (bodyPRE,x[j],Quaternion.identity);
		}*/


		if (threaded) {
			modelThread = new Thread (this.threadedActions);
			modelThread.Start ();
		}

	}

	public void threadedActions() {
		while (threadRunning) {

			try 
			{
				if(stepFree||fastrun) {
					stepRunning=true;

					//float threadDT=0.001;
					takeStep(threadDT);

					stepRunning=false;
				}
				if(!fastrun) lock(_locker) {
					stepFree=false;
				}
			} 
			catch (ThreadAbortException ex) 
			{
				threadRunning=false;
			}



		}
	}

	void Center() {
		lock(_locker) {
			stepFree=false;
			// would a lock be more efficient here?
			while(stepRunning) {} // wait for step to finish to avoid race condition

			Vector3 COM = Vector3.zero;
			Vector3 COP = Vector3.zero;
			float M = 0.0f;

			for(int j=0;j<nb;j++) {
				COM += m[j]*x[j];
				COP += m[j]*v[j];
				M += m[j];
			}
			COM /= M;
			COP /= M;

			for(int j=0;j<nb;j++) {
				x[j] -= COM;
				v[j] -= COP;
			}
			
			
			stepFree=true;
		}
	}

	void FixedUpdate() {
		if (threaded) {
			lock (_locker) {
				stepFree = true;
			}
		} else {
			stepRunning = true;
		
		//float threadDT=0.001;
			takeStep (Time.fixedDeltaTime);

			stepRunning = false;
		}		
	}

	//
	//
	//


	public void Pause(bool yesNo) {
		lock(_locker) {
			stepFree=false;
			// would a lock be more efficient here?
			while(stepRunning) {} // wait for step to finish to avoid race condition
			// grow array if needed
			paused=yesNo;
		}
	}

	void takeStep(float dt) {
		if (!paused) {
			for (int j=0; j<nb; j++) {
				a [j] = Vector3.zero;
			}



		
			for (int j=0; j<nb; j++) {
				for (int k=j+1; k<nb; k++) {
					Vector3 dx = (x [k] - x [j]);
					float dx2 = dx.sqrMagnitude;
					float dx3 = dx2 * dx.magnitude;
					if (dx3 > 1.0e-9) {
						a [j] += G * m [k] / dx3 * dx;
						a [k] -= G * m [j] / dx3 * dx;
						//Debug.Log ("STEP "+a[j].magnitude);
					}
				}
			}
		
			for (int j=0; j<nb; j++) {
				v [j] += a [j] * dt;
				x [j] += v [j] * dt;

			}
			//sight = new Vector3(GameObject.Find("Player").GetComponent<PlayerMovement>().cc.transform.position - bodyGos[0].transform.position);
			sight = Camera.main.transform.position - x [0];
			rv = Vector3.Dot (sight, v [0]);
			for (int j=0; j<nb; j++) {
				bodyGos [j].GetComponent<bodyActions> ().Push (x [j]);
			}
			t += dt;



		}
	}




	// Update is called once per frame
	void Update () {
		for(int j=0;j<nb;j++) {
			bodyGos[j].transform.position=x[j];
		}

		Debug.Log ("" + nb);

		float timestep = Input.GetAxis ("Timestep");
		threadDT *= (1.0f + 0.25f * timestep);
		for(int j=0;j<nb;j++) {
			if(x[j].sqrMagnitude>2000.0f) {
				popObject(j);
				break;
			}
		}

		Center ();

		float dt;
		if (threaded)
			dt = threadDT;
		else
			dt = Time.fixedDeltaTime;
		GameObject.Find ("TimeDisplay").GetComponent<Text> ().text = string.Format ("Time = {0} dt = {1}", t, dt);

		sight = Camera.main.transform.position - x [0];
		rv = Vector3.Dot (sight, v [0]);
		GameObject.Find ("rv").GetComponent<Text> ().text = string.Format ("Radial Velocity = {0}", rv );


		if (Input.GetButtonDown ("Cancel")) {
			// clear all trail
			for(int j=0;j<nb;j++) {
				bodyGos[j].GetComponent<bodyActions>().Clear();
			}
		}


	}
}

