using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OOP_Lab2.Models;

namespace OOP_Lab2.Strategies
{
    public class LinqToXmlStrategy : ISearchStrategy
    {
        public List<SearchResult> Search(SearchResult criteria, string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return new List<SearchResult>();

            try
            {
                XDocument doc = XDocument.Load(filePath);

                var query = from subject in doc.Descendants("Subject")
                            let teacher = subject.Parent
                            let department = teacher.Parent
                            let faculty = department.Parent

                            let fName = (string?)faculty.Attribute("name") ?? ""
                            let dName = (string?)department.Attribute("name") ?? ""
                            let tName = (string?)teacher.Attribute("name") ?? ""
                            let sTitle = (string?)subject.Attribute("title") ?? ""
                            let sRoom = (string?)subject.Attribute("room") ?? ""
                            let sBuild = (string?)subject.Attribute("building") ?? ""
                            let fullRoom = string.IsNullOrEmpty(sBuild) ? sRoom : $"{sRoom} (к.{sBuild})"
                            let sGroups = (string?)subject.Element("Groups") ?? ""

                            where
                            (string.IsNullOrEmpty(criteria.Faculty) || fName.Contains(criteria.Faculty, StringComparison.OrdinalIgnoreCase)) &&
                            (string.IsNullOrEmpty(criteria.Department) || dName.Contains(criteria.Department, StringComparison.OrdinalIgnoreCase)) &&
                            (string.IsNullOrEmpty(criteria.TeacherName) || tName.Contains(criteria.TeacherName, StringComparison.OrdinalIgnoreCase)) &&
                            (string.IsNullOrEmpty(criteria.Subject) || sTitle.Contains(criteria.Subject, StringComparison.OrdinalIgnoreCase)) &&
                            (string.IsNullOrEmpty(criteria.Room) || fullRoom.Contains(criteria.Room, StringComparison.OrdinalIgnoreCase)) &&
                            (string.IsNullOrEmpty(criteria.Groups) || sGroups.Contains(criteria.Groups, StringComparison.OrdinalIgnoreCase))

                            select new SearchResult
                            {
                                Faculty = fName,
                                Department = dName,
                                TeacherName = tName,
                                Subject = sTitle,
                                Room = fullRoom,
                                Credits = (string?)subject.Attribute("credits") ?? "-",
                                Hours = (string?)subject.Attribute("hours") ?? "-",
                                Groups = (string?)subject.Element("Groups") ?? ""
                            };

                return query.ToList();
            }
            catch
            {
                return new List<SearchResult>();
            }
        }
    }
}