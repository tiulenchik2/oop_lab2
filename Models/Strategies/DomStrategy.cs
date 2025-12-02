using System.Collections.Generic;
using System.Xml;
using OOP_Lab2.Models;

namespace OOP_Lab2.Strategies
{
    public class DomStrategy : ISearchStrategy
    {
        public List<SearchResult> Search(SearchResult criteria, string filePath)
        {
            List<SearchResult> results = new List<SearchResult>();
            XmlDocument doc = new XmlDocument();

            if (!System.IO.File.Exists(filePath)) return results;

            try
            {
                doc.Load(filePath);
                XmlElement? root = doc.DocumentElement;
                foreach (XmlNode facultyNode in root?.SelectNodes("Faculty")!)
                {
                    string facultyName = facultyNode.Attributes?["name"]?.Value ?? "";

                    if (!string.IsNullOrEmpty(criteria.Faculty) &&
                        !facultyName.Contains(criteria.Faculty, StringComparison.OrdinalIgnoreCase))
                        continue;

                    foreach (XmlNode deptNode in facultyNode.SelectNodes("Department")!)
                    {
                        string deptName = deptNode.Attributes?["name"]?.Value ?? "";

                        if (!string.IsNullOrEmpty(criteria.Department) &&
                            !deptName.Contains(criteria.Department, StringComparison.OrdinalIgnoreCase))
                            continue;

                        foreach (XmlNode teacherNode in deptNode.SelectNodes("Teacher")!)
                        {
                            string teacherName = teacherNode.Attributes?["name"]?.Value ?? "";

                            if (!string.IsNullOrEmpty(criteria.TeacherName) &&
                                !teacherName.Contains(criteria.TeacherName, StringComparison.OrdinalIgnoreCase))
                                continue;

                            foreach (XmlNode subjectNode in teacherNode.SelectNodes("Subject")!)
                            {
                                string subjectTitle = subjectNode.Attributes?["title"]?.Value ?? "";
                                string roomAttr = subjectNode.Attributes?["room"]?.Value ?? "";
                                string building = subjectNode.Attributes?["building"]?.Value ?? "";
                                string fullRoom = string.IsNullOrEmpty(building) ? roomAttr : $"{roomAttr} (к.{building})";
                                string credits = subjectNode.Attributes?["credits"]?.Value ?? "-";
                                string hours = subjectNode.Attributes?["hours"]?.Value ?? "-";

                                XmlNode groupsNode = subjectNode.SelectSingleNode("Groups")!;
                                string groups = groupsNode?.InnerText ?? "";
                                if (!string.IsNullOrEmpty(criteria.Groups))
                                {
                                    if (!groups.Contains(criteria.Groups, StringComparison.OrdinalIgnoreCase))
                                        continue;
                                }
                                if (!string.IsNullOrEmpty(criteria.Subject))
                                {
                                    if (!subjectTitle.Contains(criteria.Subject, StringComparison.OrdinalIgnoreCase))
                                    {
                                        continue;
                                    }
                                }
                                if (!string.IsNullOrEmpty(criteria.Room))
                                {
                                    if (!fullRoom.Contains(criteria.Room, StringComparison.OrdinalIgnoreCase))
                                    {
                                        continue;
                                    }
                                }
                                results.Add(new SearchResult
                                {
                                    Faculty = facultyName,
                                    Department = deptName,
                                    TeacherName = teacherName,
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
            catch { }
            return results;
        }
    }
}