using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public TetrisManager tetrisManager; 
    public Piece piecePrefab;
    public Tilemap tilemap;

    public TetronimoData[] tetronimos;
    public Vector2Int boardSize;
    public Vector2Int startPosition;

    public float dropInterval = 0.5f; 

    Piece activePiece;

    int left
    {
        get { return -boardSize.x / 2; }
    }
    int right
    {
        get { return boardSize.x / 2; }
    }
    int top
    {
        get { return boardSize.y / 2; }
    }
    int bottom
    {
        get { return -boardSize.y / 2; }
    }

    float time = 0.0f; 

    private void Start()
    {
        SpawnPiece();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= dropInterval)
        {
            time = 0.0f;

            Clear(activePiece);
            bool moveResult = activePiece.Move(Vector2Int.down);
            Set(activePiece);

            if (!moveResult)
            {
                activePiece.freeze = true;
                CheckBoard();
                SpawnPiece(); 
            }
        }
    }

    public void SpawnPiece()
    {
        activePiece = Instantiate(piecePrefab);

        Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);

        activePiece.Initialize(this, t);

        CheckEndGame();

        Set(activePiece);
    }

    void CheckEndGame()
    {
        if (!IsPositionValid(activePiece, activePiece.position))
        {
            tetrisManager.SetGameOver(true); 
        }
    }

    public void UpdateGameOver()
    {
        if (!tetrisManager.gameOver) ResetBoard();
    }

    public void ResetBoard()
    {
        Piece[] foundPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);

        foreach (Piece piece in foundPieces) Destroy(piece.gameObject);

        activePiece = null;

        tilemap.ClearAllTiles();

        SpawnPiece(); 
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, null);
        }
    }

    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);

            if (cellPosition.x < left || cellPosition.x >= right ||
                cellPosition.y < bottom || cellPosition.y >= top) return false;

            if (tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow + 1; y < top; y++)
        {
            for (int x = left ; x < right; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);

                TileBase currentTile = tilemap.GetTile(cellPosition);

                tilemap.SetTile(cellPosition, null);

                cellPosition.y -= 1;
                tilemap.SetTile(cellPosition, currentTile); 
            }
        }
    }

    public void CheckBoard()
    {
        List<int> destoryedLines = new List<int>();
        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destoryedLines.Add(y);
            }

        }

        int rowsShiftedDown = 0;

        foreach (int y in destoryedLines)
        {
            ShiftRowsDown(y - rowsShiftedDown);
            rowsShiftedDown++;
        }

        int score = tetrisManager.CalculateScore(destoryedLines.Count);

        tetrisManager.ChangeScore(score);


    }

    bool IsLineFull(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);

            if (!tilemap.HasTile(cellPosition)) return false;
        }

        return true;
    }

    void DestroyLine(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);

            tilemap.SetTile(cellPosition, null); 
        }
    }
}
