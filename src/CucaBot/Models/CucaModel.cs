using System;
using System.Collections.Generic;

namespace CucaBot.Models
{
    public class CucaModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }

        public List<UserModel> Participants { get; set; } = new List<UserModel>();
    }
}