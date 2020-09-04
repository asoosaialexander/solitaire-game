using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        Reset();
    }

    public void Reset()
    {
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach(var card in cards)
        {
            Destroy(card.gameObject);
        }

        Selectable[] selectables = FindObjectsOfType<Selectable>();
        foreach(var selectable in selectables)
        {
            if (selectable.CompareTag("Top"))
            {
                selectable.suit = null;
                selectable.value = 0;
            }
        }

        FindObjectOfType<Solitaire>().PlayCards();
    }
}
