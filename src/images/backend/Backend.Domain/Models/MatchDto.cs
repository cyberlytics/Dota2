using System.Collections.Generic;

namespace Backend.Domain.Models
{
    public class MatchDto
    {

        public MatchDto(Match match)
        {
            match_id = match.match_id;
            radiant_win = match.radiant_win;
            players = new List<MatchDtoPlayer>();

            if (match.players != null)
            {
                foreach (var matchPlayer in match.players)
                {
                    players.Add(new MatchDtoPlayer()
                    {
                        pings = matchPlayer.pings,
                        kills = matchPlayer.kills,
                        deaths = matchPlayer.deaths,
                        assists = matchPlayer.assists,
                        win = matchPlayer.win != 0
                    });
                }
            }
        }

        public long match_id { get; set; }
        public bool radiant_win { get; set; }
        public List<MatchDtoPlayer> players { get; set; }

        public class MatchDtoPlayer
        {
            public int pings { get; set; }
            public int kills { get; set; }
            public int deaths { get; set; }
            public int assists { get; set; }
            public bool win { get; set; }
        }
    }
}