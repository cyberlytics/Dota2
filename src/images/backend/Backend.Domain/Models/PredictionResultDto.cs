using System.Collections.Generic;

namespace Backend.Domain.Models
{
    public class PredictionResultDto
    {
        public float score { get; set; }
        public List<Prediction> results { get; set; }

        public class Prediction
        {
            public bool predict { get; set; }
            public bool ground_truth { get; set; }
            public List<float> predict_proba { get; set; }
        }
    }
}