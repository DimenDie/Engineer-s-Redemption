using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public int score = 0;

    private GameObject hudScore;
    private void Start()
    {
        hudScore = GameObject.Find("HUDScore");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(NextLevelDelay());
    }

    private void Update()
    {
        hudScore.GetComponent<TextMeshProUGUI>().text = score.ToString("D8");
    }

    IEnumerator NextLevelDelay()
    {
        FindObjectOfType<AudioManager>().Play("Victory");
        yield return new WaitForSeconds(3);
        if (SceneManager.GetActiveScene().buildIndex == 3)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
