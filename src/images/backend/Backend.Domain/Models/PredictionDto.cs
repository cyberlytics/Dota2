using System.Collections.Generic;
using Backend.Domain.Repositories;

namespace Backend.Domain.Models
{
    public class PredictionDto
    {
        public PredictionDto(string model_name, List<MatchDto> matches)
        {
            this.model_name = model_name;
            this.matches = matches;
        }
        
        public string model_name { get; set; }
        public List<MatchDto> matches { get; set; }
    }
}