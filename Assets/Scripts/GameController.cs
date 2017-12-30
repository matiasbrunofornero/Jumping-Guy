using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Estados del juego (Detenido, Jugando)
public enum GameState
{
    Idle,
    Playing,
    Ended,
    Ready
};

public class GameController : MonoBehaviour {

    [Range(0f, 0.2f)]
    public float parallaxSpeed = 0.02f;
    public RawImage background;
    public RawImage backgroundRed;

    public RawImage platform;
    public GameObject uiIdle;
    public GameObject uiScore;

    public Text pointsText;
    public Text recordText;

    public GameState gameState = GameState.Idle;

    public GameObject player;
    public GameObject enemyGenerator;

    public float scaleTime = 6f;
    public float scaleInc = 0.25f;

    private int points = 0;

    private AudioSource musicPlayer;

    // Use this for initialization
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        recordText.text = "BEST: " + GetMaxScore().ToString();
    }
	
	// Update is called once per frame
	void Update ()
    {
        bool userAction = Input.GetKeyDown("up") || Input.GetMouseButtonDown(0);

        // Empieza el juego (Si el juego esta detenido y presiono alguna de las teclas)
        if (gameState == GameState.Idle && userAction)
        {
            gameState = GameState.Playing;
            uiIdle.SetActive(false);
            uiScore.SetActive(true);
            player.SendMessage("UpdateState", "PlayerRun");
            player.SendMessage("DustPlay");
            enemyGenerator.SendMessage("StartGenerator");
            musicPlayer.Play();
            InvokeRepeating("GameTimeScale", scaleTime, scaleTime);
        }
        else 
        if(gameState == GameState.Playing)
        {
            if (points <= 2)
            {
                Parallax();
            }
            //else
            //{
            //    ParallaxRed();
            //}
        }
        else
        if(gameState == GameState.Ready)
        {
            if(userAction)
            {
                RestartGame();
            }
        }
    }

    void Parallax()
    {
        // La velocidad se adapta a los frame de los distintos tipos de PC
        float finalSpeed = parallaxSpeed * Time.deltaTime;
        background.uvRect = new Rect(background.uvRect.x + finalSpeed, 0f, 1f, 1f);
        platform.uvRect = new Rect(platform.uvRect.x + finalSpeed * 4, 0f, 1f, 1f);
    }

    void ParallaxRed() //Change the background color by increasing the level
    {
        background.enabled = false;
        backgroundRed.enabled = true;
        float finalSpeed = parallaxSpeed * Time.deltaTime;
        backgroundRed.uvRect = new Rect(backgroundRed.uvRect.x + finalSpeed, 0f, 1f, 1f);
        platform.uvRect = new Rect(platform.uvRect.x + finalSpeed * 4, 0f, 1f, 1f);
    }

    public void RestartGame()
    {
        ResetTimeScale();
        SceneManager.LoadScene("Principal");
    }

    void GameTimeScale()
    {
        Time.timeScale += scaleInc;
        //Debug.Log("Ritmo incrementado: " + Time.timeScale.ToString());
    }

    public void ResetTimeScale(float newTimeScale = 1f)
    {
        CancelInvoke("GameTimeScale");
        Time.timeScale = newTimeScale;
        //Debug.Log("Ritmo reestablecido: " + Time.timeScale.ToString());
    }

    public void IncreasePoints()
    {
        pointsText.text = (++points).ToString();
        if(points >= GetMaxScore())
        {
            recordText.text = "BEST: " + points.ToString();
            SaveScore(points);
        }
    }

    public int GetMaxScore()
    {
        return PlayerPrefs.GetInt("Max Points", 0);
    }

    public void SaveScore(int currentPoints)
    {
        PlayerPrefs.SetInt("Max Points", currentPoints);
    }
}
