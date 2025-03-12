using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudentLibrary
{
    public class AuthenticationService
    {
        private string _usersFilePath;
        private List<User> _users;

        public AuthenticationService(string usersFilePath)
        {
            _usersFilePath = usersFilePath;
            LoadUsers();
        }

        private void LoadUsers()
        {
            _users = new List<User>();
            try
            {
                if (File.Exists(_usersFilePath))
                {
                    string[] lines = File.ReadAllLines(_usersFilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length == 2)
                        {
                            _users.Add(new User(parts[0].Trim(), parts[1].Trim()));
                        }
                    }
                }
                else
                {
                    // Создаем файл с пользователем по умолчанию
                    _users.Add(new User("admin", "admin"));
                    SaveUsers();
                }
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Ошибка загрузки пользователей", ex);
            }
        }

        private void SaveUsers()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (User user in _users)
                {
                    lines.Add($"{user.Username}:{user.Password}");
                }
                File.WriteAllLines(_usersFilePath, lines);
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Ошибка сохранения пользователей", ex);
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                User user = _users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    throw new AuthenticationException("Пользователь не найден");
                }

                if (user.Password != password)
                {
                    throw new AuthenticationException("Неверный пароль");
                }

                return true;
            }
            catch (AuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Ошибка при аутентификации", ex);
            }
        }
    }
}
