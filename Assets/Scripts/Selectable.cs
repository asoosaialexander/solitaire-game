using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool top = false;
    public string suit;
    public int value;
    public int row;

    public bool faceUp = false;
    public bool inDeckPile = false;

    private string valueString;
    private Dictionary<string, int> cardValueMap = GenerateCardValueMap();

    void Start()
    {
        {

            if (CompareTag("Card"))
            {
                suit = transform.name.Substring(0, 1);
                if (transform.name != "Card")
                {
                    value = cardValueMap[transform.name.Substring(1)];
                }
            }
        }
    }

    static Dictionary<string, int> GenerateCardValueMap()
    {
        return new Dictionary<string, int> {
            { "A", 1 },
            { "2", 2 },
            { "3", 3 },
            { "4", 4 },
            { "5", 5 },
            { "6", 6 },
            { "7", 7 },
            { "8", 8 },
            { "9", 9 },
            { "10", 10 },
            { "J", 11 },
            { "Q", 12 },
            { "K", 13 }
        };
    }
}
