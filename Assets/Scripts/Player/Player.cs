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
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform hand;
    [SerializeField] private StartingDeck startingDeck;
    [SerializeField] private float drawInterval;

    private Weapon _weapon;
    private Transform _weaponTransform;

    private FPSWalk _walker;
    private Controls _input;

    private Deck _deck;

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
        _deck = new Deck(startingDeck.Cards);
        _cardSelected = CardSelected.None;

        deathScreen.enabled = false;

        Time.timeScale = 1;
        
        Cursor.lockState = CursorLockMode.Locked;

        EquipRandomWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Cards in deck: " + _deck.Undrawn.Count;
        difficultyText.text = "Difficulty: " + GameInfo.GetDifficultyModifier();
        bossTimerText.text = "Boss spawns in: " + GameInfo.TimeToBossSpawn;
        drawTimerText.text = "Next card draw in: " + _drawTimer;

        card1Name.text = _deck.GetNameOfCardInHand(0);
        card2Name.text = _deck.GetNameOfCardInHand(1);
        card3Name.text = _deck.GetNameOfCardInHand(2);
        card4Name.text = _deck.GetNameOfCardInHand(3);
        card5Name.text = _deck.GetNameOfCardInHand(4);

        if (_cardSelected != CardSelected.None)
        {
            selectedCardName.text = _deck.Hand[(int) _cardSelected].Name;
            selectedCardDescription.text = _deck.Hand[(int) _cardSelected].Description;
        }
        else
        {
            selectedCardName.text = "";
            selectedCardDescription.text = "";
        }
        
        if (_weapon != null)
        {
            weaponText.text = _weapon.GetWeaponName() + ", " + _weapon.Ammo;
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
            _deck.Draw(1);
            _drawTimer = drawInterval;
        }

        _drawTimer -= Time.deltaTime;
    }

    public void Hurt(int damage)
    {
        if (_deck.Damage(damage))
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
            _deck.Hand[(int)_cardSelected].Activate(this, raycastOrigin);
            _deck.Discard((int)_cardSelected);
            _cardSelected = CardSelected.None;
        }
        else if (_weapon != null)
        {
            _weapon.FirePressed();
        }
    }

    private void FireReleased()
    {
        if (_weapon != null)
        {
            _weapon.FireReleased();
        }
    }

    private void AltFirePressed()
    {
        if (_cardSelected != CardSelected.None)
        {
            _cardSelected = CardSelected.None;
        }
        else if (_weapon != null)
        {
            _weapon.AltFirePressed();
        }
    }

    private void AltFireReleased()
    {
        if (_weapon != null)
        {
            _weapon.AltFireReleased();
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
            if (_deck.Hand[(int)cardNum] != null) // is there a card in the hand slot?
            {
                _cardSelected = cardNum; // select the card
            }
        }
        else if (_cardSelected == cardNum) // deselect a card by pressing it's button again.
        {
            _cardSelected = CardSelected.None;
        }
        else // switch cards by pressing another card's button when a card is selected.
        {
            // switch the cards
            Card card = _deck.Hand[(int)_cardSelected];
            _deck.Hand[(int) _cardSelected] = _deck.Hand[(int)cardNum];
            _deck.Hand[(int)cardNum] = card;
            
            if (_deck.Hand[(int)cardNum] != null) // is there a card in the hand slot?
            {
                _cardSelected = CardSelected.Card1;
            }
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
    
    public void EquipRandomWeapon()
    {
        EquipWeapon(weapons[Random.Range(0, weapons.Length)]);
    }

    private void EquipWeapon(GameObject weapon)
    {
        if (_weapon != null)
        {
            _weapon.Drop();
        }
        _weaponTransform = Instantiate(weapon, hand).transform;
        _weapon = _weaponTransform.GetComponent<Weapon>();
        _weapon.SetPlayer(this);
        _weapon.SetRaycastOrigin(raycastOrigin);
        weaponText.text = _weapon.name;
    }

    // clears weapon variables, only meant to be called from Weapon.DropWeapon
    public void DropWeapon()
    {
        _weapon = null;
        _weaponTransform = null;
    }
}
