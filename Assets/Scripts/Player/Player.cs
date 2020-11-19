using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

// The main player controller class. Handles health, hud, and score, but not movement. Interfaces with weapon scripts in prefabs to attack, and with an FPSWalk for movement.

public class Player : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI healthText;
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
    [SerializeField] private Canvas deathScreen;
    [SerializeField] private Canvas winScreen;
    [SerializeField] private Canvas pauseScreen;
    [SerializeField] private Transform raycastOrigin;
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

    private CardSelected _cardSelected;

    private float _drawTimer;

    private enum CardSelected
    {
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
        deck = new Deck(startingDeck.Cards);
        _cardSelected = CardSelected.None;

        deathScreen.enabled = false;

        Time.timeScale = 1;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Cards in deck: " + deck.Undrawn.Count;
        difficultyText.text = "Difficulty: " + GameInfo.GetDifficultyModifier();
        bossTimerText.text = "Boss spawns in: " + GameInfo.TimeToBossSpawn;
        drawTimerText.text = "Next card draw in: " + _drawTimer;

        card1Name.text = deck.GetNameOfCardInHand(0);
        card2Name.text = deck.GetNameOfCardInHand(1);
        card3Name.text = deck.GetNameOfCardInHand(2);
        card4Name.text = deck.GetNameOfCardInHand(3);
        card5Name.text = deck.GetNameOfCardInHand(4);

        if (_cardSelected != CardSelected.None)
        {
            selectedCardName.text = deck.Hand[(int) _cardSelected].Name;
            selectedCardDescription.text = deck.Hand[(int) _cardSelected].Description;
        }
        else
        {
            selectedCardName.text = "";
            selectedCardDescription.text = "";
        }
        
        if (weapon != null)
        {
            weaponText.text = weapon.GetWeaponName() + ", " + weapon.Ammo;
        }
        else
        {
            weaponText.text = "no weapon";
        }

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 2f)
            && hit.transform.CompareTag("UseTarget"))
        {
            interactPrompt.text = hit.transform.GetComponent<UseTarget>().Description();
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
            deck.Draw(1);
            cardDrawSound.Play();
            _drawTimer = drawInterval;
        }

        _drawTimer -= Time.deltaTime;
    }

    public void Hurt(int damage)
    {
        if (deck.Damage(damage))
        {
            GameInfo.State = GameInfo.GameState.Failure;
        }
    }

    public Vector3 GetCenter()
    {
        return _walker.GetCenter();
    }

    private void FirePressed()
    {
        if (_cardSelected != CardSelected.None)
        {
            deck.Hand[(int)_cardSelected].Activate(this, raycastOrigin);
            deck.Discard((int)_cardSelected);
            _cardSelected = CardSelected.None;
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
        if (_cardSelected != CardSelected.None)
        {
            _cardSelected = CardSelected.None;
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
        CardPressed(CardSelected.Card1);
    }

    private void Card2Pressed()
    {
        CardPressed(CardSelected.Card2);

    }

    private void Card3Pressed()
    {
        CardPressed(CardSelected.Card3);

    }

    private void Card4Pressed()
    {
        CardPressed(CardSelected.Card4);

    }

    private void Card5Pressed()
    {
        CardPressed(CardSelected.Card5);

    }

    private void CardPressed(CardSelected cardNum)
    {
        if (_cardSelected == CardSelected.None) // select a card when none is currently selected.
        {
            if (deck.Hand[(int)cardNum] != null) // is there a card in the hand slot?
            {
                _cardSelected = cardNum; // select the card
                cardMoveSound.Play();
            }
        }
        else if (_cardSelected == cardNum) // deselect a card by pressing it's button again.
        {
            _cardSelected = CardSelected.None;
            cardMoveSound.Play();
        }
        else // switch cards by pressing another card's button when a card is selected.
        {
            // switch the cards
            Card card = deck.Hand[(int)_cardSelected];
            deck.Hand[(int) _cardSelected] = deck.Hand[(int)cardNum];
            deck.Hand[(int)cardNum] = card;
            
            if (deck.Hand[(int)cardNum] != null) // is there a card in the hand slot?
            {
                _cardSelected = CardSelected.Card1;
            }
            cardMoveSound.Play();
        }
    }

    private void Use()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 2f)
            && hit.transform.CompareTag("UseTarget"))
        {
            hit.transform.GetComponent<UseTarget>().Use();
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
