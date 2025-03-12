using System;
using System.Collections.Generic;
using System.IO;
using StudentLibrary;

namespace StudentManagementApp
{
    class Program
    {
        private static AuthenticationService _authService;
        private static DataService _dataService;
        private static string _logFilePath = "errors.log";
        private static string _currentUsername;

        static void Main(string[] args)
        {
            try
            {
                Initialize();

                if (Authenticate())
                {
                    Console.WriteLine($"Добро пожаловать, {_currentUsername}!");
                    ShowMenu();
                }
                else
                {
                    Console.WriteLine("Ошибка авторизации. Программа будет завершена.");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                Console.WriteLine("Произошла критическая ошибка. Подробности в файле логов.");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static void Initialize()
        {
            string usersFilePath = "users.txt";
            string dataFilePath = "students.dat";

            _authService = new AuthenticationService(usersFilePath);
            _dataService = new DataService(dataFilePath);
        }

        private static bool Authenticate()
        {
            try
            {
                Console.WriteLine("=== Авторизация пользователя ===");
                Console.Write("Имя пользователя: ");
                string username = Console.ReadLine();

                Console.Write("Пароль: ");
                string password = GetPassword();

                bool result = _authService.AuthenticateUser(username, password);
                if (result)
                {
                    _currentUsername = username;
                    _dataService.Initialize(username);
                }
                return result;
            }
            catch (AuthenticationException ex)
            {
                LogError(ex);
                Console.WriteLine($"Ошибка авторизации: {ex.Message}");
                return false;
            }
        }

        private static string GetPassword()
        {
            // Функция для скрытого ввода пароля
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        private static void ShowMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n=== Меню ===");
                Console.WriteLine("1. Вывести всех студентов");
                Console.WriteLine("2. Добавить нового студента");
                Console.WriteLine("3. Удалить студента");
                Console.WriteLine("4. Редактировать студента");
                Console.WriteLine("0. Выход");

                Console.Write("\nВыберите действие: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            DisplayStudents();
                            break;
                        case "2":
                            AddStudent();
                            break;
                        case "3":
                            DeleteStudent();
                            break;
                        case "4":
                            EditStudent();
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Некорректный выбор. Пожалуйста, выберите снова.");
                            break;
                    }
                }
                catch (DataOperationException ex)
                {
                    LogError(ex);
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    Console.WriteLine("Произошла ошибка. Подробности в файле логов.");
                }
            }
        }

        private static void DisplayStudents()
        {
            List<Student> students = _dataService.GetUserStudents();

            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст.");
                return;
            }

            Console.WriteLine("\n=== Список студентов ===");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i]}");
            }
        }

        private static void AddStudent()
        {
            Console.WriteLine("\n=== Добавление нового студента ===");

            Console.Write("ФИО: ");
            string fullName = Console.ReadLine();

            Console.Write("Группа: ");
            string group = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Рейтинг: ");
            if (!double.TryParse(Console.ReadLine(), out double rating))
            {
                throw new DataOperationException("Некорректное значение рейтинга");
            }

            _dataService.AddStudent(fullName, group, email, rating);
            Console.WriteLine("Студент успешно добавлен!");
        }

        private static void DeleteStudent()
        {
            List<Student> students = _dataService.GetUserStudents();

            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст. Нечего удалять.");
                return;
            }

            DisplayStudents();

            Console.Write("\nВведите номер студента для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int number) || number < 1 || number > students.Count)
            {
                throw new DataOperationException("Некорректный номер студента");
            }

            _dataService.DeleteStudent(number - 1);
            Console.WriteLine("Студент успешно удален!");
        }

        private static void EditStudent()
        {
            List<Student> students = _dataService.GetUserStudents();

            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст. Нечего редактировать.");
                return;
            }

            DisplayStudents();

            Console.Write("\nВведите номер студента для редактирования: ");
            if (!int.TryParse(Console.ReadLine(), out int number) || number < 1 || number > students.Count)
            {
                throw new DataOperationException("Некорректный номер студента");
            }

            Student selectedStudent = students[number - 1];

            Console.WriteLine("\n=== Редактирование студента ===");
            Console.WriteLine($"Текущие данные: {selectedStudent}");

            Console.Write($"ФИО [{selectedStudent.FullName}]: ");
            string fullName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fullName))
                fullName = selectedStudent.FullName;

            Console.Write($"Группа [{selectedStudent.Group}]: ");
            string group = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(group))
                group = selectedStudent.Group;

            Console.Write($"Email [{selectedStudent.Email}]: ");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
                email = selectedStudent.Email;

            Console.Write($"Рейтинг [{selectedStudent.Rating}]: ");
            string ratingInput = Console.ReadLine();
            double rating = selectedStudent.Rating;
            if (!string.IsNullOrWhiteSpace(ratingInput) && !double.TryParse(ratingInput, out rating))
            {
                throw new DataOperationException("Некорректное значение рейтинга");
            }

            _dataService.UpdateStudent(number - 1, fullName, group, email, rating);
            Console.WriteLine("Студент успешно отредактирован!");
        }

        private static void LogError(Exception ex)
        {
            try
            {
                string errorMessage = $"{DateTime.Now}: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}\n";
                File.AppendAllText(_logFilePath, errorMessage + "------------------------------\n");
            }
            catch
            {
                // В случае проблем с логированием, просто пропускаем
            }
        }
    }
}
