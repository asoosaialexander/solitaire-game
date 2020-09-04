using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCard : MonoBehaviour
{
    public Selectable[] topStacks;
    public GameObject highScorePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HasWon())
        {
            highScorePanel.SetActive(true);
            Debug.Log("You have won");
        }
    }

    public bool HasWon()
    {
        int i = 0;
        foreach(var topStack in topStacks)
        {
            i += topStack.value;
        }

        return i >= 52 ? true : false;
    }
}
