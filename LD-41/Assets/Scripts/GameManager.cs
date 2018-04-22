using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    // Duration of the planification turn
    public float turnTimer = 30.0f;
    public float endOfExecTurnDelay = 0.5f;
    private float timer;

    public Text timerText;

    // State of the game : planifTurn or realTurn
    private bool isPlanifTurn = true;
    // We added delay at the end of the exec turn so there is a temporary state between the end of exec and start of next planif turn
    private bool inBetweenTurn = false;

    // Wether the game is started or not
    private bool isStarted = true;

    // Is the game over and did the player lose or beat the boss
    private bool isOver = false;
    private bool gameOver = false;

    // Score
    private long score = 0;
    public Text scoreText;

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
    //Foreground of the player healthbar
    public Image healthBar;
    //Foreground of the player shieldbar
    public Image shieldBar;
    //Animator for player
    public Animator PlayerAnimator;
    //Animator for the current ghost
    public Animator GhostAnimator;
    // Player movement speed (distance per frame)
    private float playerMoveSpeed = 0.2f;


    // Cards
    // Deck script that handles the ... deck (#WhatASurprise)
    private Deck deck;
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
    // Turn counter
    private int turn = 0;
    // Time to destination
    private float timeToDest = 0f;
    // Current timer
    private float timerPlayCardMove = 0f;

    //Panel End of game
    public GameObject panelEndOfGame;
    //Text score
    public Text scoreTextPanel;
    //Turns score
    public Text turnScorePanel;

    private bool canAct = true;



    [Header("Pickup")]
    // Pickup prefab
    public GameObject pickupPrefab;
    // List of cards eligible as pickups
    public List<GameObject> pickupCards;
    // Level bounds
    public Transform cornerTopLeft;
    public Transform cornerTopRight;
    public Transform cornerBottomLeft;


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
        monsters = new List<GameObject>();
        deck = GetComponent<Deck>();

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }

        if (isStarted && !isOver)
        {
            CheckWin();
            UpdateScoreText();
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
                    float moveVertical = Input.GetAxis("Vertical");

                    if(moveVertical != 0)
                    {
                        GhostAnimator.SetBool("isMoving", true);
                        if (currentPlayerGhost.transform.eulerAngles.y == 90)
                            currentPlayerGhost.transform.position -= (currentPlayerGhost.transform.right * moveVertical * ghostSpeed);
                        else
                            currentPlayerGhost.transform.position += (currentPlayerGhost.transform.right * moveVertical * ghostSpeed);
                    }

                    if (moveHorizon > 0f)
                    {
                        GhostAnimator.SetBool("isMoving", true);
                        currentPlayerGhost.transform.eulerAngles = new Vector3(0, 90f, 0);
                        // Debug.Log("Movement : " + moveHorizon);
                        currentPlayerGhost.transform.position += (currentPlayerGhost.transform.forward * moveHorizon * ghostSpeed);
                    }
                    else if (moveHorizon < 0f)
                    {
                        GhostAnimator.SetBool("isMoving", true);
                        currentPlayerGhost.transform.eulerAngles = new Vector3(0, -90f, 0);
                        currentPlayerGhost.transform.position -= (currentPlayerGhost.transform.forward * moveHorizon * ghostSpeed);
                    }

                    if(moveVertical == 0 && moveHorizon == 0)
                    {
                        GhostAnimator.SetBool("isMoving", false);
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
                    if (selectedCard != null && Input.GetMouseButtonDown(0))
                    {
                        Card cardEffect = selectedCard.GetComponentInChildren<Card>();
                        cardEffect.isTargeting = false;
                        cardEffect.correspondingGhostPos = currentPlayerGhost.transform.position;
                        //cardEffect.correspondingGhostRot = cardEffect.eff
                        GameObject cardEffectGO = cardEffect.gameObject;
                        cardEffectGO.transform.parent = player.transform;
                        cardsToPlay.Add(cardEffect);
                        playerGhosts.Add(currentPlayerGhost);

                        cardsInHand.Remove(selectedCard);
                        Destroy(selectedCard);
                        selectedCard = null;

                        currentPlayerGhost = Instantiate(playerGhostPrefab, currentPlayerGhost.transform.position, currentPlayerGhost.transform.rotation);
                        GhostAnimator.SetBool("isMoving", false);
                        GhostAnimator = currentPlayerGhost.GetComponent<Animator>();
                        followCamera.SetTarget(currentPlayerGhost);
                    }

                    // On pressing space we add a movement
                    if (!emptyCardPlayedThisTurn && Input.GetKeyDown(KeyCode.Space))
                    {
                        emptyCardPlayedThisTurn = true;
                        emptyCard.correspondingGhostPos = currentPlayerGhost.transform.position;
                        emptyCard.correspondingGhostRot = currentPlayerGhost.transform.localEulerAngles;
                        cardsToPlay.Add(emptyCard);
                        playerGhosts.Add(currentPlayerGhost);
                        currentPlayerGhost = Instantiate(playerGhostPrefab, currentPlayerGhost.transform.position, currentPlayerGhost.transform.rotation);
                        GhostAnimator.SetBool("isMoving", false);
                        GhostAnimator = currentPlayerGhost.GetComponent<Animator>();
                        followCamera.SetTarget(currentPlayerGhost);
                    }
                }
            }
            else if(!inBetweenTurn)
            {
                // We execute everything that was planned
                // Get a card to play
                if (cardToPlay == null)
                {
                    // No more cards we go back to planif
                    if (cardsToPlay.Count == 0)
                    {
                        inBetweenTurn = true;
                        StartCoroutine(DelayedEndExecTurn(endOfExecTurnDelay));
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
                if (cardToPlay != null && canAct)
                {
                    // Two steps to play the card => Move to the designated postion and play the effect of the card
                    timerPlayCardMove += Time.deltaTime;
                    if (timerPlayCardMove >= timeToDest)
                    {
                        // We play the card
                        if (cardToPlay != emptyCard)
                        {
                            player.transform.localEulerAngles = cardToPlay.correspondingGhostRot;
                            PlayerAnimator.SetBool("isMoving", false);
                            PlayerAnimator.Play(cardToPlay.animationDo);
                            canAct = false;
                        }
                        else
                        {
                            // Nothing to do if the current card is the empty card
                            PlayerAnimator.SetBool("isMoving", false);
                            cardToPlay = null;
                        }
                    }
                    else
                    {
                        // We move towards the ghost's position

                        Vector3 diff = cardToPlay.correspondingGhostPos - player.transform.position;
   
                        PlayerAnimator.SetBool("isMoving", true);
                        if (diff.x < 0)
                            player.transform.localEulerAngles = new Vector3(0, -90f, 0);
                        else
                            player.transform.localEulerAngles = new Vector3(0, 90f, 0);
   
                    
                        player.transform.position = Vector3.Lerp(player.transform.position, cardToPlay.correspondingGhostPos, (timerPlayCardMove / timeToDest) * playerMoveSpeed);

                        if(Vector3.Distance(cardToPlay.correspondingGhostPos, player.transform.position) < 0.2)
                        {
                            PlayerAnimator.SetBool("isMoving", false);
                        }
                        
                        //player.transform.Translate(diff.normalized * playerMoveSpeed);
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

        if(SoundManager._instance != null)
        {
            SoundManager._instance.PlayMusic(SoundType.LD41);
        }

        // Get the FollowCamera component
        followCamera = Camera.main.GetComponent<FollowCamera>();

        // Player initialisation
        player = Instantiate(playerPrefab);
        player.GetComponent<Health>().healthBar = healthBar;
        player.GetComponent<Health>().shieldBar = shieldBar;
        PlayerAnimator = player.GetComponent<Animator>();
        playerGhosts = new List<GameObject>();

        // Cards initialisation
        cardsInHand = new List<GameObject>();
        DrawCards();

        // First turn is planif ?
        SetupPlanifTurn();
    }

    // Must be called before each turn
    private void SetupPlanifTurn()
    {
        turn++;
        timer = turnTimer;
        inBetweenTurn = false;

        // Player initialisation
        playerGhosts = new List<GameObject>();
        currentPlayerGhost = Instantiate(playerGhostPrefab, player.transform.position, player.transform.rotation);
        GhostAnimator = currentPlayerGhost.GetComponent<Animator>();
        followCamera.SetTarget(currentPlayerGhost);

        // Cards initialisation
        DrawCard();
        DrawCard();
        emptyCardPlayedThisTurn = false;

        // Spawn a pickup
        if (Random.Range(0, 100) < (10 + (turn / 2)))
        {
            SpawnPickup();
        }

        // Spawn some monsters
        foreach (MonsterSpawnPoint sp in spawnPoints)
        {
            List<GameObject> newMonsters = sp.SpawnMonsters(turn);
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

        GhostAnimator.SetBool("isMoving", false);

        foreach (Card c in cardsToPlay)
        {
            if (c.effectInst != null)
            {
                Destroy(c.effectInst);
            }
        }

        GameObject[] delGhosts = new GameObject[playerGhosts.Count];
        int i = 0;
        foreach (GameObject ghost in playerGhosts)
        {
            delGhosts[i] = ghost;
            i++;
        }
        for (int j = 0; j < i; j++)
        {
            Destroy(delGhosts[j]);
        }
        Destroy(currentPlayerGhost);

        followCamera.target = player;


        SetupExecTurn();
    }

    private void SetupExecTurn()
    {

    }

    IEnumerator DelayedEndExecTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndExecTurn();
    }

    private void EndExecTurn()
    {
        isPlanifTurn = true;
        SetupPlanifTurn();
    }

    private void DiscardHand()
    {
        GameObject[] delCards = new GameObject[cardsInHand.Count];
        int i = 0;
        foreach (GameObject card in cardsInHand)
        {
            delCards[i] = card;
            i++;
        }
        for (int j = 0; j < i; j++)
        {
            Destroy(delCards[j]);
        }
        cardsInHand = new List<GameObject>();
    }

    // Draw a single card per turn
    private void DrawCard()
    {
        if (cardsInHand.Count < nbCards)
        {
           // GameObject card = Instantiate(PickRandomCard());
            GameObject card = Instantiate(deck.DrawCardFromDeck());
            card.transform.SetParent(handPanel.transform);
            cardsInHand.Add(card);
        }
    }

    // Card draw mechanics = random cards every turn
    private void DrawCards()
    {
        while (cardsInHand.Count < nbCards)
        {
            DrawCard();
        }
    }

    // Gives a random card
    private GameObject PickRandomCard()
    {
        return cards[Random.Range(0, cards.Count)];
    }

    public void PickupCard(PickupItem pick)
    {
        GameObject card = Instantiate(pick.cardPrefab);
        card.transform.SetParent(handPanel.transform);
        cardsInHand.Add(card);
    }

    private void SpawnPickup()
    {
        float minX = cornerTopLeft.position.x;
        float maxX = cornerTopRight.position.x;
        float minZ = cornerBottomLeft.position.z;
        float maxZ = cornerTopLeft.position.z;

        float posX = Random.Range(minX, maxX);
        float posY = 1.5f;
        float posZ = Random.Range(minZ, maxZ);

        GameObject card = pickupCards[Random.Range(0, pickupCards.Count)];
        GameObject pickup = Instantiate(pickupPrefab, new Vector3(posX, posY, posZ), Quaternion.identity);
        pickup.GetComponent<PickupItem>().cardPrefab = card;
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
        if (currentPlayerGhost == null)
            Debug.LogError("Wat");
        return currentPlayerGhost.transform.position;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public bool IsPlayTurn()
    {
        return !isPlanifTurn;
    }

    public void RemoveMonster(GameObject mob)
    {
        monsters.Remove(mob);
    }

    private void CheckWin()
    {
        bool win = true;
        // All SpawnPoints must be done
        foreach (MonsterSpawnPoint sp in spawnPoints)
        {
            win &= sp.isDone;
        }

        // Now we check if all monsters are dead
        if(win)
        {
            win &= monsters.Count == 0;

            if(win)
            {
                isOver = true;
                scoreTextPanel.text = score.ToString();
                turnScorePanel.text = turn.ToString();
                panelEndOfGame.SetActive(true);
            }
        }
    }

    private void UpdateTimerText()
    {
        timerText.text = "Turn " + turn.ToString() + " Timer : " + timer.ToString("F");
    }


    private void UpdateScoreText()
    {
        scoreText.text = "Score : " + score.ToString();
    }

    public void GainScore(int scoreGain)
    {
        score += scoreGain;
    }

    public void HitPlayer(int damage)
    {
        player.GetComponent<Health>().TakeDamage(damage);
    }

    public void HealPlayer()
    {
        player.GetComponent<Health>().Heal(20);
    }

    public void ShieldPlayer()
    {
        player.GetComponent<Health>().AddShield(15);
    }

    public void Lose()
    {
        gameOver = true;
        scoreTextPanel.text = score.ToString();
        turnScorePanel.text = turn.ToString();
        panelEndOfGame.SetActive(true);
        isOver = true;
    }

    public bool isGameOver()
    {
        return gameOver;
    }

    public void ExecuteCard()
    {
        cardToPlay.Do();
        cardToPlay = null;
        canAct = true;
    }

    public void ReturnToMainMenu()
    {
        if(SoundManager._instance != null)
        {
            SoundManager._instance.StopMusic();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Test");
    }
}
