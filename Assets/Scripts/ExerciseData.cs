using System;

[Serializable]
public class ExerciseData
{
    public int exerciseId;
    public string exerciseName;
    public int difficulty;
    public string dueDate;
    public string scheduleDate;
    public string releaseDate;
    public string completionTime;
    public int questions;
    public int correctAnswers;
    public StudentsRecords[] studentRecords;
    public StudentsNotAnswered[] studentsNotAnswered;
}