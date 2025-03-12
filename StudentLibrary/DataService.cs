using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace StudentLibrary
{
    public class DataService
    {
        private string _dataFilePath;
        private List<Student> _allStudents;
        private string _currentUsername;

        public DataService(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
            _allStudents = new List<Student>();
        }

        public void Initialize(string username)
        {
            _currentUsername = username;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    using (FileStream fs = new FileStream(_dataFilePath, FileMode.Open))
                    {
                        if (fs.Length > 0)
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            _allStudents = (List<Student>)formatter.Deserialize(fs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataOperationException("Ошибка загрузки данных", ex);
            }
        }

        private void SaveData()
        {
            try
            {
                using (FileStream fs = new FileStream(_dataFilePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, _allStudents);
                }
            }
            catch (Exception ex)
            {
                throw new DataOperationException("Ошибка сохранения данных", ex);
            }
        }

        public List<Student> GetUserStudents()
        {
            try
            {
                return _allStudents.Where(s => s.Owner == _currentUsername).ToList();
            }
            catch (Exception ex)
            {
                throw new DataOperationException("Ошибка при получении данных пользователя", ex);
            }
        }

        public void AddStudent(string fullName, string group, string email, double rating)
        {
            try
            {
                _allStudents.Add(new Student(fullName, group, email, rating, _currentUsername));
                SaveData();
            }
            catch (Exception ex)
            {
                throw new DataOperationException("Ошибка при добавлении студента", ex);
            }
        }

        public void DeleteStudent(int index)
        {
            try
            {
                List<Student> userStudents = GetUserStudents();
                if (index < 0 || index >= userStudents.Count)
                {
                    throw new DataOperationException("Некорректный индекс для удаления");
                }

                Student studentToDelete = userStudents[index];
                _allStudents.Remove(studentToDelete);
                SaveData();
            }
            catch (DataOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataOperationException("Ошибка при удалении студента", ex);
            }
        }

        public void UpdateStudent(int index, string fullName, string group, string email, double rating)
        {
            try
            {
                List<Student> userStudents = GetUserStudents();
                if (index < 0 || index >= userStudents.Count)
                {
                    throw new DataOperationException("Некорректный индекс для редактирования");
                }

                Student studentToUpdate = userStudents[index];
                studentToUpdate.FullName = fullName;
                studentToUpdate.Group = group;
                studentToUpdate.Email = email;
                studentToUpdate.Rating = rating;

                SaveData();
            }
            catch (DataOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataOperationException("Ошибка при редактировании студента", ex);
            }
        }
    }
}
