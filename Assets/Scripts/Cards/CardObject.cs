using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro description;
    [SerializeField] private TextMeshPro fact;
    [SerializeField] private TextMeshPro type;
    [SerializeField] private TextMeshPro tier;
    [SerializeField] private SpriteRenderer art;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer border;
    public Card card;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        UpdateCard();
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
}
