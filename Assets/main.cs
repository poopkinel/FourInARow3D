using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour
{
	
	char [,] board = new char [7, 7];
	char player1 = 'O';
	char player2 = 'X';
	char currentPlayer;
	
	public char winner;
	bool isBoardFull;
	
	public Camera camera;
	GameObject cellToPlay;
	
	public int insideWhileLoop;
	
	private bool CheckEmptyCell(int row, int col){
		
		if (board[row, col] != player1 && board[row, col] != player2) 
			return true; 
		else 
			return false;
	}
	
	private bool CheckBoardFull(){
		for (int i=0; i<7; i++) {
			for (int j=0; j<7; j++) {
				if (!CheckEmptyCell(i, j))
					return false;
			}
		}
		return true;
	}
	
	private char CheckWinner() {
		
		char[] players = {player1, player2};
		
		for (int p=0; p < players.Length; p++) { // iter over players
			int consRowCount = 0;
			for (int i=0; i<7; i++){ // iter over rows
				for (int j=0; j<7; j++) { // iter over cells (cols in each row)
					// add to row count or reset count
					if (board [i, j] == players[p]) { consRowCount ++; }
					else { consRowCount = 0; }
					// check if 4 in row horizontal
					if (consRowCount == 4) 
						return players[p];
					
					// check 4 in a row vertical
					int consColCount = 0;
					if (board[i, j] == players[p])
						consColCount ++ ;
					
					if (i + 3 < 7) {  // if vertical win possible
						for (int k=1; k<4; k++) { 
							if (board[i+k, j] == players[p]) { 
								consColCount ++;  
							} 
						} 
						if (consColCount >= 4) {
							return players[p];
						}						
						else 
							consColCount = 0;
					}
					
					
				
					// check diagonal wins
					
					//int conDiagCount = 0;
					if (i + 3 < 7 && j + 3 < 7){
						if (board[i, j] == players[p] &&
							board[i+1, j+1] == players[p] && 
							board[i+2, j+2] == players[p] && 
							board[i+3, j+3] == players[p]) 
							return players[p];
					}
					if (i + 3 < 7 && j - 3 > 0) {
						if (board[i, j] == players[p] &&
							board[i+1, j-1] == players[p] && 
							board[i+2, j-2] == players[p] && 
							board[i+3, j-3] == players[p]) 
						   return players[p];
					}
				}
			}
		}
		return '\0'; // no winner
	}
	
	private char NextPlayer() {
		if (currentPlayer == player1) 
			return player2; 
		else 
			return player1;
	}
	
	
    // Start is called before the first frame update
    void Start()
    {
        winner = '\0';
		isBoardFull = false;
		currentPlayer = player1;
    }

	private bool CheckValidInput(int row, int col) {
		if (col < 0 || col > 6 || row < 0 || row > 6) {
			return false;
		}
		if (!CheckEmptyCell(row, col)) {
			return false;
		}
		return true;
	}
	
	private void DoTurn (int row, int col) {
		cellToPlay = GameObject.Find("Cell " + row + " " + col );
		GameObject mesh;
		if (currentPlayer == player1) {
			mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
			var meshRenderer = mesh.GetComponent<Renderer>();
			meshRenderer.material.SetColor("_Color", Color.red);
		}
		else {
			mesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			var meshRenderer = mesh.GetComponent<Renderer>();
			meshRenderer.material.SetColor("_Color", Color.blue);
		}
		
		mesh.transform.position = cellToPlay.transform.position;
		mesh.transform.SetParent(cellToPlay.transform);	
	}
	
	private void PlacePiece(int row, int col) {
		int i = row;
		while (i >= 0) {
			if (i == 0 && CheckEmptyCell(i, col)){
				board[i, col] = currentPlayer;
				DoTurn(i, col);
			}
			else if (CheckEmptyCell(i, col) && !CheckEmptyCell(i-1, col)) {				
				board[i, col] = currentPlayer;
				DoTurn(i, col);
			}
			i -- ;
		}		
	}

    // Update is called once per frame
    void Update() {	
		
		if (winner == '\0' && !isBoardFull) {
			if (Input.GetMouseButtonDown(0)) { // if left button pressed...
				//Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);

				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)){
					string[] coordinates = hit.collider.gameObject.name.Split(' ');
					int row = int.Parse(coordinates[1]);
					int col = int.Parse(coordinates[2]);
					
					if (CheckValidInput(row, col)) {
						PlacePiece(row, col);
						currentPlayer = NextPlayer();
					}	
					winner = CheckWinner();
					isBoardFull = CheckBoardFull();
				}
			}
		}
		else {
			if (winner != '\0') {
					Text textObject = GameObject.Find("Text").GetComponent<Text>();
					textObject.text = "Winner: " + (currentPlayer == 'X' ? "Player1".ToString() : "Player2".ToString());
				}
			else if (isBoardFull) {
					Text textObject = GameObject.Find("Text").GetComponent<Text>();
					textObject.text = "Tie";
			}
		}
	}
}