using UnityEngine;
using System.Collections;

public class PodiumControl : MonoBehaviour {
	public Color primaryColor;
	public Color secondaryColor;

	// Use this for initialization
	void Start () {

		//Setting the primary colour
		GetComponent<Renderer>().material.color = primaryColor;
		
		//Setting the secondary colour
		for (int i = 0; i < transform.childCount; ++i) 
		{
			Renderer rend = transform.GetChild(i).GetComponent<Renderer>();
			if (rend != null)
			{
				rend.material.color = secondaryColor;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
