using System;
using System.Collections.Generic;
using System.Text;
using Workshop.MovieRent.Data.BaseModels;
using Workshop.MovieRent.Data.Enums;

namespace Workshop.MovieRent.Data.Models
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; }
        public Genre Genre { get; set; }
        public string Language { get; set; }
        public bool IsAvailable { get; set; }
        public int Length { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int AgeRestriction { get; set; }
        public int Quantity { get; set; }

    }
}
