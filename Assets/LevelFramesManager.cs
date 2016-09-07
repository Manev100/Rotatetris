using UnityEngine;
using System.Collections;

public class LevelFramesManager : MonoBehaviour {

	GameObject[] levelFrames = new GameObject[18];
	// Use this for initialization
	void Start () {
		for(int i = 0; i < transform.childCount; i++){
			Transform child = transform.GetChild(i);
			if(child){
				levelFrames[i] = child.gameObject; 
			}
			changeStateOfLevelFrame(i, false);
		}
	}


	public void deactivateLevelFrame(int level){
		if(level < 20 && level > 1){
			changeStateOfLevelFrame(level-2, false);
		}
	}

	public void activateLevelFrame(int level){
		if(level < 20 && level > 1){
			changeStateOfLevelFrame(level-2, true);
		}
	}

	private void changeStateOfLevelFrame(int frameIndex, bool visible){
		Transform child = levelFrames[frameIndex].transform;
		Vector3 position = child.position;
		if(visible){
			position.z = 0;
		}else{
			position.z = 5;
		}
		child.position = position;
	}

}
