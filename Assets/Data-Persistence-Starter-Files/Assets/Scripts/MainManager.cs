using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    private int h_Score;
    private string h_ScorePlayerName;

    private string currentPlayerName;

    private void Start()
    {

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        // Load the player's name from PlayerPrefs
        currentPlayerName = PlayerPrefs.GetString("PlayerName", "Player");

        LoadData();

        // Update the high score text at the start of the game
        HighScoreText.text = $"BestScore : {h_ScorePlayerName} - {h_Score}";

    }

    private void Update()
    {
        // If the game hasn't started, check for Space key press
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        // If the game is over, allow restarting with Space
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        // Check if current points beat the high score
        if (m_Points > h_Score)
        {
            // Update the high score
            UpdateHighScore();
            Debug.Log("New High Score Achieved!");
        }
    }

    private void UpdateHighScore()
    {
        // Update high score and player name
        h_Score = m_Points;
        h_ScorePlayerName = currentPlayerName;

        // Update high score text
        HighScoreText.text = $"BestScore : {h_ScorePlayerName} - {h_Score}";

        // Save the new high score and player name
        SavedData();
    }


    [System.Serializable]
    class SaveData
    {
        public int HighScore;
        public string HighScorePlayerName;
    }

    public void SavedData()
    {
        SaveData data = new SaveData();
        data.HighScore = h_Score;
        data.HighScorePlayerName = h_ScorePlayerName;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            h_Score = data.HighScore;
            h_ScorePlayerName = data.HighScorePlayerName;

            //File.Delete(path);  
        }
        else
        {
            h_Score = 0;
            h_ScorePlayerName = "No Name";
        }
    }
}
