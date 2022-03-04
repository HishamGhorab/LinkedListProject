using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public delegate void OnMovePrevious();
    public OnMovePrevious onMovePrevious;
    
    public delegate void OnMoveCurrent(Vector3Int pos);
    public OnMoveCurrent onMoveCurrent;

    [SerializeField] private float moveTimerValue = 0.5f;
    private float moveTimer = 0;
    
    private GameBoard gameBoard;

    private List<Vector3> foodPositions;
    private Vector3 directionVector;
    private Quaternion directionRotation;

    private void Start()
    {
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();

        foodPositions = gameBoard.food.Keys.ToList();
        
        moveTimer = moveTimerValue;
        directionVector = transform.up;
    }
    
    private void Update()
    {
        Move();
    }
    
    string GetPlayerInput()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return "Left";
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            return "Right";
        }
        else if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            return "Up";
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            return "Down";
        }

        return " ";
    }
    
    void SetDirection(string input)
    {
        switch(input)
        {
            case "Left":             
                directionVector = -transform.right;
                directionRotation = Quaternion.Euler(0, 0, 90);
                break;
            case "Right":             
                directionVector = transform.right;
                directionRotation = Quaternion.Euler(0, 0, -90);
                break;
            case "Up":             
                directionVector = transform.up;
                directionRotation = Quaternion.Euler(0, 0, 0);
                break;
            case "Down":             
                directionVector = -transform.up;
                directionRotation = Quaternion.Euler(0, 0, 180);
                break;
        }
    }
    
    private void Move()
    {
        SetDirection(GetPlayerInput());
        
        if(moveTimer <= 0)
        {
            onMovePrevious.Invoke();
            gameObject.transform.GetChild(0).transform.rotation = directionRotation;
            transform.position += directionVector;
            onMoveCurrent.Invoke(Vector3Int.FloorToInt(transform.position));
            
            moveTimer = moveTimerValue;
        }

        moveTimer -= Time.deltaTime;
    }
}
