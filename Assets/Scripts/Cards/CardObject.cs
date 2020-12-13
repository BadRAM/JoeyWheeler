using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private float handAnimSpeed = 1f;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI fact;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI tier;
    [SerializeField] private Image art;
    [SerializeField] private Image background;
    [SerializeField] private Image border;
    [SerializeField] private TierArt tierArt;
    [HideInInspector] public Player player;
    public Player.CardSlot cardSlot;

    private bool _discarded; // when set to true, card will self terminate upon reaching destination.
    private float _animStart;

    // Start is called before the first frame update
    void Start()
    {
        SetCard(card);
    }

    // Update is called once per frame
    void Update()
    {
        Transform modelTarget = player.GetCardModelTarget(cardSlot);
        
        transform.position = Vector3.Lerp(transform.position, modelTarget.position, Time.deltaTime * handAnimSpeed);
        transform.position = Vector3.MoveTowards(transform.position, modelTarget.position, Time.deltaTime * handAnimSpeed/2);
        
        transform.localScale = Vector3.Lerp(transform.localScale,  modelTarget.localScale, Time.deltaTime * handAnimSpeed);
        transform.localScale = Vector3.MoveTowards(transform.localScale, modelTarget.localScale, Time.deltaTime * handAnimSpeed/2);

        transform.rotation = Quaternion.Lerp(transform.rotation, modelTarget.rotation, Time.deltaTime * handAnimSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, modelTarget.rotation, Time.deltaTime * handAnimSpeed/2);
        
        if (_discarded && transform.position == modelTarget.position)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnValidate()
    {
        UpdateCard();
    }

    public void UpdateCard()
    {
        nameText.text = card.Name;
        description.text = card.Description;
        fact.text = card.Fact;
        type.text = card.Type.ToString();
        tier.text = card.Tier.ToString();
        art.sprite = card.Art;
        background.sprite = tierArt.GetTypeBackground(card.Type);
        border.sprite = tierArt.GetTierBorder(card.Tier);
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

    public void Discard()
    {
        _discarded = true;
    }

    public Card GetCard()
    {
        return card;
    }

    public string GetName()
    {
        return card.Name;
    }
}