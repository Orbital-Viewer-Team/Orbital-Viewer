using UnityEngine;
using System.Collections;

public class ProjectionActions : MonoBehaviour {

	Vector3 velocity;
	public bool show=false;

	// Use this for initialization
	void Start () {
		velocity = transform.forward * 0.01f;
	
	}
	
	// Update is called once per frame
	void Update () {



	
	}

	private Material mat;


	void OnRenderObject() {
		if (show) {
			velocity = transform.parent.forward * 0.02f;
			ModelActions model = GameObject.Find ("Model").GetComponent<ModelActions> ();
			int nProject = 1000;
			float dt = 1.0f;
			Vector3 [] x = new Vector3[nProject];
			Vector3 [] v = new Vector3[nProject];
			x [0] = transform.parent.position;
			v [0] = velocity;
			float t = 0.0f;

			for (int j=0; j<nProject-1; j++) {
				Vector3 a = Vector3.zero;
				for (int k=0; k<model.nb; k++) {
					Vector3 dx = model.x [k] - x [j];
					float dx2 = dx.sqrMagnitude;
					float dx3 = dx2 * dx.magnitude;
					a += model.G * model.m [k] * dx / dx3;
				}
				v [j + 1] = v [j] + a * dt;
				x [j + 1] = x [j] + v [j] * dt;

			}





			if (!mat) {
				// Unity has a built-in shader that is useful for drawing
				// simple colored things.
				var shader = Shader.Find ("VertexColorUnlit");
				mat = new Material (shader);
				mat.hideFlags = HideFlags.HideAndDontSave;
				// Turn on alpha blending
				mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				// Turn backface culling off
				mat.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				// Turn off depth writes
				mat.SetInt ("_ZWrite", 0);
			}
		
			mat.SetPass (0);
		
		
			GL.PushMatrix ();
			// Set transformation matrix for drawing to
			// match our transform
			//GL.MultMatrix (transform.localToWorldMatrix);
		
			GL.Begin (GL.LINES);
			GL.Color (Color.white);
			for (int j=0; j<nProject-1; j++) {
				GL.Vertex (x [j]);
				GL.Vertex (x [j + 1]);
			}

			GL.End ();
		
			GL.PopMatrix ();
		}


	}
}
