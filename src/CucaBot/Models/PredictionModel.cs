using System;

namespace CucaBot.Models
{
    [Serializable]
    public class PredictionModel
    {
        public string TagId { get; set; }

        public string Tag { get; set; }

        public double Probability { get; set; }
    }
}
