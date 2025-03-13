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
                ShowStartMenu();
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

        private static void ShowStartMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Система учета студентов ===");
                Console.WriteLine("1. Вход в систему");
                Console.WriteLine("2. Регистрация нового пользователя");
                Console.WriteLine("0. Выход");

                Console.Write("\nВыберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (Authenticate())
                        {
                            Console.WriteLine($"Добро пожаловать, {_currentUsername}!");
                            ShowMainMenu();
                        }
                        break;
                    case "2":
                        RegisterUser();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор. Пожалуйста, выберите снова.");
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static bool Authenticate()
        {
            try
            {
                Console.Clear();
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
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return false;
            }
        }

        private static void RegisterUser()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Регистрация нового пользователя ===");
                Console.Write("Имя пользователя: ");
                string username = Console.ReadLine();

                Console.Write("Пароль: ");
                string password = GetPassword();

                Console.Write("Подтверждение пароля: ");
                string confirmPassword = GetPassword();

                if (password != confirmPassword)
                {
                    Console.WriteLine("Ошибка: Пароли не совпадают.");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    return;
                }

                _authService.RegisterUser(username, password);
                Console.WriteLine("Пользователь успешно зарегистрирован!");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
            catch (AuthenticationException ex)
            {
                LogError(ex);
                Console.WriteLine($"Ошибка регистрации: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void DeleteAccount()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Удаление учетной записи ===");
                Console.WriteLine("ВНИМАНИЕ: Это действие удалит вашу учетную запись!");
                Console.Write("Для подтверждения введите ваш пароль: ");
                string password = GetPassword();

                Console.Write("Вы уверены? (да/нет): ");
                string confirmation = Console.ReadLine().ToLower();

                if (confirmation != "да")
                {
                    Console.WriteLine("Удаление отменено.");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    return;
                }

                _authService.DeleteUser(_currentUsername, password);
                Console.WriteLine("Учетная запись успешно удалена.");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();

                // Возврат в стартовое меню
                _currentUsername = null;
            }
            catch (AuthenticationException ex)
            {
                LogError(ex);
                Console.WriteLine($"Ошибка удаления: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
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

        private static void ShowMainMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine($"=== Меню пользователя: {_currentUsername} ===");
                Console.WriteLine("1. Вывести всех студентов");
                Console.WriteLine("2. Добавить нового студента");
                Console.WriteLine("3. Удалить студента");
                Console.WriteLine("4. Редактировать студента");
                Console.WriteLine("5. Удалить мою учетную запись");
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
                        case "5":
                            DeleteAccount();
                            if (_currentUsername == null) // Если учетная запись успешно удалена
                                return; // Выходим в главное меню
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Некорректный выбор. Пожалуйста, выберите снова.");
                            Console.WriteLine("Нажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (DataOperationException ex)
                {
                    LogError(ex);
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    Console.WriteLine("Произошла ошибка. Подробности в файле логов.");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        private static void DisplayStudents()
        {
            List<Student> students = _dataService.GetUserStudents();

            Console.Clear();
            Console.WriteLine("=== Список студентов ===");

            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст.");
            }
            else
            {
                for (int i = 0; i < students.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {students[i]}");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void AddStudent()
        {
            Console.Clear();
            Console.WriteLine("=== Добавление нового студента ===");

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

            if (rating < 0 || rating > 100)
            {
                throw new DataOperationException("Рейтинг должен быть от 0 до 100");
            }

            _dataService.AddStudent(fullName, group, email, rating);
            Console.WriteLine("Студент успешно добавлен!");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void DeleteStudent()
        {
            List<Student> students = _dataService.GetUserStudents();

            Console.Clear();
            Console.WriteLine("=== Удаление студента ===");

            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст. Нечего удалять.");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i]}");
            }

            Console.Write("\nВведите номер студента для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int number) || number < 1 || number > students.Count)
            {
                throw new DataOperationException("Некорректный номер студента");
            }

            _dataService.DeleteStudent(number - 1);
            Console.WriteLine("Студент успешно удален!");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void EditStudent()
        {
            List<Student> students = _dataService.GetUserStudents();

            Console.Clear();
            Console.WriteLine("=== Редактирование студента ===");

            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст. Нечего редактировать.");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i]}");
            }

            Console.Write("\nВведите номер студента для редактирования: ");
            if (!int.TryParse(Console.ReadLine(), out int number) || number < 1 || number > students.Count)
            {
                throw new DataOperationException("Некорректный номер студента");
            }

            Student selectedStudent = students[number - 1];

            Console.WriteLine($"\nТекущие данные: {selectedStudent}");

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
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
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