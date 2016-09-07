using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {
	private float moveRate = 1f;  // how many squares per second the piece is moving
	public float standardMoveRate = 1f;
	public float fastMoveRate = 0.1f;
	public Vector2 startPositionOffset = new Vector2();
	public Vector2[] pieceOutline;
	public bool disableRotation = false;

	private bool falling = true;
	private float timeSinceLastMoved = 0f;
	private Vector3 positionalChange = new Vector3(0f, -10f, 0f);
	private GameManager gameManager;
	private PiecesManager piecesManager;

	// Use this for initialization
	void Start () {
		gameManager = FindObjectOfType<GameManager>();
		piecesManager = FindObjectOfType<PiecesManager>();
		GameObject parent = GameObject.Find("Pieces");

		if(parent){
			transform.SetParent(parent.transform);
		}

		piecesManager.registerPiece(this);

	}
	
	// Update is called once per frame
	void Update () {
		if(falling){
			timeSinceLastMoved += Time.deltaTime;
			if(timeSinceLastMoved >= moveRate){

				Vector3 currentPosition = gameObject.transform.position;
				Vector3 newPosition = currentPosition + positionalChange;
				Vector2 newBoardPosition = new Vector2();
				newBoardPosition.x = getBoardValueFromWorldValue(newPosition.x);
				newBoardPosition.y = getBoardValueFromWorldValue(newPosition.y);

				if(gameManager.isNextStepPossible(newBoardPosition, pieceOutline)){
					gameObject.transform.position = newPosition;
					timeSinceLastMoved = 0f;
				}else{
					falling = false;

					Vector2 boardPosition = new Vector2();
					boardPosition.x = getBoardValueFromWorldValue(currentPosition.x);
					boardPosition.y = getBoardValueFromWorldValue(currentPosition.y);
					piecesManager.registerBlocks(getBlocks());
					gameManager.setPieceToPlace(boardPosition, this);

				}
			}

			if(Input.GetKey(KeyCode.W)){

			}
			if(Input.GetKeyDown(KeyCode.A)){
				moveInDirection(new Vector3(-10f,0,0));
			}
			if(Input.GetKeyDown(KeyCode.D)){
				moveInDirection(new Vector3(10f,0,0));
			}
			if(Input.GetKeyDown(KeyCode.S)){
				moveRate = fastMoveRate;
			}
			if(Input.GetKeyUp(KeyCode.S)){
				moveRate = standardMoveRate;
			}
			if(Input.GetKeyDown(KeyCode.Space)){
				if(!disableRotation){
					rotate();
				}
			}
		}

	}

	private bool moveInDirection(Vector3 direction){
		Vector3 currentPosition = gameObject.transform.position;
		Vector3 newPosition = currentPosition + direction;
		Vector2 boardPosition = new Vector2();
		boardPosition.x = getBoardValueFromWorldValue(newPosition.x);
		boardPosition.y = getBoardValueFromWorldValue(newPosition.y);

		if(gameManager.isNextStepPossible(boardPosition, pieceOutline)){
			gameObject.transform.position = newPosition;
			return true;
		}else{
			return false;
		}
	}

	// Translate from world coordinates to board coordinates
	private int getBoardValueFromWorldValue(float value){
		if(value < 0){
			return  (int)((value - 10) / 10);
		}else{
			return  (int)(value / 10);
		}
	}

	private bool rotate(){
		// matrix multiplication to rotate coordinates for 90°
		//	0  -1        x
		//  1   0	x    y
		//
		Vector2[] oldOutline = new Vector2[pieceOutline.Length];
		for(int i = 0; i<pieceOutline.Length; i++){
			oldOutline[i] = pieceOutline[i];

			float x = pieceOutline[i].x;
			float y = pieceOutline[i].y;

			Vector2 newOutlinePoint = new Vector2();
			newOutlinePoint.x = -y;
			newOutlinePoint.y = x; 

			pieceOutline[i] = newOutlinePoint;
		}
		// check if piece is not overlapping with another after rotating
		Vector3 currentPosition = gameObject.transform.position;
		Vector2 boardPosition = new Vector2();
		boardPosition.x = getBoardValueFromWorldValue(currentPosition.x);
		boardPosition.y = getBoardValueFromWorldValue(currentPosition.y);

		bool canBeRotated = false;
		if(gameManager.isNextStepPossible(boardPosition, pieceOutline)){
			canBeRotated= true;
		}else{
			// piece cannot be rotated directly, try moving the piece first
			if(moveInDirection(new Vector3(10f,0,0)) ){
				canBeRotated = true;
			}else if(moveInDirection(new Vector3(-10f,0,0))){
				canBeRotated = true;
			}else if(moveInDirection(new Vector3(0,10f,0))){
				canBeRotated = true;
			}
		}

		// rotate if rotation is possible or rollback
		if(canBeRotated){
			gameObject.transform.Rotate(new Vector3(0,0,90));
		}else{
			pieceOutline = oldOutline;
		}
		return canBeRotated;
	}

	/*
	public bool deleteBlockAtOffset(int offsetX, int offsetY){
		// when we destroy a block pieceOutline needs to be refreshed
		Vector2[] newOutline = new Vector2[pieceOutline.Length - 1];
		int newOutlineIndex = 0;

		Debug.Log("Deleting black at " + offsetX + ", " + offsetY);
		bool blockDeleted = false;
		foreach(Vector2 blockCoordinate in pieceOutline){
			// is there a subblock at this position?
			if((int)blockCoordinate.x == offsetX && (int)blockCoordinate.y == offsetY){
				Debug.Log("Found block");
				// get subblock and delete it
				foreach(Transform block in transform){
					Debug.Log("Child: " + block.position);
					// Issue: child transforms have absolute position in the world and not relative to parent
					if((int)block.position.x == offsetX*10 && (int)block.position.y == offsetY*10){
						GameObject.Destroy(block.gameObject);
						blockDeleted = true;
					}
				}
			}else{
				// the current blockCoodinate is not the one we are looking for so we save it for the refreshed pieceOutline
				// Since pieceOutline stays the same when no block is deleted we need to prevent outOfBounds error
				if(newOutlineIndex < pieceOutline.Length - 1){
					newOutline[newOutlineIndex++] = blockCoordinate;
				}
			}
		}

		if(blockDeleted){
			pieceOutline = newOutline;
		}

		return blockDeleted;
	}
	*/

	public Vector2 getStartPositionOffet(){
		return startPositionOffset;
	}

	public Vector2[] getPieceOutline(){
		return pieceOutline;
	}

	public GameObject[] getBlocks(){
		GameObject[] blocks = new GameObject[transform.childCount];
		for(int i = 0; i < transform.childCount; i++){
			blocks[i] = transform.GetChild(i).gameObject;
		}
		return blocks;
	}

}
