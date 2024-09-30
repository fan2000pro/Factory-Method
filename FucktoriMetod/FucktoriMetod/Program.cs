using System;
using System.Collections.Generic;
using System.IO;

public class Teacher
{
    public int Id { get; }
    public string Name { get; }
    public int Experience { get; }
    public List<int> Courses { get; }

    public Teacher(int id, string name, int experience)
    {
        Id = id;
        Name = name;
        Experience = experience;
        Courses = new List<int>();
    }

    public override string ToString()
    {
        return $"Teacher(Id={Id}, Name='{Name}', Experience={Experience}, Courses=[{string.Join(", ", Courses)}])";
    }
}

public class Student
{
    public int Id { get; }
    public string Name { get; }
    public List<int> Courses { get; }

    public Student(int id, string name)
    {
        Id = id;
        Name = name;
        Courses = new List<int>();
    }

    public override string ToString()
    {
        return $"Student(Id={Id}, Name='{Name}', Courses=[{string.Join(", ", Courses)}])";
    }
}

public class Course
{
    public int Id { get; }
    public string Name { get; }
    public int TeacherId { get; }
    public List<int> Students { get; }

    public Course(int id, string name, int teacherId)
    {
        Id = id;
        Name = name;
        TeacherId = teacherId;
        Students = new List<int>();
    }

    public override string ToString()
    {
        return $"Course(Id={Id}, Name='{Name}', TeacherId={TeacherId}, Students=[{string.Join(", ", Students)}])";
    }
}

public class EntityFactory
{
    public Student CreateStudent(int id, string name)
    {
        return new Student(id, name);
    }

    public Teacher CreateTeacher(int id, string name, int experience)
    {
        return new Teacher(id, name, experience);
    }

    public Course CreateCourse(int id, string name, int teacherId)
    {
        return new Course(id, name, teacherId);
    }
}

public class SchoolDatabase
{
    public Dictionary<int, Student> students = new Dictionary<int, Student>();
    public Dictionary<int, Teacher> teachers = new Dictionary<int, Teacher>();
    public Dictionary<int, Course> courses = new Dictionary<int, Course>();
    private EntityFactory factory = new EntityFactory();

    public Student AddStudent(int id, string name)
    {
        var student = factory.CreateStudent(id, name);
        students[id] = student;
        return student;
    }

    public Teacher AddTeacher(int id, string name, int experience)
    {
        var teacher = factory.CreateTeacher(id, name, experience);
        teachers[id] = teacher;
        return teacher;
    }

    public Course AddCourse(int id, string name, int teacherId)
    {
        var course = factory.CreateCourse(id, name, teacherId);
        courses[id] = course;
        return course;
    }

    public void EnrollStudentInCourse(int studentId, int courseId)
    {
        if (students.TryGetValue(studentId, out var student) && courses.TryGetValue(courseId, out var course))
        {
            student.Courses.Add(courseId);
            course.Students.Add(studentId);
        }
    }

    public void SaveToFile(string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            foreach (var student in students.Values)
            {
                writer.WriteLine($"student {student.Id} {student.Name}");
            }
            foreach (var teacher in teachers.Values)
            {
                writer.WriteLine($"teacher {teacher.Id} {teacher.Name} {teacher.Experience}");
            }
            foreach (var course in courses.Values)
            {
                writer.WriteLine($"course {course.Id} {course.Name} {course.TeacherId} {string.Join(" ", course.Students)}");
            }
        }
    }

    public void LoadFromFile(string filename)
    {
        using (var reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                var entityType = parts[0];
                if (entityType == "student")
                {
                    int id = int.Parse(parts[1]);
                    string name = parts[2];
                    AddStudent(id, name);
                }
                else if (entityType == "teacher")
                {
                    int id = int.Parse(parts[1]);
                    string name = parts[2];
                    int experience = int.Parse(parts[3]);
                    AddTeacher(id, name, experience);
                }
                else if (entityType == "course")
                {
                    int id = int.Parse(parts[1]);
                    string name = parts[2];
                    int teacherId = int.Parse(parts[3]);
                    var course = AddCourse(id, name, teacherId);
                    for (int i = 4; i < parts.Length; i++)
                    {
                        int studentId = int.Parse(parts[i]);
                        EnrollStudentInCourse(studentId, id);
                    }
                }
            }
        }
    }
}


public class Program
{
    public static void Main()
    {
        var db = new SchoolDatabase();
        db.AddTeacher(1, "Alecsandro", 10);
        db.AddStudent(2, "Dennis");
        db.AddStudent(3, "Jeff");
        db.AddStudent(4, "Anton");
        db.AddCourse(1, "Math", 1);
        db.EnrollStudentInCourse(1, 1);
        db.EnrollStudentInCourse(2, 1);
        db.EnrollStudentInCourse(3, 1);
        db.EnrollStudentInCourse(4, 1);

        db.SaveToFile("data.txt");

        var db2 = new SchoolDatabase();
        db2.LoadFromFile("data.txt");
        Console.WriteLine(string.Join("\n", db2.students.Values));
        Console.WriteLine(string.Join("\n", db2.teachers.Values));
        Console.WriteLine(string.Join("\n", db2.courses.Values));
    }
}