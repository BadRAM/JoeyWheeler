using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    [SerializeField] private float worldSize = 0.1f;
    [SerializeField] private float handSize = 0.05f;
    [SerializeField] private float handAnimSpeed = 1f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float floatHeight;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro description;
    [SerializeField] private TextMeshPro fact;
    [SerializeField] private TextMeshPro type;
    [SerializeField] private TextMeshPro tier;
    [SerializeField] private SpriteRenderer art;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private Card card;
    [HideInInspector] public Player player;
    [HideInInspector] public Player.CardSlot cardSlot;

    private bool _discarded; // when set to true, card will self terminate upon reaching destination.


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized, 
                transform.right, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 
                Mathf.Infinity), Vector3.up);

            RaycastHit hit;
            if (!Physics.Raycast(transform.position, Vector3.down, out hit, fallSpeed * Time.deltaTime + floatHeight, LayerMask.GetMask("Terrain")))
            {
                transform.position += fallSpeed * Time.deltaTime * Vector3.down; 
            }
            else
            {
                transform.position = hit.point + Vector3.up * floatHeight;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, player.GetCardModelTarget(cardSlot).position, Time.time * handAnimSpeed);
            transform.position = Vector3.MoveTowards(transform.position, player.GetCardModelTarget(cardSlot).position, Time.time * handAnimSpeed * 0.01f);
            
            transform.localScale = Vector3.Lerp(transform.localScale, handSize * Vector3.one, Time.time * handAnimSpeed);
            transform.localScale = Vector3.MoveTowards(transform.localScale, handSize * Vector3.one, Time.time * handAnimSpeed * 0.01f);
            if (transform.position == player.GetCardModelTarget(cardSlot).position)
            {
                transform.parent = player.GetCardModelTarget(cardSlot);
                if (_discarded)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, player.GetCardModelTarget(cardSlot).rotation, Time.time * handAnimSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, player.GetCardModelTarget(cardSlot).rotation, Time.time * handAnimSpeed * 0.01f);
        }
    }

    private void OnValidate()
    {
        UpdateCard();
    }

    public void Pickup(Player user)
    {
        player.PickupCard(this);
    }

    public void UpdateCard()
    {
        nameText.text = card.name;
        description.text = card.Description;
        fact.text = card.Fact;
        type.text = card.Type.ToString();
        tier.text = card.Tier.ToString();
        art.sprite = card.Art;
        
        // TODO: Set background and border to reflect type and tier.
    }

    public void SetSlot(Player.CardSlot slot)
    {
        cardSlot = slot;
    }

    public void SetCard(Card cardToSet)
    {
        card = cardToSet;
        UpdateCard();
    }

    public void SetPlayer(Player playerToSet)
    {
        player = playerToSet;
    }

    public void BeSmall()
    {
        transform.localScale = handSize * Vector3.one;
    }

    public void Drop()
    {
        transform.localScale = Vector3.one * worldSize;
    }

    public void Discard()
    {
        _discarded = true;
    }

    public Card GetCard()
    {
        return card;
    }
}
