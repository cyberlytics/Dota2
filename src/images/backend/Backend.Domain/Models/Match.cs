using System.Collections.Generic;

namespace Backend.Domain.Models
{
    public class Match
    {
        public long match_id { get; set; }
        public bool radiant_win { get; set; }
        public List<Player> players { get; set; }

        public class Player
        {
            public long account_id { get; set; } = -1;
            public int player_slot { get; set; }
            public int pings { get; set; }
        }
    }
}