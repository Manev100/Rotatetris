using UnityEngine;
using System.Collections;

public class Block {
	// x and y are coordinates of the block when the piece is set to a place. 
	// when rotating the board the objects coordinates (x,y) are not updated(could be implemented, but not needed now)
	// the level stays the same even after a rotation and only the start coodinates are needed to compute the level after tetrises have been deleted 
	public int x;
	public int y;

	private int level;
	private GameObject blockObject;

	public Block(int x, int y){
		this.x = x;
		this.y = y;
		computeLevel();
	}

	public Block(int x, int y, GameObject aObject): this(x,y){
		setBlockObject(aObject);
	}

	private void computeLevel(){
		// change origin
		float altX = x + 0.5f;
		float altY = y + 0.5f;
		if(Mathf.Abs(altX) >= Mathf.Abs(altY)){
			level = Mathf.RoundToInt(Mathf.Abs(altX) + 0.5f);
		}else if(Mathf.Abs(altY) > Mathf.Abs(altX)){
			level = Mathf.RoundToInt(Mathf.Abs(altY) + 0.5f);
		}
		//Debug.Log("("+x+","+ y+ ") "  +" Level: " + level);
	}

	// 0: top, 1: right, 2: left, 3: button, 4: error(shouldn't happen)
	public void decreaseByOneLevel(int area){
		
		// update coodinates x,y (depending on which side they are)
		switch(area){
			case 0:
				y -= 1;
				break;
			case 1:
				x -= 1;
				break;
			case 2:
				x += 1;
				break;
			case 3:
				y += 1;
				break;
		}

		// set BlockObj down 10 world units(depending on which side they are) 
		// find out the area
		Vector3 position = blockObject.transform.position;

		if(Mathf.Abs(position.x) > Mathf.Abs(position.y)){
			if(position.x > 0){
				// right
				position.x -= 10;
			}else{
			 	// left
				position.x += 10;
			}
		}else{
			if(position.y > 0){
				// top
				position.y -= 10;
			}else{
				// bottom
				position.y += 10;
			}
		}
		blockObject.transform.position = position;

		// decrease Level
		level--;
	}


	public void setBlockObject(GameObject aObject){
		blockObject = aObject;
	}

	public void clearBlockObject(){
		blockObject = null;
	}

	public GameObject getBlockObject(){
		return blockObject;
	}

	public int getLevel(){
		return level;
	}




}
