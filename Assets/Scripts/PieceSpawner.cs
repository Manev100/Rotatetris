using UnityEngine;
using System.Collections;

public class PieceSpawner : MonoBehaviour {

	public GameObject[] spawnPoints;
	public GameObject[] pieces;

	private int nextPosition = 0;

	// Use this for initialization
	void Start () {
		spawnNext();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void spawnOnSameSpot(){
		nextPosition = (nextPosition - 1) % spawnPoints.Length;
		spawnNext();
	}

	public void spawnNext(){

		int randomPieceIndex = (int)Random.Range(0, pieces.Length);
		
		GameObject pieceToCreate = pieces[randomPieceIndex];
		Vector2 positionOffset = pieceToCreate.GetComponent<Piece>().getStartPositionOffet();

		Vector3 positionOfPiece = spawnPoints[nextPosition].transform.position;
		positionOfPiece.x += positionOffset.x;
		positionOfPiece.y += positionOffset.y;

		GameObject.Instantiate(pieceToCreate, positionOfPiece, Quaternion.identity);

		nextPosition = (nextPosition + 1) % spawnPoints.Length;
	}
}
