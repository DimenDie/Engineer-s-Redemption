using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesBar : MonoBehaviour
{
    private Transform[] hearts = new Transform[12];

    private Character character;
    public Boss boss;
    private void Awake()
    {
        character = FindObjectOfType<Character>();
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = transform.GetChild(i);
        }
    }


    public void RefreshCharacter()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < character.Health) 
                hearts[i].gameObject.SetActive(true);
            else 
                hearts[i].gameObject.SetActive(false);
        }
    }

    public void RefreshBoss()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < boss.Health)
                hearts[i].gameObject.SetActive(true);
            else
                hearts[i].gameObject.SetActive(false);
        }
    }
}
