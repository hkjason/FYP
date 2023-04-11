using System;

[Serializable]
public class QuestionData
{
    public int questionId;
    public string question;
    public int questionType;
    public string answerA;
    public string answerB;
    public string answerC;
    public string answerD;
    public string correctAnswer;
    public string answer;
    public int exerciseId;
    public RecordData[] records;
}