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

    // Player
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
    [HideInInspector]
    public GameObject selectedCard;
    // The cards that must be played
    private List<Card> cardsToPlay;
    // The card currently being played
    private Card cardToPlay;
    // The empty card
    public Card emptyCard;
    private bool emptyCardPlayedThisTurn = false;


    // Monsters and SpawnPoints
    // List of spawnPoints
    public List<MonsterSpawnPoint> spawnPoints;
    // Empty object to hold all the shit we spawn
    public Transform monstersParent;
    // List of the spawned monsters
    private List<GameObject> monsters;


    // Playing the turn
    // Time to destination
    private float timeToDest = 0f;
    // Current timer
    private float timerPlayCardMove = 0f;
    // Player movement speed (distance per frame)
    private float playerMoveSpeed = 0.2f;

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
        cardsToPlay = new List<Card>();

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
                    EndPlanifTurn();
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

                    if (cardsInHand.Count >= 1 && Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        SelectCardInHand(cardsInHand[0]);
                    }
                    if (cardsInHand.Count >= 2 && Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        SelectCardInHand(cardsInHand[1]);
                    }
                    if (cardsInHand.Count >= 3 && Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        SelectCardInHand(cardsInHand[2]);
                    }
                    if (cardsInHand.Count >= 4 && Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        SelectCardInHand(cardsInHand[3]);
                    }
                    if (cardsInHand.Count >= 5 && Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        SelectCardInHand(cardsInHand[4]);
                    }


                    // On left-click we validate the selected card play
                    if(selectedCard != null && Input.GetMouseButtonDown(0))
                    {
                        Card cardEffect = selectedCard.GetComponentInChildren<Card>();
                        cardEffect.isTargeting = false;
                        cardEffect.correspondingGhostPos = currentPlayerGhost.transform.position;
                        GameObject cardEffectGO = cardEffect.gameObject;
                        cardEffectGO.transform.parent = player.transform;
                        cardsToPlay.Add(cardEffect);
                        playerGhosts.Add(currentPlayerGhost);

                        cardsInHand.Remove(selectedCard);
                        Destroy(selectedCard);
                        selectedCard = null;

                        currentPlayerGhost = Instantiate(playerGhostPrefab, currentPlayerGhost.transform.position, currentPlayerGhost.transform.rotation);
                        followCamera.SetTarget(currentPlayerGhost);
                    }

                    // On pressing space we add a movement
                    if(!emptyCardPlayedThisTurn && Input.GetKeyDown(KeyCode.Space))
                    {
                        emptyCardPlayedThisTurn = true;
                        emptyCard.correspondingGhostPos = currentPlayerGhost.transform.position;
                        cardsToPlay.Add(emptyCard);
                        playerGhosts.Add(currentPlayerGhost);
                        currentPlayerGhost = Instantiate(playerGhostPrefab, currentPlayerGhost.transform.position, currentPlayerGhost.transform.rotation);
                        followCamera.SetTarget(currentPlayerGhost);
                    }
                }
            }
            else
            {
                // We execute everything that was planned
                // Get a card to play
                if (cardToPlay == null)
                {
                    // No more cards we go back to planif
                    if(cardsToPlay.Count == 0)
                    {
                        isPlanifTurn = true;
                        EndExecTurn();
                    }
                    else
                    {
                        // Get a card from the list
                        cardToPlay = cardsToPlay[0];
                        cardsToPlay.Remove(cardToPlay);

                        // Calculate the time it takes to move there
                        timeToDest = Vector3.Distance(player.transform.position, cardToPlay.correspondingGhostPos) * (1 / playerMoveSpeed) * Time.deltaTime;
                        timerPlayCardMove = 0f;
                        // Debug.Log("Dist to goal : " + Vector3.Distance(player.transform.position, cardToPlay.correspondingGhostPos) + ", time : " + Vector3.Distance(player.transform.position, cardToPlay.correspondingGhostPos) * (1/playerMoveSpeed) * Time.deltaTime);
                    }
                }

                // Allows to grab a card and start playing it in the same frame
                if(cardToPlay!=null)
                {
                    // Two steps to play the card => Move to the designated postion and play the effect of the card
                    timerPlayCardMove += Time.deltaTime;
                    if(timerPlayCardMove >= timeToDest)
                    {
                        // We play the card
                        cardToPlay.Do();

                        cardToPlay = null;
                    }
                    else
                    {
                        // We move towards the ghost's position
                        player.transform.Translate((cardToPlay.correspondingGhostPos - player.transform.position).normalized * playerMoveSpeed);
                    }
                }
                
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
        DrawCards();

        // Monster initialisation
        monsters = new List<GameObject>();

        // First turn is planif ?
        SetupPlanifTurn();
    }

    // Must be called before each turn
    private void SetupPlanifTurn()
    {
        timer = turnTimer;

        // Player initialisation
        playerGhosts = new List<GameObject>();
        currentPlayerGhost = Instantiate(playerGhostPrefab, player.transform.position, player.transform.rotation);
        followCamera.SetTarget(currentPlayerGhost);

        // Cards initialisation
        DrawCard();
        emptyCardPlayedThisTurn = false;



        // Spawn some monsters
        foreach (MonsterSpawnPoint sp in spawnPoints)
        {
            List<GameObject> newMonsters = sp.SpawnMonsters(5);
            foreach (GameObject mob in newMonsters)
            {
                monsters.Add(mob);
                mob.transform.SetParent(monstersParent);
            }
        }
    }

    private void EndPlanifTurn()
    {
        // DiscardHand();
        if (selectedCard != null)
        {
            selectedCard.GetComponent<CardInHand>().DeselectCard();
        }
        selectedCard = null;

        foreach(Card c in cardsToPlay)
        {
            if(c.effectInst != null)
            {
                Destroy(c.effectInst);
            }
        }

        foreach(GameObject ghost in playerGhosts)
        {
            Destroy(ghost);
        }
        Destroy(currentPlayerGhost);

        followCamera.target = player;

        SetupExecTurn();
    }

    private void SetupExecTurn()
    {

    }

    private void EndExecTurn()
    {
        SetupPlanifTurn();
    }

    private void DiscardHand()
    {
        foreach (GameObject card in cardsInHand)
        {
            Destroy(card);
        }
        cardsInHand = new List<GameObject>();
    }

    // Draw a single card per turn
    private void DrawCard()
    {
        if (cardsInHand.Count < nbCards)
        {
            GameObject card = Instantiate(PickRandomCard());
            card.transform.SetParent(handPanel.transform);
            cardsInHand.Add(card);
        }
    }

    // Card draw mechanics = random cards every turn
    private void DrawCards()
    {
        while (cardsInHand.Count < nbCards)
        {
            GameObject card = Instantiate(PickRandomCard());
            card.transform.SetParent(handPanel.transform);
            cardsInHand.Add(card);
        }
    }

    // Gives a random card
    private GameObject PickRandomCard()
    {
        return cards[Random.Range(0, cards.Count)];
    }

    public void SelectCardInHand(GameObject card)
    {
        if (cardsInHand.Find(x => x == card))
        {
            if (selectedCard != null)
            {
                selectedCard.GetComponent<CardInHand>().DeselectCard();
            }
            selectedCard = card;
            selectedCard.GetComponent<CardInHand>().SelectCard();
        }
        else
        {
            Debug.LogError("Card not found in hand");
        }
    }


    public Vector3 GetCurrentGhostPosition()
    {
        return currentPlayerGhost.transform.position;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public bool isPlayTurn()
    {
        return !isPlanifTurn;
    }

    private void UpdateTimerText()
    {
        timerText.text = "Timer : " + timer.ToString("F");
    }
}
