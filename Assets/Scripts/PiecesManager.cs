using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PiecesManager : MonoBehaviour {

	private List<Piece> piecesList;
	private List<Block> blocksList;
	private List<Block>[] blockOnLevels;
	private LevelFramesManager levelFramesManager;


	// Use this for initialization
	void Start () {
		levelFramesManager = FindObjectOfType<LevelFramesManager>();

		piecesList = new List<Piece>();
		blocksList = new List<Block>();
		blockOnLevels = new List<Block>[20];
		for(int i = 0; i < blockOnLevels.Length; i++){
			blockOnLevels[i] = new List<Block>();
		}
	}


	public void registerPiece(Piece piece){
		piecesList.Add(piece);
	}

	public void registerBlocks(GameObject[] blocks){
		foreach(GameObject block in blocks){
			Vector3 position = block.transform.position;
			int posX = ( ((Mathf.RoundToInt(position.x)) - 5)) / 10;
			int posY = ( ((Mathf.RoundToInt(position.y)) - 5)) / 10;
			Block blockObj = new Block(posX, posY, block);
			blockOnLevels[blockObj.getLevel() - 1].Add(blockObj);
			//Debug.Log("level " + blockObj.getLevel() + " #: " + blockOnLevels[blockObj.getLevel() - 1].Count);

			levelFramesManager.activateLevelFrame(blockObj.getLevel());
		}
	}

	public void deleteLevels(int[] levels){
		foreach(int level in levels){
			Debug.Log("Delete level: " + level + " with elements " + blockOnLevels[level - 1].Count);
			foreach(Block block in blockOnLevels[level - 1]){
				Debug.Log(block.getBlockObject());
				GameObject.Destroy(block.getBlockObject());
			}
			blockOnLevels[level - 1].Clear();
		}

		for(int i = 0; i < levels.Length; i++){
			// new idea iterate over deleted levels and check which blocks need to be pulled down

			int level = levels[i];
			// get corners (screwed up coordinate system, found them out by trial and error)
			/*
			Vector2 topLeft = new Vector2(-level, level-1);
			Vector2 topRight = new Vector2(level-1, level-1);
			Vector2 bottomLeft = new Vector2(-level, -level);
			Vector2 bottomRight = new Vector2(level-1, -level);
			*/

			for(int j = level+1; j<= 20; j++){
				for(int k = 0; k < blockOnLevels[j-1].Count; k++){
					Block block = blockOnLevels[j-1][k];
					// find out if block is above the level
					// every area has own "line" that is not pulled down, because next area uses that place
					bool inTopArea = inBetween(block.x, -level, level-2) && inBetween(block.y, level-1, 19);
					bool inRightArea = inBetween(block.x, level-1, 19) && inBetween(block.y, -level+1, level-1);
					bool inLeftArea = inBetween(block.x, -20, -level) && inBetween(block.y, -level, level-2);
					bool inBottomArea = inBetween(block.x, -level+1, level-1) && inBetween(block.y, -20, -level);

					if(inTopArea || inRightArea || inLeftArea || inBottomArea){
						Debug.Log("("+block.x + "," + block.y + ") is " + (inTopArea ? "top area": (inRightArea? "right area": (inLeftArea? "left area": (inBottomArea? "bottom area": ""))) ) );
						blockOnLevels[block.getLevel() - 1].Remove(block);
						block.decreaseByOneLevel(inTopArea ? 0 : (inRightArea? 1 : (inLeftArea? 2: (inBottomArea? 3: 4))));
						//block.updateLevelAfterDeletingLevels(levels);
						blockOnLevels[block.getLevel() - 1].Add(block);
						k--;
					}
				}
			}

		}

		for(int i = 0; i< blockOnLevels.Length; i++){
			List<Block> blocksList = blockOnLevels[i];
			if(blocksList.Count > 0){
				levelFramesManager.activateLevelFrame(i+1);
			}else{
				levelFramesManager.deactivateLevelFrame(i+1);
			}
		}

	}

	// returns true if x element of [a;b]
	private bool inBetween(int x, int a, int b){
		if(x >= a && x <= b){
			return true;
		}else{
			return false;
		}
	}

	/* Solved in a different way without floats and rounding errors after rotating

	public void deleteBlocksOnSquare(int squareSide){
		
			---------
			|       |
			|   *   |
			|       |
			--------
			<-->
			squareSide = half of the length of a side

		
		float squareSideInWC = (squareSide-1)*10 + 5;

		for(int i = 0; i< blocksList.Count; i++){
			GameObject block = blocksList[i];
			float x = Mathf.Round(block.transform.position.x);
			float y = Mathf.Round(block.transform.position.y);

			// Mathf.Approximately to account for rounding errors from the rotation
			bool onRightSide = Mathf.Approximately(x, squareSideInWC) && (y <= (squareSideInWC + .5) && y >= -(squareSideInWC + .5));
			bool onLeftSide = Mathf.Approximately(x, -squareSideInWC) && (y <= (squareSideInWC + .5) && y >= -(squareSideInWC + .5));
			bool onTopSide = Mathf.Approximately(y, squareSideInWC) && (x <= (squareSideInWC + .5) && x >= -(squareSideInWC + .5));
			bool onBottomSide = Mathf.Approximately(y, -squareSideInWC) && (x <= (squareSideInWC + .5) && x >= -(squareSideInWC + .5));

			if(onRightSide || onLeftSide || onTopSide || onBottomSide){
				blocksList.RemoveAt(i--);
				GameObject.Destroy(block);
			}
		}
		//TODO: move blocks nearer to center to fill in destroyed blocks
	}
	*/

}
