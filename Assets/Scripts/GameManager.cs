using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    private List<Fruit> fruits = new List<Fruit>();
    private List<Bomb> bombs = new List<Bomb>();

    public GameObject fruitPrefab;
    public GameObject bombPrefab;
    public Transform trail;

    private Collider2D[] entitiesCols;

    public const float REQUIRED_SLICEFORCE = 400.0f;
    private Vector3 lastMousePos;
    private float lastSpawn;
    private float deltaSpawn = 0.6f;
    private bool isPaused;

    // UI part of the game
    private int score;
    private int highscore;
    private int lifepoint;
    public Text scoreText;
    public Text highscoreText;
    public Image[] lifepoints;
    public GameObject pauseMenu;
    public GameObject deathMenu;

    private Fruit GetFruit()
    {
        Fruit f = fruits.Find(x => !x.IsActive);
        if (f == null)
        {
            f = Instantiate(fruitPrefab).GetComponent<Fruit>();
            fruits.Add(f);
        }
        return f;
    }

    private Bomb GetBomb()
    {
        Bomb b = bombs.Find(x => !x.IsActive);
        if (b == null)
        {
            b = Instantiate(bombPrefab).GetComponent<Bomb>();
            bombs.Add(b);
        }
        return b;
    }

    public void SliceAllFruits()
    {
        foreach (var fruit in fruits)
        {
            fruit.Slice();
        }
    }

    public void NewGame()
    {
        score = 0;
        lifepoint = 3;
        entitiesCols = new Collider2D[0];
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        scoreText.text = score.ToString();
        highscore = PlayerPrefs.GetInt("Score");
        highscoreText.text = $"BEST : {highscore}";
        deathMenu.SetActive(false);
        foreach (Image i in lifepoints)
            i.enabled = true;
        foreach (Fruit f in fruits)
            Destroy(f.gameObject);
        foreach (Bomb b in bombs)
            Destroy(b.gameObject);
        fruits.Clear();
        bombs.Clear();
    }

    public void LoseLifepoint()
    {
        if (lifepoint == 0) return;
        
        lifepoint--;
        lifepoints[lifepoint].enabled = false;
        if (lifepoint == 0)
        {
            Lose();
        }
    }

    public void IncrementScore(int scoreAmount)
    {
        score += scoreAmount;
        scoreText.text = score.ToString();
        if (score > highscore)
        {
            highscore = score;
            highscoreText.text = $"BEST : {highscore}";
            PlayerPrefs.SetInt("Score", highscore);
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        isPaused = pauseMenu.activeSelf;
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
    }

    public void Lose()
    {
        isPaused = true;
        deathMenu.SetActive(true);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (isPaused) return;

        if (Time.time - lastSpawn > deltaSpawn)
        {
            float randomX = Random.Range(-1.65f, 1.65f);
            float randomVel = Random.Range(1.85f, 2.75f);
            
            int x = (int)Random.Range(0.0f, 6.0f);
            if (x == 0)
            {
                Bomb b = GetBomb();
                b.LaunchBomb(randomVel, randomX, -randomX);
            } else
            {
                Fruit f = GetFruit();
                f.LaunchFruit(randomVel, randomX, -randomX);
            }
            lastSpawn = Time.time;
        }



        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -1;
            trail.position = pos;

            Collider2D[] thisFrameEntities = Physics2D.OverlapPointAll(new Vector2(pos.x, pos.y), LayerMask.GetMask("Default"));
            
            if ((Input.mousePosition - lastMousePos).sqrMagnitude > REQUIRED_SLICEFORCE)
                foreach(Collider2D c2 in entitiesCols)
                {
                    for (int i = 0; i < entitiesCols.Length; i++)
                    {
                        if (c2 == entitiesCols[i])
                        {
                            Fruit f = c2.GetComponent<Fruit>();
                            if (f != null) f.Slice();
                            Bomb b = c2.GetComponent<Bomb>();
                            if (b != null) b.Slice();
                        }
                    }
                }
            lastMousePos = Input.mousePosition;
            entitiesCols = thisFrameEntities;
        }
    }
}
