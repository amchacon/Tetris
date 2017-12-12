using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

    // reference to our board
    Board m_gameBoard;

    // reference to our spawner 
    Spawner m_spawner;

    // reference to our scoreManager
    ScoreManager m_scoreManager;

    // currently active shape
    Shape m_activeShape;

    public float m_dropInterval = 0.1f;

    // drop interval modified by level
    float m_dropIntervalModded;

    float m_timeToDrop;

    float m_timeToNextKeyLeftRight;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = 0.25f;

    float m_timeToNextKeyDown;

    [Range(0.01f, 0.5f)]
    public float m_keyRepeatRateDown = 0.01f;

    float m_timeToNextKeyRotate;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateRotate = 0.25f;

    public GameObject m_gameOverPanel;

    bool m_gameOver = false;


    // Use this for initialization
    void Start()
    {
        // find spawner and board with generic version of GameObject.FindObjectOfType, slower but less typing
        m_gameBoard = GameObject.FindObjectOfType<Board>();
        m_spawner = GameObject.FindObjectOfType<Spawner>();
        m_scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (!m_gameBoard)
        {
            Debug.LogWarning("WARNING!  There is no game board defined!");
        }

        if (!m_scoreManager)
        {
            Debug.LogWarning("WARNING!  There is no score manager defined!");
        }

        if (!m_spawner)
        {
            Debug.LogWarning("WARNING!  There is no spawner defined!");
        }
        else
        {
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);

            if (!m_activeShape)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
        }

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }

        m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level * 0.1f), 0.05f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // if we are missing a spawner or game board or active shape, then we don't do anything
        if (!m_spawner || !m_gameBoard || !m_activeShape || m_gameOver || !m_scoreManager)
        {
            return;
        }
        PlayerInput();
    }

    void PlayerInput()
    {
        if (Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveRight"))
        {
            m_activeShape.MoveRight();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
            }

        }
        else if (Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft"))
        {
            m_activeShape.MoveLeft();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
            }

        }
        else if (Input.GetButtonDown("Rotate") && (Time.time > m_timeToNextKeyRotate))
        {
            m_activeShape.RotateRight();
            m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateLeft();
            }

        }
        else if (Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown) || (Time.time > m_timeToDrop))
        {
            m_timeToDrop = Time.time + m_dropInterval;
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;

            m_activeShape.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if (m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }

        }
    }

    // shape lands
    void LandShape()
    {
        // move the shape up, store it in the Board's grid array
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);

        // spawn a new shape
        m_activeShape = m_spawner.SpawnShape();

        // set all of the timeToNextKey variables to current time, so no input delay for the next spawned shape
        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;

        // remove completed rows from the board if we have any 
        m_gameBoard.ClearAllRows();

        if (m_gameBoard.m_completedRows > 0)
        {
            m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

            if (m_scoreManager.didLevelUp)
            {
                m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level * 0.05f), 0.05f, 1f);
            }
        }
    }

    // triggered when we are over the board's limit
    void GameOver()
    {
        // move the shape one row up
        m_activeShape.MoveUp();

        // turn on the Game Over Panel
        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }

        // set the game over condition to true
        m_gameOver = true;
    }

    // reload the level
    public void Restart()
    {
        //Debug.Log("Restart");
        Application.LoadLevel(Application.loadedLevel);
    }

}
