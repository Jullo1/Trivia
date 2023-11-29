using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    AudioSource audioSource;
    public GameObject endButtons;

    public GameObject resultOutput;
    public GameObject outputContainer;
    public GameObject buttonContainer;

    public Text timerOutput;
    public Text currentQuestionOutput;
    public Text scoreOutput;
    public Text scoreTextUI;

    public int score; //respuestasCorrectas * tiempo restante en c/u
    float gameTimer;
    int outputTime;
    int gameCount;
    int maxGameCount;
    bool gameOver;
    public int currentQuestionId = -1;

    public Text questionOutput;
    string[] questionList = new string[5];
    public Text[] answerOutput = new Text[4];
    string[,] answerList = new string[5, 4];
    int[] correctAnswerList = new int[5];

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        FillQuestionList();
    }

    void Start()
    {
        gameCount = 1;
        score = 0;
        maxGameCount = 5;
        gameTimer = 20;
        //scoreOutput.text = score.ToString();
        currentQuestionOutput.text = gameCount + " / " + maxGameCount;

        NextQuestion();
    }

    void Update()
    {
        UpdateUI();
    }

    public void GameOver()
    {
        score *= 8;
        scoreTextUI.gameObject.SetActive(true);
        scoreOutput.gameObject.SetActive(true);

        gameOver = true;
        endButtons.SetActive(true);
        gameCount--;
        RefreshUI();
        buttonContainer.gameObject.SetActive(false);
        StartCoroutine(SendScore(score));
    }

    public void AnswerQuestion(int selectedQuestion)
    {
        gameCount++;
        if (selectedQuestion == correctAnswerList[currentQuestionId])
            score += (int) gameTimer;
       
        if (currentQuestionId < questionList.Length-1)   NextQuestion();
        else GameOver();
    }

    public void NextQuestion()
    {
        currentQuestionId++;
        SetupQuestion();
        currentQuestionOutput.text = gameCount + " / " + maxGameCount;
    }

    void SetupQuestion()
    {
        questionOutput.text = questionList[currentQuestionId];

        for (int i = 0; i < answerOutput.Length; i++)
            answerOutput[i].text = answerList[currentQuestionId, i];

        gameTimer = 20;
        RefreshUI();
    }

    void OutputResult(string output, Color color)
    {
        Text outputText = resultOutput.GetComponent<Text>();
        outputText.text = output;
        outputText.color = color;
        Instantiate(resultOutput, outputContainer.transform);

    }

    void FillQuestionList()
    {
        questionList[0] = "What does HTML stand for?";
        questionList[1] = "Who is making the Web standards?";
        questionList[2] = "Choose the correct HTML element for the largest heading:";
        questionList[3] = "What is the correct HTML element for inserting a line break?";
        questionList[4] = "Choose the correct HTML element to define important text";

        answerList[0, 0] = "Hyperlinks and Text Markup Language";
        answerList[0, 1] = "Home Tool Markup Language";
        answerList[0, 2] = "Hyper Text Markup Language";
        answerList[0, 3] = "All are correct";
        correctAnswerList[0] = 3;

        answerList[1, 0] = "The World Wide Web Consortium";
        answerList[1, 1] = "Mozilla";
        answerList[1, 2] = "Google";
        answerList[1, 3] = "Microsoft";
        correctAnswerList[1] = 1;

        answerList[2, 0] = "<heading>";
        answerList[2, 1] = "<h6>";
        answerList[2, 2] = "<h1>";
        answerList[2, 3] = "<head>";
        correctAnswerList[2] = 3;

        answerList[3, 0] = "<br>";
        answerList[3, 1] = "<lb>";
        answerList[3, 2] = "<break>";
        answerList[3, 3] = "The 3 tags can be used as line breaks";
        correctAnswerList[3] = 1;

        answerList[4, 0] = "<i>";
        answerList[4, 1] = "<strong>";
        answerList[4, 2] = "<important>";
        answerList[4, 3] = "<b>";
        correctAnswerList[4] = 2;
    }

    void UpdateUI() //timer stuff
    {
        if (gameTimer < 0f) AnswerQuestion(0);

        if (!gameOver)  gameTimer -= Time.deltaTime;
        outputTime = (int)gameTimer;
        timerOutput.text = outputTime.ToString();
    }

    void RefreshUI() //manual UI update
    {
        currentQuestionOutput.text = gameCount + " / " + maxGameCount;
        scoreOutput.text = score.ToString();
    }

    IEnumerator SendScore(int value)
    {
        WWWForm form = new WWWForm();
        form.AddField("game", "Trivia");
        form.AddField("score", score);

        WWW www = new WWW("https://julianlerej.com/app/views/sendScore.php", form);
        yield return www;
    }
}