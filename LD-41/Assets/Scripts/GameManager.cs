using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

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

    // Camera
    FollowCamera followCamera;

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
    // The panel that holds the cards objects
    public GameObject handPanel;
    // The card currently selected
    public GameObject selectedCard;



    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

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
                    timer = 0f;
                    UpdateTimerText();
                    isPlanifTurn = false;
                    EndTurn();
                }
                else
                {
                    // Grab the inputs
                    float moveHorizon = Input.GetAxis("Horizontal");
                    if (moveHorizon != 0f)
                    {
                        // Debug.Log("Movement : " + moveHorizon);
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
        isPlanifTurn = true;

        // Get the FollowCamera component
        followCamera = Camera.main.GetComponent<FollowCamera>();

        // Player initialisation
        player = Instantiate(playerPrefab);
        playerGhosts = new List<GameObject>();

        // Cards initialisation
        cardsInHand = new List<GameObject>();

        // First turn is planif ?
        SetupPlanifTurn();
    }

    // Must be called before each turn
    private void SetupPlanifTurn()
    {
        timer = turnTimer;

        // Player initialisation
        // Remove old ghosts
        foreach(GameObject ghost in playerGhosts)
        {
            Destroy(ghost); // ghostbusting :)
        }
        playerGhosts = new List<GameObject>();
        currentPlayerGhost = Instantiate(playerGhostPrefab, player.transform.position, player.transform.rotation);
        followCamera.SetTarget(currentPlayerGhost);

        // Cards initialisation
        DrawCards();
        selectedCard = null;
    }

    private void EndTurn()
    {
        DiscardHand();
        selectedCard = null;
    }

    private void DiscardHand()
    {
        foreach(GameObject card in cardsInHand)
        {
            Destroy(card);
        }
        cardsInHand = new List<GameObject>();
    }

    // Card draw mechanics = random cards every turn
    private void DrawCards()
    {
        while(cardsInHand.Count < nbCards)
        {
            GameObject card = Instantiate(DrawCard());
            card.transform.SetParent(handPanel.transform);
            cardsInHand.Add(card);
        }
    }

    // Gives a random card
    private GameObject DrawCard()
    {
        return cards[Random.Range(0, cards.Count)];
    }

    public void SelectCardInHand(GameObject card)
    {
        if(cardsInHand.Find(x => x == card))
        {
            if(selectedCard != null)
            {
                selectedCard.GetComponent<Card>().DeselectCard();
            }
            selectedCard = card;
        }
        else
        {
            Debug.LogError("Card not found in hand");
        }
    }


    private void UpdateTimerText()
    {
        timerText.text = "Timer : " + timer.ToString("F");
    }
}
