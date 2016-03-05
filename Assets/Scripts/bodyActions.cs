using UnityEngine;
using System.Collections;

public class bodyActions : MonoBehaviour {


	int nTrail = 1000;
	float [] x;
	float [] y;
	float [] z;
	int iPos=0;
	int iPosLast=0;
	bool iFilled=false;
	Vector3 lastX;


	// Use this for initialization
	void Awake () {
		x = new float[nTrail];
		y = new float[nTrail];
		z = new float[nTrail];
		
	}

	public void Clear() {
		iPos = 0;
		iFilled = false;
	}

	public void Push(Vector3 newX) {

		bool proceed=false;
		if (iPos == 0 && !iFilled) {
			proceed=true;
		}
		if(!proceed) {
			if((newX-lastX).sqrMagnitude>0.005) {
				proceed=true;
			}
		}
		
		if(proceed) {
			x [iPos] = newX.x;
			y [iPos] = newX.y;
			z [iPos] = newX.z;
			lastX.x=x[iPos];
			lastX.y=y[iPos];
			lastX.z=z[iPos];
			iPosLast=iPos;
			iPos++;
			if (iPos == nTrail) {
				iPos = 0;
				iFilled = true;
			}
		}
	}
	
	private Material mat;
	
	// Will be called from camera after regular rendering is done.
	public void OnRenderObject ()
	{
		if( !mat ) {
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
		int start = iPos;
		if (iFilled)
			start += nTrail;
		int end = 0;
		if (iFilled)
			end = iPos + 1;
		for (int i=start-1; i>end; i--) {
			int inow= i%nTrail;
			int inext = (i-1)%nTrail;
			GL.Vertex3 (x[inow],y[inow],z[inow]);
			GL.Vertex3 (x[inext],y[inext],z[inext]);
		}
		GL.End ();
		
		GL.PopMatrix ();
	}
}
