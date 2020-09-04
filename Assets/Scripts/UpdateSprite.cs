using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFront;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Solitaire solitaire;
    private UserInput userInput;

    void Start()
    {
        List<string> deck = Solitaire.GenerateDeck();
        solitaire = FindObjectOfType<Solitaire>();
        userInput = FindObjectOfType<UserInput>();

        int i = 0;
        foreach (string card in deck)
        {
            if (name == card)
            {
                cardFront = solitaire.cardFaces[i];
                break;
            }
            i++;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
    }

    void Update()
    {
        spriteRenderer.sprite = selectable.faceUp ? cardFront : cardBack;

        if (userInput.slot1)
        {
            spriteRenderer.color = name == userInput.slot1.name ? Color.yellow : Color.white;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
