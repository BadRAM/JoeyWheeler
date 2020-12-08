using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.XR;
using Random = UnityEngine.Random;

// The main player controller class. Handles health, hud, and score, but not movement. Interfaces with weapon scripts in prefabs to attack, and with an FPSWalk for movement.

public class Player : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI deckText;
    [SerializeField] private TextMeshProUGUI discardText;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private TextMeshProUGUI bossTimerText;
    [SerializeField] private TextMeshProUGUI drawTimerText;
    [SerializeField] private TextMeshProUGUI interactPrompt;
    [SerializeField] private TextMeshProUGUI selectedCardName;
    [SerializeField] private TextMeshProUGUI selectedCardDescription;
    [SerializeField] private TextMeshProUGUI card1Name;
    [SerializeField] private TextMeshProUGUI card2Name; 
    [SerializeField] private TextMeshProUGUI card3Name;
    [SerializeField] private TextMeshProUGUI card4Name;
    [SerializeField] private TextMeshProUGUI card5Name;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Canvas deathScreen;
    [SerializeField] private Canvas winScreen;
    [SerializeField] private Canvas pauseScreen;
    [SerializeField] public Transform raycastOrigin;
    [SerializeField] private Transform hand;
    [SerializeField] private StartingDeck startingDeck;
    [SerializeField] private float drawInterval;
    [SerializeField] private SFXPool weaponLoadSound;
    [SerializeField] private SFXPool cardDrawSound;
    [SerializeField] private SFXPool cardMoveSound;

    [HideInInspector] public Weapon weapon;
    private Transform _weaponTransform;

    private FPSWalk _walker;
    private Controls _input;

    [HideInInspector] public Deck deck;

    private CardSlot _cardSlotSelected;

    private float _drawTimer;

    [HideInInspector] public Transform focusedCardPos;
    [HideInInspector] public Transform card1Pos;
    [HideInInspector] public Transform card2Pos;
    [HideInInspector] public Transform card3Pos;
    [HideInInspector] public Transform card4Pos;
    [HideInInspector] public Transform card5Pos;
    [HideInInspector] public Transform deckPos;
    [HideInInspector] public Transform discardPos;
    private GameObject _deckCard;
    private GameObject _discardCard;

    private CardObject[] _visualHand;

    public enum CardSlot
    {
        Held = -4,
        Deck = -3,
        Discard = -2,
        None = -1,
        Card1 = 0,
        Card2 = 1,
        Card3 = 2,
        Card4 = 3,
        Card5 = 4
    }

    private void Awake()
    {
        _input = new Controls();
        _input.Player.Enable();
        _input.Player.Use.performed += ctx => Use();
//        _input.Player.Pause.performed += ctx => TogglePause();
        _input.Player.Fire.performed += ctx => FirePressed();
        _input.Player.Fire.canceled += ctx => FireReleased();
        _input.Player.AltFire.performed += ctx => AltFirePressed();
        _input.Player.AltFire.canceled += ctx => AltFireReleased();

        _input.Player.Card1.performed += ctx => Card1Pressed();
        _input.Player.Card2.performed += ctx => Card2Pressed();
        _input.Player.Card3.performed += ctx => Card3Pressed();
        _input.Player.Card4.performed += ctx => Card4Pressed();
        _input.Player.Card5.performed += ctx => Card5Pressed();
    }

    // Start is called before the first frame update
    void Start()
    {
        List<Transform> spawns = new List<Transform>();

        foreach (GameObject i in GameObject.FindGameObjectsWithTag("ScorePickupSpawn"))
        {
            spawns.Add(i.transform);
        }
        transform.position = spawns[Random.Range(0, spawns.Count-1)].position;
        
        _walker = GetComponent<FPSWalk>();
        
        GameInfo.Player = GetComponent<Player>();
        GameInfo.State = GameInfo.GameState.Active;
        _cardSlotSelected = CardSlot.None;

        deathScreen.enabled = false;

        Time.timeScale = 1;
        
        Cursor.lockState = CursorLockMode.Locked;

        Camera cam = raycastOrigin.GetComponent<Camera>();

        if (GameInfo.LoadSavedDeck() != null)
        {
            Debug.Log("Loading saved deck.");
            deck = new Deck(GameInfo.LoadSavedDeck());
        }
        else
        {
            deck = new Deck(startingDeck.Cards);
        }
        
        focusedCardPos = new GameObject().transform;
        focusedCardPos.gameObject.name = "FocusedCardPos";
        focusedCardPos.parent = raycastOrigin;
        focusedCardPos.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f, 0.25f));
        focusedCardPos.rotation = raycastOrigin.rotation;
        
        card1Pos = new GameObject().transform;
        card1Pos.gameObject.name = "Card1Pos";
        card1Pos.parent = raycastOrigin;
        card1Pos.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.2f, cam.pixelHeight * 0.1f, 0.4f));
        card1Pos.rotation = raycastOrigin.rotation;
        
        card2Pos = new GameObject().transform;
        card2Pos.gameObject.name = "Card2Pos";
        card2Pos.parent = raycastOrigin;
        card2Pos.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.35f, cam.pixelHeight * 0.1f, 0.4f));
        card2Pos.rotation = raycastOrigin.rotation;
        
        card3Pos = new GameObject().transform;
        card3Pos.gameObject.name = "Card3Pos";
        card3Pos.parent = raycastOrigin;
        card3Pos.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.1f, 0.4f));
        card3Pos.rotation = raycastOrigin.rotation;
        
        card4Pos = new GameObject().transform;
        card4Pos.gameObject.name = "Card4Pos";
        card4Pos.parent = raycastOrigin;
        card4Pos.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.65f, cam.pixelHeight * 0.1f, 0.4f));
        card4Pos.rotation = raycastOrigin.rotation;
        
        card5Pos = new GameObject().transform;
        card5Pos.gameObject.name = "Card5Pos";
        card5Pos.parent = raycastOrigin;
        card5Pos.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.8f, cam.pixelHeight * 0.1f, 0.4f));
        card5Pos.rotation = raycastOrigin.rotation;
        
        deckPos = new GameObject().transform;
        deckPos.gameObject.name = "DeckPos";
        deckPos.parent = raycastOrigin;
        deckPos.position = cam.ScreenToWorldPoint(new Vector3( cam.pixelWidth * 0.95f, cam.pixelHeight * 0.1f, 0.5f));
        deckPos.rotation = Quaternion.LookRotation(-raycastOrigin.forward, raycastOrigin.up);
        
        _deckCard = Instantiate(cardPrefab, deckPos.position, deckPos.rotation, deckPos);
        _deckCard.GetComponent<CardObject>().SetPlayer(this);
        _deckCard.GetComponent<CardObject>().BeSmall();
        _deckCard.GetComponent<CardObject>().SetSlot(CardSlot.Deck);
        
        discardPos = new GameObject().transform;
        discardPos.gameObject.name = "DiscardPos";
        discardPos.parent = raycastOrigin;
        discardPos.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.05f, cam.pixelHeight * 0.1f, 0.5f));
        discardPos.rotation = Quaternion.LookRotation(-raycastOrigin.forward, raycastOrigin.up);
        
        _discardCard = Instantiate(cardPrefab, discardPos.position, discardPos.rotation, discardPos);
        _discardCard.GetComponent<CardObject>().SetPlayer(this);
        _discardCard.GetComponent<CardObject>().BeSmall();
        _discardCard.GetComponent<CardObject>().SetSlot(CardSlot.Discard);
        
        _visualHand = new CardObject[5];
    }

    // Update is called once per frame
    void Update()
    {
        if (deck.Undrawn.Count == 0)
        {
            deckText.text = "";
            _deckCard.SetActive(false);
        }
        else
        {
            deckText.text = "" + deck.Undrawn.Count;
            _deckCard.SetActive(true);
        }

        if (deck.Discards.Count == 0)
        {
            discardText.text = "";
            _discardCard.SetActive(false);
        }
        else
        {
            discardText.text = "" + deck.Discards.Count;
            _discardCard.SetActive(true);
        }

        difficultyText.text = "Difficulty: " + GameInfo.GetDifficultyModifier();
        bossTimerText.text = "Boss spawns in: " + GameInfo.TimeToBossSpawn;
        drawTimerText.text = "Next card draw in: " + _drawTimer;

//        card1Name.text = deck.GetNameOfCardInHand(0);
//        card2Name.text = deck.GetNameOfCardInHand(1);
//        card3Name.text = deck.GetNameOfCardInHand(2);
//        card4Name.text = deck.GetNameOfCardInHand(3);
//        card5Name.text = deck.GetNameOfCardInHand(4);

//        if (_cardSlotSelected != CardSlot.None)
//        {
//            selectedCardName.text = deck.Hand[(int) _cardSlotSelected].Name;
//            selectedCardDescription.text = deck.Hand[(int) _cardSlotSelected].Description;
//        }
//        else
//        {
//            selectedCardName.text = "";
//            selectedCardDescription.text = "";
//        }
        
        if (weapon != null)
        {
            weaponText.text = weapon.GetWeaponName() + ", " + weapon.Ammo;
        }
        else
        {
            weaponText.text = "no weapon";
        }

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 2f, LayerMask.GetMask("UseTarget")))
        {
            interactPrompt.text = hit.collider.GetComponent<UseTarget>().Description();
        }
        else
        {
            interactPrompt.text = "";
        }
        
        // update various aspects of game state.
        // these are wildly inefficient, and should be changed to only trigger on state change.
        if (GameInfo.Paused)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            deathScreen.enabled = false;
            winScreen.enabled = false;
            _walker.LockLook = true;
        }
        else
        {
            switch (GameInfo.State)
            {
                case GameInfo.GameState.Active:
                    Time.timeScale = 1;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    deathScreen.enabled = false;
                    winScreen.enabled = false;
                    _walker.LockLook = false;
                    break;
                
                case GameInfo.GameState.Failure:
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    deathScreen.enabled = true;
                    winScreen.enabled = false;
                    _walker.LockLook = true;
                    break;
                
                case GameInfo.GameState.Victory:
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    deathScreen.enabled = false;
                    winScreen.enabled = true;
                    _walker.LockLook = true;         
                    break;
            }
        }
                


        GameInfo.RunTime += Time.deltaTime;
        GameInfo.TimeToBossSpawn = Mathf.Max(0, GameInfo.TimeToBossSpawn - Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_drawTimer <= 0)
        {
            Debug.Log("Drawing.");
            Draw();
            cardDrawSound.Play();
            _drawTimer = drawInterval;
        }

        _drawTimer -= Time.deltaTime;

        if (_cardSlotSelected != CardSlot.None && deck.Hand[(int)_cardSlotSelected] == null)
        {
            _cardSlotSelected = CardSlot.None;
        }
    }

    public void Hurt(int damage)
    {
        if (deck.Damage(damage))
        {
            GameInfo.State = GameInfo.GameState.Failure;
        }

        for (int i = 0; i < 5; i++)
        {
            if (_visualHand[i] != null && deck.Hand[i] == null)
            {
                _visualHand[i].SetSlot(CardSlot.Discard);
                _visualHand[i].Discard();
            }
        }
    }

    public Vector3 GetCenter()
    {
        return _walker.GetCenter();
    }

    private void FirePressed()
    {
        if (_cardSlotSelected != CardSlot.None)
        {
            Card toPlay = deck.Hand[(int) _cardSlotSelected];
            deck.Discard((int)_cardSlotSelected);
            _visualHand[(int)_cardSlotSelected].SetSlot(CardSlot.Discard);
            _visualHand[(int)_cardSlotSelected].Discard();
            toPlay.Activate(this, raycastOrigin);
            _cardSlotSelected = CardSlot.None;
            cardMoveSound.Play();
        }
        else if (weapon != null)
        {
            weapon.FirePressed();
        }
    }

    private void FireReleased()
    {
        if (weapon != null)
        {
            weapon.FireReleased();
        }
    }

    private void AltFirePressed()
    {
        if (_cardSlotSelected != CardSlot.None)
        {
            _visualHand[(int)_cardSlotSelected].SetSlot(_cardSlotSelected);
            _cardSlotSelected = CardSlot.None;
            cardMoveSound.Play();
        }
        else if (weapon != null)
        {
            weapon.AltFirePressed();
        }
    }

    private void AltFireReleased()
    {
        if (weapon != null)
        {
            weapon.AltFireReleased();
        }
    }

    private void Card1Pressed()
    {
        CardPressed(CardSlot.Card1);
    }

    private void Card2Pressed()
    {
        CardPressed(CardSlot.Card2);

    }

    private void Card3Pressed()
    {
        CardPressed(CardSlot.Card3);

    }

    private void Card4Pressed()
    {
        CardPressed(CardSlot.Card4);

    }

    private void Card5Pressed()
    {
        CardPressed(CardSlot.Card5);

    }

    private void CardPressed(CardSlot cardNum)
    {
        if (_cardSlotSelected == CardSlot.None) // select a card when none is currently selected.
        {
            if (deck.Hand[(int)cardNum] != null) // is there a card in the hand slot?
            {
                _cardSlotSelected = cardNum; // select the card
                cardMoveSound.Play();
            }
        }
        else if (_cardSlotSelected == cardNum) // deselect a card by pressing it's button again.
        {
            _cardSlotSelected = CardSlot.None;
            cardMoveSound.Play();
        }
        else // switch cards by pressing another card's button when a card is selected.
        {
            // switch the cards
            Card card = deck.Hand[(int)_cardSlotSelected];
            CardObject cardObject = _visualHand[(int) _cardSlotSelected];
            cardObject.SetSlot(cardNum);
            _visualHand[(int) cardNum].SetSlot(_cardSlotSelected);
            deck.Hand[(int) _cardSlotSelected] = deck.Hand[(int)cardNum];
            _visualHand[(int) _cardSlotSelected] = _visualHand[(int) cardNum];
            deck.Hand[(int)cardNum] = card;
            _visualHand[(int) cardNum] = cardObject;
            
            if (deck.Hand[(int)cardNum] != null) // is there a card in the hand slot?
            {
                _cardSlotSelected = CardSlot.Card1;
            }
            cardMoveSound.Play();
        }
    }

    public Transform GetCardModelTarget(CardSlot card)
    {
        if (card == _cardSlotSelected)
        {
            return focusedCardPos;
        }
        else
        {
            switch (card)
            {
                case CardSlot.None:
                    return focusedCardPos;
                case CardSlot.Held:
                    return focusedCardPos;
                case CardSlot.Card1:
                    return card1Pos;
                case CardSlot.Card2:
                    return card2Pos;
                case CardSlot.Card3:
                    return card3Pos;
                case CardSlot.Card4:
                    return card4Pos;
                case CardSlot.Card5:
                    return card5Pos;
                case CardSlot.Deck:
                    return deckPos;
                case CardSlot.Discard:
                    return discardPos;
                default:
                    return focusedCardPos;
            }
        }
    }

    private void Use()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 2f, LayerMask.GetMask("UseTarget")))
        {
            hit.transform.GetComponent<UseTarget>().Use(this);
        }
    }

    public void PickupCard(CardObject cardObject)
    {
        if (_cardSlotSelected != CardSlot.None)
        {
            _visualHand[(int)_cardSlotSelected].Drop(cardObject.transform.position);
            _visualHand[(int) _cardSlotSelected] = cardObject;
            //_visualHand[(int)_cardSlotSelected].Drop();
            deck.Hand[(int) _cardSlotSelected] = _visualHand[(int) _cardSlotSelected].GetCard();

            cardObject.transform.position = focusedCardPos.position;

            cardObject.SetPlayer(this);
            cardObject.SetSlot(CardSlot.Held);
        }
        
    }

    public void Draw()
    {
        if (deck.CardsInHand() < 5 && deck.Undrawn.Count > 0)
        {
            CardSlot slot = CardSlot.None;
            for (int i = 0; i < 5; i++)
            {
                if (deck.Hand[i] == null)
                {
                    deck.Hand[i] = deck.Undrawn[0];
                    slot = (CardSlot)i;
                    break;
                }
            }
            deck.Undrawn.RemoveAt(0);
            
            GameObject newCardObject = Instantiate(cardPrefab, deckPos.position, deckPos.rotation, deckPos);
            _visualHand[(int) slot] = newCardObject.GetComponent<CardObject>();
            _visualHand[(int) slot].SetPlayer(this);
            _visualHand[(int) slot].BeSmall();
            _visualHand[(int) slot].SetCard(deck.Hand[(int)slot]);
            _visualHand[(int) slot].SetSlot(slot);
        }
    }

    public void EquipWeapon(GameObject prefab)
    {
        if (weapon != null)
        {
            weapon.Drop();
        }
        _weaponTransform = Instantiate(prefab, hand).transform;
        weapon = _weaponTransform.GetComponent<Weapon>();
        weapon.SetPlayer(this);
        weapon.SetRaycastOrigin(raycastOrigin);
        weaponText.text = weapon.name;
        LoadWeapon();
    }

    public void LoadWeapon()
    {
        weapon.Ammo = weapon.MaxAmmo;
        weaponLoadSound.Play();
    }

    // clears weapon variables, only meant to be called from Weapon.DropWeapon
    public void DropWeapon()
    {
        weapon = null;
        _weaponTransform = null;
    }
}
