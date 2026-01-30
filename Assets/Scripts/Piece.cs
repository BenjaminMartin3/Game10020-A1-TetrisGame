using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TetronimoData data;
    public Vector2Int[] cells;
    public Vector2Int position;
    public Board board;

    public bool freeze = false;

    public void Initialize(Board board, Tetronimo tetronimo)
    {
        this.board = board;
        for (int i = 0; i < board.tetronimos.Length; i++)
        {
            if (board.tetronimos[i].tetronimo == tetronimo)
            {
                this.data = board.tetronimos[i];
                break;
            }
        }

        cells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < data.cells.Length; i++) cells[i] = data.cells[i];

        position = board.startPosition;
    }

    private void Update()
    {
        if (board.tetrisManager.gameOver) return; 
        if (freeze) return;

        board.Clear(this);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rotate(1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rotate(-1);
        }

        board.Set(this);

        // debug
        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CheckBoard();
        }

        if (freeze)
        {
            board.CheckBoard();
            board.SpawnPiece();
        }
    }

    void Rotate(int direction)
    {
        Vector2Int[] temporaryCells = new Vector2Int[cells.Length]; 

        for (int i = 0; i < cells.Length; i++) temporaryCells[i] = cells[i];

        ApplyRotation(direction);

        if (!board.IsPositionValid(this, position))
        {
            if (!TryWallKicks())
            {
                RevertRotation(temporaryCells);
            }
            else
            {
                Debug.Log("Wall Kick Successful");
            }
        }
        else
        {
            Debug.Log("Valid Rotation");
        }
    }

    bool TryWallKicks()
    {
        Vector2Int[] wallKickOffsets = new Vector2Int[]
        {
            Vector2Int.left,
            Vector2Int.down,
            Vector2Int.right,
            new Vector2Int(-1, -1), 
            new Vector2Int(1, -1), 
        }; 

        foreach (Vector2Int offset in wallKickOffsets)
        {
            if (Move(offset)) return true; 
        }

        return false;
    }

    void RevertRotation(Vector2Int[] temporaryCells)
    {
        for (int i = 0; i < cells.Length; i++) cells[i] = temporaryCells[i];
    }

    void ApplyRotation(int direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90.0f * direction);

        bool isSpecial = data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.O;
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cellPosition = new Vector3(cells[i].x, cells[i].y);

            if (isSpecial)
            {
                cellPosition.x -= 0.5f;
                cellPosition.y -= 0.5f; 
            }

            Vector3 result = rotation * cellPosition;

            if (isSpecial)
            {
                cells[i].x = Mathf.CeilToInt(result.x);
                cells[i].y = Mathf.CeilToInt(result.y);
            }
            else
            {
                cells[i] = new Vector2Int(
                    Mathf.RoundToInt(result.x),
                    Mathf.RoundToInt(result.y));
            }
        }
    }

    void HardDrop()
    {
        while (Move(Vector2Int.down)) ;

        freeze = true;
    }

    public bool Move(Vector2Int translation)
    {
        Vector2Int newPosition = position;
        newPosition += translation;


        bool isValid = board.IsPositionValid(this, newPosition);
        if (isValid) position = newPosition;

        return isValid;
    }
}
