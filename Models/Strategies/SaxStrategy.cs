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
                            string subjectTitle = reader.GetAttribute("title") ?? "";
                            string room = reader.GetAttribute("room") ?? "";
                            string building = reader.GetAttribute("building") ?? "";
                            string credits = reader.GetAttribute("credits") ?? "-";
                            string hours = reader.GetAttribute("hours") ?? "-";
                            string fullRoom = string.IsNullOrEmpty(building) ? room : $"{room} (к.{building})";

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
                            if (match)
                            {
                                string groups = "";
                                using (XmlReader subReader = reader.ReadSubtree())
                                {
                                    if (subReader.ReadToDescendant("Groups"))
                                    {
                                        groups = subReader.ReadElementContentAsString();
                                    }
                                }
                                if (!string.IsNullOrEmpty(criteria.Groups) &&
                                    !groups.Contains(criteria.Groups, StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }
                                results.Add(new SearchResult
                                {
                                    Faculty = currentFaculty,
                                    Department = currentDept,
                                    TeacherName = currentTeacher,
                                    Subject = subjectTitle,
                                    Room = fullRoom,
                                    Credits = credits,
                                    Hours = hours,
                                    Groups = groups
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