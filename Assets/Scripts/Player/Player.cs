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
    [SerializeField] private TextMeshProUGUI bossTimerText;
    [SerializeField] private TextMeshProUGUI drawTimerText;
    [SerializeField] private TextMeshProUGUI interactPrompt;
    [SerializeField] private Canvas packOpenPrompt;
    [SerializeField] private Canvas packSelectPrompt;
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
    private CardPack _cardPack;

    private float _drawTimer;

    public RectTransform packOpenPos;
    public RectTransform focusedCardPos;
    public RectTransform packPreview2Discard;
    public RectTransform packPreview1;
    public RectTransform packPreview1Discard;
    public RectTransform packPreview3;
    public RectTransform packPreview3Discard;
    public RectTransform card1Pos;
    public RectTransform card2Pos;
    public RectTransform card3Pos;
    public RectTransform card4Pos;
    public RectTransform card5Pos;
    public RectTransform deckPos;
    public RectTransform discardPos;
    private GameObject _deckCard;
    private GameObject _discardCard;

    private CardObject[] _visualHand;
    private CardObject[] _visualPack;

    public enum CardSlot
    {
        PackPreview1 = -12,
        PackDiscard1 = -11,
        PackPreview2 = -10,
        PackDiscard2 = -9,
        PackPreview3 = -8,
        PackDiscard3 = -7,
        PackOpen = -6,
        PackSelect = -5,
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
        _walker = GetComponent<FPSWalk>();
        
        GameInfo.Player = GetComponent<Player>();
        GameInfo.State = GameInfo.GameState.Active;
        _cardSlotSelected = CardSlot.None;

        deathScreen.enabled = false;
        packOpenPrompt.enabled = false;
        packSelectPrompt.enabled = false;

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

        _deckCard = Instantiate(cardPrefab, deckPos.position, deckPos.rotation, focusedCardPos.parent);
        _deckCard.GetComponent<CardObject>().SetPlayer(this);
        _deckCard.GetComponent<CardObject>().SetSlot(CardSlot.Deck);

        _discardCard = Instantiate(cardPrefab, discardPos.position, discardPos.rotation, focusedCardPos.parent);
        _discardCard.GetComponent<CardObject>().SetPlayer(this);
        _discardCard.GetComponent<CardObject>().SetSlot(CardSlot.Discard);
        
        _visualHand = new CardObject[5];
        _visualPack = new CardObject[3];
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

        difficultyText.text = "cardslotselected: " + _cardSlotSelected.ToString();
        //difficultyText.text = "Difficulty: " + GameInfo.GetDifficultyModifier();
        bossTimerText.text = "Boss spawns in: " + GameInfo.TimeToBossSpawn;
        drawTimerText.text = "Next card draw in: " + _drawTimer;
        

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

        if (_cardSlotSelected != CardSlot.PackOpen && _cardSlotSelected != CardSlot.PackSelect)
        {
            _drawTimer -= Time.deltaTime;
        }

        if ((int)_cardSlotSelected >= 0 && deck.Hand[(int)_cardSlotSelected] == null)
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
        if ((int)_cardSlotSelected >= 0) // scary
        {
            Card toPlay = deck.Hand[(int)_cardSlotSelected];
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
        if (_cardSlotSelected == CardSlot.PackOpen)
        {
            if (deck.Hand[(int)cardNum] != null)
            {
                _visualHand[(int)cardNum].SetSlot(CardSlot.Discard);
                _visualHand[(int)cardNum].Discard();
                deck.Hand[(int)cardNum] = null;
                _cardSlotSelected = CardSlot.PackSelect;
                cardMoveSound.Play();
                
                _visualPack[0].SetSlot(CardSlot.PackPreview1);
                _visualPack[1].SetSlot(CardSlot.PackPreview2);
                _visualPack[2].SetSlot(CardSlot.PackPreview3);

                packOpenPrompt.enabled = false;
                packSelectPrompt.enabled = true;
            }
        }
        else if (_cardSlotSelected == CardSlot.PackSelect)
        {
            _cardSlotSelected = CardSlot.None;
            packSelectPrompt.enabled = false;

            if (cardNum == CardSlot.Card1)
            {
                AddCardToHand(_visualPack[0]);
                
                _visualPack[1].Discard();
                _visualPack[1].SetSlot(CardSlot.PackDiscard2);
                
                _visualPack[2].Discard();
                _visualPack[2].SetSlot(CardSlot.PackDiscard3);

                cardMoveSound.Play();
                return;
            }
            if (cardNum == CardSlot.Card2)
            {
                AddCardToHand(_visualPack[1]);

                _visualPack[0].Discard();
                _visualPack[0].SetSlot(CardSlot.PackDiscard1);
                
                _visualPack[2].Discard();
                _visualPack[2].SetSlot(CardSlot.PackDiscard3);
                
                cardMoveSound.Play();
                return;
            }            
            if (cardNum == CardSlot.Card3)
            {
                AddCardToHand(_visualPack[2]);
                
                _visualPack[0].Discard();
                _visualPack[0].SetSlot(CardSlot.PackDiscard1);
                
                _visualPack[1].Discard();
                _visualPack[1].SetSlot(CardSlot.PackDiscard2);
                
                cardMoveSound.Play();
                return;
            }

            _cardSlotSelected = CardSlot.PackSelect;
            packSelectPrompt.enabled = true;
        }
        else if (_cardSlotSelected == CardSlot.None) // select a card when none is currently selected.
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
        if (card == _cardSlotSelected && card != CardSlot.PackOpen)
        {
            return focusedCardPos;
        }
        else
        {
            switch (card)
            {
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
                case CardSlot.PackOpen:
                    return packOpenPos;
                case CardSlot.PackPreview1:
                    return packPreview1;
                case CardSlot.PackPreview2:
                    return focusedCardPos;
                case CardSlot.PackPreview3:
                    return packPreview3;
                case CardSlot.PackDiscard1:
                    return packPreview1Discard;
                case CardSlot.PackDiscard2:
                    return packPreview2Discard;
                case CardSlot.PackDiscard3:
                    return packPreview3Discard;
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

    public void PickupCardPack(CardPack cardPack)
    {
        _cardPack = cardPack;
        _cardSlotSelected = CardSlot.PackOpen;
        packOpenPrompt.enabled = true;
        
        _visualPack[0] = Instantiate(cardPrefab, packOpenPos.position, packOpenPos.rotation, packOpenPos.parent).GetComponent<CardObject>();
        _visualPack[0].SetPlayer(this);
        _visualPack[0].SetCard(_cardPack.Card1);
        _visualPack[0].SetSlot(CardSlot.PackOpen);
                
        _visualPack[1] = Instantiate(cardPrefab, packOpenPos.position, packOpenPos.rotation, packOpenPos.parent).GetComponent<CardObject>();
        _visualPack[1].SetPlayer(this);
        _visualPack[1].SetCard(_cardPack.Card2);
        _visualPack[1].SetSlot(CardSlot.PackOpen);
                
        _visualPack[2] = Instantiate(cardPrefab, packOpenPos.position, packOpenPos.rotation, packOpenPos.parent).GetComponent<CardObject>();
        _visualPack[2].SetPlayer(this);
        _visualPack[2].SetCard(_cardPack.Card3);
        _visualPack[2].SetSlot(CardSlot.PackOpen);
    }

    private CardSlot GetEmptyCardSlotInHand()
    {
        for (int i = 0; i < deck.Hand.Length; i++)
        {
            if (deck.Hand[i] == null)
            {
                return (CardSlot)i;
            }
        }

        return CardSlot.None;
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
            
            GameObject newCardObject = Instantiate(cardPrefab, deckPos.position, deckPos.rotation, focusedCardPos.parent);
            _visualHand[(int) slot] = newCardObject.GetComponent<CardObject>();
            _visualHand[(int) slot].SetPlayer(this);
            _visualHand[(int) slot].SetCard(deck.Hand[(int)slot]);
            _visualHand[(int) slot].SetSlot(slot);
        }
    }

    public void AddCardToHand(CardObject toAdd)
    {
        for (int i = 0; i < 5; i++)
        {
            if (deck.Hand[i] == null)
            {
                deck.Hand[i] = toAdd.GetCard();
                _visualHand[i] = toAdd;
                toAdd.SetSlot((CardSlot)i);
                return;
            }
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
