﻿using System.Collections.Generic;

namespace Backend.Domain.Models
{
    public class MatchDto
    {

        public MatchDto(Match match)
        {
            radiant_win = match.radiant_win;
            players = new List<MatchDtoPlayer>();

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