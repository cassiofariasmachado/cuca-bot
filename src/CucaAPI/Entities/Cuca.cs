using System;
using System.Collections.Generic;

namespace CucaAPI.Entities
{
    public class Cuca
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }

        public List<User> Participants { get; set; } = new List<User>();
    }
}
