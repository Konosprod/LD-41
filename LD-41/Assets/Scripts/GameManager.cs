using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    // Duration of the planification turn
    public float turnTimer = 30.0f;
    private float timer;

    public Text timerText;

    // State of the game : planifTurn or realTurn
    private bool isPlanifTurn = true;

    // Wether the game is started or not
    private bool isStarted = true;

    // Is the game over and did the player lose or beat the boss
    private bool isOver = false;
    private bool gameOver = false;

    // Prefab for the player
    public GameObject playerPrefab;
    private GameObject player; // Actual player GameObject
    // Prefab for the player ghost
    public GameObject playerGhostPrefab;
    // List of ghost used to represent card uses
    private List<GameObject> playerGhosts;
    private GameObject currentPlayerGhost;
    // Speed of the ghost
    public float ghostSpeed = 0.2f;


    // Cards
    // All Cards of the game
    public List<GameObject> cards;
    // Hand of the player
    private List<GameObject> cardsInHand;
    // Number of cards in hand
    public int nbCards = 5;


    // Use this for initialization
    void Start()
    {
        timer = turnTimer;
        playerGhosts = new List<GameObject>();

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted && !isOver)
        {
            if (isPlanifTurn)
            {
                // The world is stopped, you can plan for your movement and card use
                timer -= Time.deltaTime;
                UpdateTimerText();
                if (timer <= 0f)
                {
                    // Planification is over
                    isPlanifTurn = false;
                }
                else
                {
                    // Grab the inputs
                    float moveHorizon = Input.GetAxis("Horizontal");
                    if (moveHorizon != 0f)
                    {
                        Debug.Log("Movement : " + moveHorizon);
                        if (!currentPlayerGhost.activeSelf)
                        {
                            currentPlayerGhost.SetActive(true);
                        }
                        currentPlayerGhost.transform.Translate(Vector3.right * moveHorizon * ghostSpeed);
                    }

                }
            }
            else
            {
                // We execute everything that was planned

            }
        }
    }


    public void StartGame()
    {
        isStarted = true;
        isOver = false;
        gameOver = false;
        timer = turnTimer;
        isPlanifTurn = true;

        // Player initialisation
        player = Instantiate(playerPrefab);
        playerGhosts = new List<GameObject>();
        currentPlayerGhost = Instantiate(playerGhostPrefab);
        currentPlayerGhost.SetActive(false);

        // Cards initialisation
        cardsInHand = new List<GameObject>();
    }

    // Card draw mechanics = random cards every turn
    private void DrawCards()
    {
        cardsInHand = new List<GameObject>();
        while(cardsInHand.Count < nbCards)
        {
            cardsInHand.Add(DrawCard());
        }
    }

    // Gives a random card
    private GameObject DrawCard()
    {
        return cards[Random.Range(0, cards.Count)];
    }


    private void UpdateTimerText()
    {
        timerText.text = "Timer : " + timer.ToString("F");
    }
}
