using System;

namespace StudentLibrary
{
    [Serializable]
    public class Student
    {
        public string FullName { get; set; }
        public string Group { get; set; }
        public string Email { get; set; }
        public double Rating { get; set; }
        public string Owner { get; set; } // Имя пользователя-владельца записи

        public Student() { }

        public Student(string fullName, string group, string email, double rating, string owner)
        {
            FullName = fullName;
            Group = group;
            Email = email;
            Rating = rating;
            Owner = owner;
        }

        public override string ToString()
        {
            return $"ФИО: {FullName}, Группа: {Group}, Email: {Email}, Рейтинг: {Rating}";
        }
    }
}
