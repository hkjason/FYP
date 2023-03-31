using System;

[Serializable]
public class CourseData
{
    public int courseId;
    public string courseName;
    public string connectionCode;
    public UserData teacher;
    public UserData[] students;
}