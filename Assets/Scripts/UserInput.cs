using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitaire solitaire;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
    }

    void Update()
    {
        //Logic to get double click time
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }
        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                if (hit.collider.CompareTag("Deck"))
                {
                    Deck();
                }
                if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                if (hit.collider.CompareTag("Top"))
                {
                    Top(hit.collider.gameObject);
                }
                if (hit.collider.CompareTag("Bottom"))
                {
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    private void Card(GameObject selectedCard)
    {
        //If the card is facedown and not blocked, then turn and display card
        if (!selectedCard.GetComponent<Selectable>().faceUp && Blocked(selectedCard) == false)
        {
            selectedCard.GetComponent<Selectable>().faceUp = true;
            slot1 = null;
            return;
        }


        if (!slot1)
        {
            //If the card is in the deck trips and not blocked, then select card
            if (selectedCard.GetComponent<Selectable>().inDeckPile && !Blocked(selectedCard))
            {
                slot1 = selectedCard;
            }
            //If the top card is selected
            if (selectedCard.GetComponent<Selectable>().top)
            {
                slot1 = selectedCard;
            }
            //If the bottom card is selected
            if (selectedCard.name == solitaire.bottoms[selectedCard.GetComponent<Selectable>().row].Last())
            {
                slot1 = selectedCard;
            }
        }
        else
        {
            if (!Blocked(selectedCard))
            {
                if (slot1 != selectedCard)
                {
                    if (Stackable(selectedCard))
                    {
                        Stack(selectedCard);
                    }
                    else
                    {
                        slot1 = selectedCard;
                    }
                }
                else //If same card is selected twice, then double click
                {
                    if (DoubleClick())
                    {
                        AutoStack(selectedCard);
                    }
                }
            }
        }
    }

    private void Deck()
    {
        print("deck clicked");
        solitaire.DealFromDeck();
    }

    private void Top(GameObject selectedCard)
    {
        print("top clicked");
        if (slot1)
        {
            if (slot1.CompareTag("Card") && slot1.GetComponent<Selectable>().value == 1)
            {
                Stack(selectedCard);
            }
        }
    }

    private void Bottom(GameObject selectedCard)
    {
        print("bottom clicked");
        if (slot1)
        {
            if (slot1.CompareTag("Card") && slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selectedCard);
            }
        }
    }

    private bool Stackable(GameObject selectedCard)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selectedCard.GetComponent<Selectable>();

        if (!s2.inDeckPile)
        {
            if (s2.top) //Top pile Ace to King
            {
                if ((s1.suit == s2.suit && s1.value == s2.value + 1) || (s1.value == 1 && s2.suit is null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //Bottom pile alternate color King to Ace
            {
                if (s1.value == s2.value - 1)
                {
                    string s1SuitColor = (s1.suit == "C" || s1.suit == "S") ? "black" : "red";
                    string s2SuitColor = (s2.suit == "C" || s2.suit == "S") ? "black" : "red";

                    if (s1SuitColor != s2SuitColor)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        return false;
    }

    void Stack(GameObject selectedCard)
    {
        Selectable sourceCard = slot1.GetComponent<Selectable>();
        Selectable targetCard = selectedCard.GetComponent<Selectable>();

        float yOffset = 0.3f;
        float zOffset = 0.01f;

        if (targetCard.top || (!targetCard.top && sourceCard.value == 13))
        {
            yOffset = 0f;
            zOffset = sourceCard.value * zOffset;
        }

        Debug.Log("Z position: "+selectedCard.transform.position.z);
        slot1.transform.position = new Vector3(
            selectedCard.transform.position.x,
            selectedCard.transform.position.y - yOffset,
            selectedCard.transform.position.z - zOffset);

        if (selectedCard.CompareTag("Card"))
        {
            slot1.transform.parent = selectedCard.transform.parent;
        }
        else
        {
            slot1.transform.parent = selectedCard.transform;
        }
        //If unblocked card in trip is moved
        if (sourceCard.inDeckPile)
        {
            solitaire.deckCardsOnDisplay.Remove(slot1.name);
        }
        //If Ace is moved from top pile
        else if (sourceCard.top && targetCard.top && sourceCard.value == 1)
        {
            solitaire.topPos[sourceCard.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[sourceCard.row].GetComponent<Selectable>().suit = null;
        }
        //If cards other than Ace is moved from top pile
        else if (sourceCard.top)
        {
            solitaire.topPos[sourceCard.row].GetComponent<Selectable>().value = sourceCard.value - 1;
        }
        else
        {
            solitaire.bottoms[sourceCard.row].Remove(slot1.name);
        }

        sourceCard.inDeckPile = false;
        sourceCard.row = targetCard.row;

        //If the card is placed on top
        if (targetCard.top)
        {
            solitaire.topPos[sourceCard.row].GetComponent<Selectable>().value = sourceCard.value;
            solitaire.topPos[sourceCard.row].GetComponent<Selectable>().suit = sourceCard.suit;
            sourceCard.top = true;
        }
        else
        {
            sourceCard.top = false;
            solitaire.bottoms[targetCard.row].Add(slot1.name);
        }

        slot1 = null;
    }

    bool Blocked(GameObject selectedCard)
    {
        Selectable s2 = selectedCard.GetComponent<Selectable>();

        if (s2.top) return false;

        if (s2.inDeckPile)
        {
            //Card is unblocked if last display card from Deck
            return s2.name == solitaire.deckCardsOnDisplay.Last() ? false : true;
        }
        else
        {
            //Card is unblocked if last card in the bottom piles
            return s2.name == solitaire.bottoms[s2.row].Last() ? false : true;
        }
    }

    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            Debug.Log("Double Clicked");
            return true;
        }
        return false;
    }

    void AutoStack(GameObject selectedCard)
    {
        for (int i = solitaire.topPos.Length - 1; i >= 0; i--)
        {
            Selectable selectedTop = solitaire.topPos[i].GetComponent<Selectable>();
            if (selectedCard.GetComponent<Selectable>().value == 1 && selectedTop.value == 0)
            {
                slot1 = selectedCard;
                Stack(selectedTop.gameObject);
                break;
            }
            if (selectedCard.GetComponent<Selectable>().suit == selectedTop.suit &&
                selectedCard.GetComponent<Selectable>().value == selectedTop.value + 1)
            {
                slot1 = selectedCard;
                Stack(selectedTop.gameObject);
                break;
            }
        }
    }
}
