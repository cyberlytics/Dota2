using System.Collections.Generic;

namespace Backend.Domain.Models
{
    public class MatchDto
    {

        public MatchDto(Match match)
        {
            radiant_win = match.radiant_win;
            players = new List<Player>();

            foreach (var matchPlayer in match.players)
            {
                players.Add(new Player()
                {
                    pings = matchPlayer.pings,
                    //TODO: Uncomment after Match Model Update
                    //kills = matchPlayer.kills,
                    //deaths = matchPlayer.deaths,
                    //assists = matchPlayer.assists,
                    //win = matchPlayer.win
                });
            }
        }
        
        public bool radiant_win { get; set; }
        public List<Player> players { get; set; }

        public class Player
        {
            public int pings { get; set; }
            public int kills { get; set; }
            public int deaths { get; set; }
            public int assists { get; set; }
            public bool win { get; set; }
        }
    }
}