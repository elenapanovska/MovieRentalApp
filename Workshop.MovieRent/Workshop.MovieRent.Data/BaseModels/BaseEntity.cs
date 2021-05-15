using System;
using System.Collections.Generic;
using System.Text;

namespace Workshop.MovieRent.Data.BaseModels
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public BaseEntity()
        {
            CreatedOn = DateTime.Now;
        }

    }
}
