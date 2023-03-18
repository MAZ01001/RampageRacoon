using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI scoreText; //Variable f�r Text UI Dingsbums nh
    private int score; //Score halt, gibt nix zu erkl�ren ja

    void Awake()
    {
        //nur eine Instanz erlaubt vom ScoreManager
        if (instance == null)
        {
            instance = this;
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Score addieren zu aktuellen Punkten
    public void AddScore(int points)
    {
        Debug.Log("davor: " + score);
        score += points;
        Debug.Log("danach: " + score);
        scoreText.text = "Score: " + score.ToString();
    }

    //Score bekommen
    public int GetScore()
    {
        return score;
    }
}