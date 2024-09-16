using System;
using System.Collections.Generic;
using System.IO;

public abstract class Entity
{
    public int Id { get; set; }
}
public class Student : Entity
{
    public string Name { get; set; }
    public List<Course> Courses { get; set; }
    public Student(int id, string name)
    {
        Id = id;
        Name = name;
        Courses = new List<Course>();
    }
    internal void Add(Student? student)
    {
        throw new NotImplementedException();
    }
}
public class Teacher : Entity
{
    public string Name { get; set; }
    public int Experience { get; set; }
    public List<Course> Courses { get; set; }
    public Teacher(int id, string name, int experience)
    {
        Id = id;
        Name = name;
        Experience = experience;
        Courses = new List<Course>();
    }
}
public class Course : Entity
{
    public string Name { get; set; }
    public int TeacherId { get; set; }
    public List<int> StudentIds { get; set; }
    public Teacher? Teacher { get; internal set; }
    public Student? Student { get; set; }
    public IEnumerable<Student> Students { get; internal set; }
    public Course(int id, string name, int teacherId)
    {
        Id = id;
        Name = name;
        TeacherId = teacherId;
        StudentIds = new List<int>();
    }
}
public abstract class EntityFactory
{
    public abstract Entity CreateEntity(string[] data);
}
public class StudentFactory : EntityFactory
{
    public override Entity CreateEntity(string[] data)
    {
        int id = int.Parse(data[1]);
        string name = data[2];
        return new Student(id, name);
    }
}
public class TeacherFactory : EntityFactory
{
    public override Entity CreateEntity(string[] data)
    {
        int id = int.Parse(data[1]);
        string name = data[2];
        int experience = int.Parse(data[3]);
        return new Teacher(id, name, experience);
    }
}
public class CourseFactory : EntityFactory
{
    public override Entity CreateEntity(string[] data)
    {
        int id = int.Parse(data[1]);
        string name = data[2];
        int teacherId = int.Parse(data[3]);
        return new Course(id, name, teacherId);
    }
}
public class Database
{
    public List<Student> students;
    private List<Teacher> teachers;
    private List<Course> courses;
    public Database()
    {
        students = new List<Student>();
        teachers = new List<Teacher>();
        courses = new List<Course>();
    }
    public void LoadFromFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            string entityType = data[0];
            EntityFactory factory;
            switch (entityType)
            {
                case "student":
                    factory = new StudentFactory();
                    break;
                case "teacher":
                    factory = new TeacherFactory();
                    break;
                case "course":
                    factory = new CourseFactory();
                    break;
                default:
                    throw new Exception("Unknown entity type");
            }
            Entity entity = factory.CreateEntity(data);
            switch (entityType)
            {
                case "student":
                    students.Add((Student)entity);
                    break;
                case "teacher":
                    teachers.Add((Teacher)entity);
                    break;
                case "course":
                    courses.Add((Course)entity);
                    break;
            }
        }
        foreach (Course course in courses)
        {
            Teacher teacher = teachers.Find(t => t.Id == course.TeacherId);
            course.Teacher = teacher;
            foreach (int studentId in course.StudentIds)
            {
                Student student = students.Find(s => s.Id == studentId);
                course.Student.Add(student);
                student.Courses.Add(course);
            }
        }
    }
    public void SaveToFile(string filePath)
    {
        List<string> lines = new List<string>();
        foreach (Student student in students)
        {
            lines.Add($"student,{student.Id},{student.Name}");
        }
        foreach (Teacher teacher in teachers)
        {
            lines.Add($"teacher,{teacher.Id},{teacher.Name},{teacher.Experience}");
        }
        foreach (Course course in courses)
        {
            lines.Add($"course,{course.Id},{course.Name},{course.TeacherId}");
            foreach (Student student in course.Students)
            {
                lines.Add($"course,{course.Id},{student.Id}");
            }
        }
        File.WriteAllLines(filePath, lines);
    }
}
class Program
{
    static void Main()
    {
        Database db = new Database();
        db.LoadFromFile("data.txt");
        List<Student> list = db.students;
        for (int i = 0; i < list.Count; i++)
        {
            Student student = list[i];
            Console.WriteLine($"Student {student.Name} is taking courses:");
            foreach (Course course in student.Courses)
            {
                Console.WriteLine($"  {course.Name}");
            }
        }
        db.SaveToFile("data.txt");
    }
}