using System.Collections.Generic;
using System.Xml;
using OOP_Lab2.Models;

namespace OOP_Lab2.Strategies
{
    public class SaxStrategy : ISearchStrategy
    {
        public List<SearchResult> Search(SearchResult criteria, string filePath)
        {
            List<SearchResult> results = new List<SearchResult>();

            if (!System.IO.File.Exists(filePath)) return results;

            string currentFaculty = "";
            string currentDept = "";
            string currentTeacher = "";

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "Faculty")
                        {
                            currentFaculty = reader.GetAttribute("name") ?? "";
                        }
                        else if (reader.Name == "Department")
                        {
                            currentDept = reader.GetAttribute("name") ?? "";
                        }
                        else if (reader.Name == "Teacher")
                        {
                            currentTeacher = reader.GetAttribute("name") ?? "";
                        }
                        else if (reader.Name == "Subject")
                        {
                            // 1. Читаємо нові атрибути (Учбовий план та корпус)
                            string subjectTitle = reader.GetAttribute("title") ?? "";
                            string room = reader.GetAttribute("room") ?? "";
                            string building = reader.GetAttribute("building") ?? ""; // Новий атрибут
                            string credits = reader.GetAttribute("credits") ?? "-";  // Новий атрибут
                            string hours = reader.GetAttribute("hours") ?? "-";      // Новий атрибут

                            // Формуємо гарний рядок аудиторії (наприклад: "305 (к.18)")
                            string fullRoom = string.IsNullOrEmpty(building) ? room : $"{room} (к.{building})";

                            // --- ОНОВЛЕНА ФІЛЬТРАЦІЯ SAX ---
                            bool match = true;

                            if (!string.IsNullOrEmpty(criteria.Faculty) &&
                                !currentFaculty.Contains(criteria.Faculty, StringComparison.OrdinalIgnoreCase)) match = false;

                            if (!string.IsNullOrEmpty(criteria.Department) &&
                                !currentDept.Contains(criteria.Department, StringComparison.OrdinalIgnoreCase)) match = false;

                            if (!string.IsNullOrEmpty(criteria.TeacherName) &&
                                !currentTeacher.Contains(criteria.TeacherName, StringComparison.OrdinalIgnoreCase)) match = false;

                            if (!string.IsNullOrEmpty(criteria.Subject) &&
                                !subjectTitle.Contains(criteria.Subject, StringComparison.OrdinalIgnoreCase)) match = false;

                            if (!string.IsNullOrEmpty(criteria.Room) &&
                                !fullRoom.Contains(criteria.Room, StringComparison.OrdinalIgnoreCase)) match = false;

                            // 3. Якщо запис підходить - треба дістати <Groups>
                            if (match)
                            {
                                string groups = "";

                                // НЮАНС SAX: Groups - це вкладений тег.
                                // Ми створюємо "під-читач" (Subtree), який пробіжить тільки всередині поточного <Subject>
                                // Це найбезпечніший спосіб не зламати основний цикл while.
                                using (XmlReader subReader = reader.ReadSubtree())
                                {
                                    // Шукаємо тег Groups всередині Subject
                                    if (subReader.ReadToDescendant("Groups"))
                                    {
                                        // Читаємо текст всередині тегів <Groups>...</Groups>
                                        groups = subReader.ReadElementContentAsString();
                                    }
                                }
                                if (!string.IsNullOrEmpty(criteria.Groups) &&
                                    !groups.Contains(criteria.Groups, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Якщо група не підходить - не додаємо і виходимо з цього Subject
                                    continue; // або просто не викликаємо results.Add
                                }
                                // ---
                                // Додаємо повний результат
                                results.Add(new SearchResult
                                {
                                    Faculty = currentFaculty,
                                    Department = currentDept,
                                    TeacherName = currentTeacher,
                                    Subject = subjectTitle,
                                    Room = fullRoom,
                                    Credits = credits, // Додали
                                    Hours = hours,     // Додали
                                    Groups = groups    // Додали
                                });
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}