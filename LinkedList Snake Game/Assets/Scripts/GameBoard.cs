using System.Collections.Generic;
using ADT;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class GameBoard : MonoBehaviour
{
    public delegate void OnObtainScore(int score);
    public OnObtainScore onObtainScore;
    
    public delegate void OnDeath();
    public OnDeath onDeath;
    
    public delegate void OnWin();
    public OnWin onWin;

    public Dictionary<Vector3, GameObject> food;

    public LList<GameObject> snakeBody;
    [HideInInspector] public GameObject snakeHead;

    [SerializeField] private GameObject snakeBodyPrefab;
    [SerializeField] private GameObject foodPrefab;

    [SerializeField] private int foodScore;
    
    public Vector2Int gridSize;

    public enum BoardPieces
    {
        Empty, Snake, Food
    }
    
    public BoardPieces[,] boardPieces;
    
    private void Awake()
    {   
        food = new Dictionary<Vector3, GameObject>();
        
        snakeBody = new LList<GameObject>();
        snakeBody.Add(GameObject.Find("SnakeHead"));
        snakeHead = snakeBody.headNode.value;

        snakeHead.GetComponent<Movement>().onMoveCurrent += SnakeHeadHit;
        
        boardPieces = new BoardPieces[gridSize.x, gridSize.y];
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                boardPieces[x, y] = BoardPieces.Empty;
            }
        }
        
        InstantiateFoodRnd(7);
    }

    public string CheckPieceOnPos(int x, int y)
    {
        if (x < 0 || x > gridSize.x - 1 || y < 0 || y > gridSize.y - 1)
        {
            return "IsDeath";
        }
        
        if (boardPieces[x, y] == BoardPieces.Food)
        {
            return "Food";
        }
        else if(boardPieces[x, y] == BoardPieces.Snake)
        {
            return "IsDeath";
        }

        return "";
    }

    public void AddSnakeBody(Vector3 pos)
    {
        snakeBody.Add(Instantiate(snakeBodyPrefab, pos, Quaternion.identity));
    }

    public void Death()
    {
        Time.timeScale = 0;
        onDeath.Invoke();
    }
    
    private void SnakeHeadHit(Vector3Int pos)
    {
        switch(CheckPieceOnPos(pos.x, pos.y))
        {
            case "IsDeath":
                Death();
                break;
            case "Food":
                AddSnakeBody(snakeBody.tailNode.value.transform.position);
                DestroyFoodPiece(pos);
                break;
        }
    }
    
    private void DestroyFoodPiece(Vector3Int pos)
    {
        Destroy(food[pos]);
        food.Remove(pos);
        
        boardPieces[pos.x, pos.y] = BoardPieces.Empty;
        
        onObtainScore.Invoke(foodScore);

        if (food.Count == 0)
        {
            Time.timeScale = 0;       
            onWin.Invoke();
        }
    }
    
    private void InstantiateFoodRnd(int amount)
    {
        Random rdm = new Random();
        
        for (int i = 0; i < amount; i++)
        {
            boardPieces[rdm.Next(0, gridSize.x), rdm.Next(0, gridSize.y)] = BoardPieces.Food;
        }
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (boardPieces[x, y] == BoardPieces.Food)
                {
                    GameObject go = Instantiate(foodPrefab, new Vector3(x, y, 0), quaternion.identity);
                    food.Add(go.transform.position, go);
                }
            }
        }
    }

    private void OnSnakeDestroy()
    {
        if(snakeHead == null)
            return;
        
        snakeHead.GetComponent<Movement>().onMoveCurrent -= SnakeHeadHit;
    }
}
