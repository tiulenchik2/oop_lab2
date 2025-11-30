using OOP_Lab2.Models;
using System.Collections.Generic;

namespace OOP_Lab2.Strategies
{
    public interface ISearchStrategy
    {
        List<SearchResult> Search(SearchResult criteria, string filePath);
    }

}
