using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {
	public GameObject wall;
	public bool test = false;

	private bool[,] board; 
	private PieceSpawner spawner;
	private Board playBoard;
	private PiecesManager piecesManager;

	// Use this for initialization
	void Start () {
		spawner = FindObjectOfType<PieceSpawner>();
		playBoard = FindObjectOfType<Board>();
		piecesManager = FindObjectOfType<PiecesManager>();

		board = new bool[40, 40];
		board[20, 20] = true;
		board[19, 20] = true;
		board[20, 19] = true;
		board[19, 19] = true;
		for(int i = 0; i<=9; i++){
			for(int j= 0; j <=9; j++){
				board[j,30+i] = true;
				board[j,i] = true;
				board[39-j,30+i] = true;
				board[39-j,i] = true;
			}
		}
	}

	void Update(){
		if(test){
			testBoardArray();
			test = false;
		}
	}

	public bool isNextStepPossible(Vector2 newPosition, Vector2[] pieceOutline){
		//print("X: " + newPosition.x + " Y: " + newPosition.y + " free?");
		// out of bounds check
		Vector2 normalizedNewPosition = newPosition + new Vector2(20f,20f);

		if(!isInBounds(normalizedNewPosition,0, 39)){
			return false;
		}

		// check if parts of the piece are overlapping with other pieces oder borders
		for(int i = 0; i< pieceOutline.Length; i++){
			Vector2 outlinePart = pieceOutline[i];
			Vector2 piecePartPosition = new Vector2(normalizedNewPosition.x + outlinePart.x, normalizedNewPosition.y + outlinePart.y); 

			if(!isInBounds(piecePartPosition, 0, 39)){
				return false;
			}

			if(board[(int)(piecePartPosition.x), (int)(piecePartPosition.y)] == true){
				return false;
			}
		}

		return true;
	} 

	private bool isInBounds(Vector2 vector, int boundLow, int boundHigh){
		if(vector.x < boundLow || vector.y < boundLow || vector.x > boundHigh || vector.y > boundHigh){
			return false;
		}else{
			return true;
		}
	}

	public void setPieceToPlace(Vector2 position, Piece piece){
		Vector2 normalizedPosition = position + new Vector2(20f,20f);

		Vector2[] outline = piece.getPieceOutline();
		for(int i = 0; i< outline.Length; i++){
			Vector2 outlinePart = outline[i];
			board[(int)(normalizedPosition.x + outlinePart.x), (int)(normalizedPosition.y + outlinePart.y)] = true;
		}

		checkForTetris();

		playBoard.rotate();
		Invoke("spawnNextPiece", 1.1f);
		rotateTheBoard();
	} 

	private void spawnNextPiece(){
		spawner.spawnOnSameSpot();
	}

	private void rotateTheBoard(){
		bool [,] newBoard = new bool[40,40];
		for(int i = -20; i < 20; i++){
			for(int j = -20; j < 20; j++){
				if(board[i+20,j+20] == true){
					// matrix multiplication to rotate coordinates for -90°
					//	0    1       i
					//  -1   0	x    j
					// but we need to change the origin to (-0.5,-0.5) i.e. add 0.5 to every coordinate
					// then rotate by matrix multiplication and finally change the origin back i.e. substract 0.5 from every coodinate
					float newX = i + 0.5f;
					float newY = j + 0.5f;

					float rotatedX = newY;
					float rotatedY = -newX;

					float newI = rotatedX - 0.5f;
					float newJ = rotatedY - 0.5f;

					newBoard[(int) newI+20, (int) newJ+20] = true;
				}
			}
		}
		board = newBoard;
	}

	private void checkForTetris(){
		// "radius" of square
		int numberOfTetrises = 0;
		int[] tempLevelOfTetrises = new int[18]; 
		for(int i = 2; i<= 20; i++){
			if(checkForTetrisOnLevel(i)){
				tempLevelOfTetrises[numberOfTetrises++] = i;
			}

		}

		int[] levelOfTetrises = new int[numberOfTetrises]; 
		Array.Copy(tempLevelOfTetrises, levelOfTetrises, numberOfTetrises);

		// refresh board array, swap bools down
		foreach(int level in levelOfTetrises){
			for(int i = -level; i <= level-2; i++){
				for(int j = level; j <= 19; j++){
					// top
					setBoardAt(i, j-1, getBoardAt(i,j));
					// bot
					setBoardAt(i+1, -j, getBoardAt(i+1,-j-1));
					// right
					setBoardAt(j-1, i+1, getBoardAt(j,i+1));
					// left
					setBoardAt(-j, i, getBoardAt(-j-1,i));
				}

				setBoardAt(i, 19, false);
				setBoardAt(i, -20, false);
				setBoardAt(19, i, false);
				setBoardAt(-20, i, false);
			}
		}

		piecesManager.deleteLevels(levelOfTetrises);

	}



	// level between 2 and 20
	private bool checkForTetrisOnLevel(int level){
		bool tetris = true; 
		// coordinate system is altered in the board-array
		// board array from coordinate 0 to 39 -> origin in (20,20): -20 to 19
		// level 20: every side side has 2*level blocks, j between -level+1 and level (0 is a block too)
		for(int j = -level; j<=level-1 & tetris; j++){
			// AND: tetris is true only if all booleans are true i.e. there is a block on every place of the square
			tetris = tetris & board[level-1 +20, j + 20] & board[-level +20, j + 20] & board[j + 20, level-1 +20] & board[j + 20, -level +20];

		}
		return tetris;
	}

	private void setBoardAt(int x, int y, bool state){
		board[x+20, y+20] = state;
	}
	private bool getBoardAt(int x, int y){
		return board[x+20, y+20];
	}


	/*                 TEST METHODS                                     */
	// TODO: delete
	private void buildWallAtLevel(int level){
		for(int j = -level; j<=level-1; j++){
			GameObject.Instantiate(wall, new Vector3((level-1)*10+5,(j)*10+5,-5), Quaternion.identity);
			GameObject.Instantiate(wall, new Vector3((-level)*10+5,(j)*10+5,-5), Quaternion.identity);
			GameObject.Instantiate(wall, new Vector3((j)*10+5,(level-1)*10+5,-5), Quaternion.identity);
			GameObject.Instantiate(wall, new Vector3((j)*10+5,(-level)*10+5,-5), Quaternion.identity);
		}
	}

	private void testBoardArray(){
		// test board setup

		for(int i= 0; i<40; i++){
			for(int j= 0; j<40; j++){
				if(board[i,j] == true){
					GameObject.Instantiate(wall, new Vector3((i-20)*10+5,(j-20)*10+5,-5), Quaternion.identity);
				}
			}
		}

	}

	//DELETE
	/*
	 private string abc(int[] array){
		List<string> list = new List<string>();
		foreach (int listitem in array)
		{
    		list.Add("" + listitem);
		}
		return string.Join(",", list.ToArray());
	 }
	 */

}
