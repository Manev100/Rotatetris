using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {
	private bool rotating = false;
	private float startTime = 0;
	private Vector3 startRotation;
	private Vector3 targetRotation;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		// rotate for -90 degrees in 1 second
		// produces rounding errors!
		if(rotating){
			Vector3 currentRotation = transform.rotation.eulerAngles;
			Vector3 rotation = Vector3.Lerp(startRotation, targetRotation, (Time.time - startTime));
			transform.Rotate(rotation-currentRotation);
			if((Time.time - startTime) >= 1f){
				rotating = false;
			}
		}
	}

	public void rotate(){
		startRotation = transform.rotation.eulerAngles;
		targetRotation = transform.rotation.eulerAngles + new Vector3(0,0,-90);
		startTime = Time.time;
		rotating = true;
	}
}
