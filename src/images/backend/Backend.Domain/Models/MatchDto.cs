using System.Collections.Generic;

namespace Backend.Domain.Models
{
    /// <summary>
    /// Gekuerzte Variante eines Matches mit weniger Informationen
    /// </summary>
    public class MatchDto
    {

        public MatchDto(Match match)
        {
            match_id = match.match_id;
            radiant_win = match.radiant_win;
            players = new List<MatchDtoPlayer>();

            // Neuen MatchDtoPlayer fuer jeden teilnehmenden Spieler erstellen
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

        public MatchDto(Match match, long player_id)
        {
            match_id = match.match_id;
            radiant_win = match.radiant_win;
            players = new List<MatchDtoPlayer>();

            var matchPlayer = match.players.Find(x => x.account_id == player_id);
            if(matchPlayer != null) players.Add(new MatchDtoPlayer()
            {
                pings = matchPlayer.pings,
                kills = matchPlayer.kills,
                deaths = matchPlayer.deaths,
                assists = matchPlayer.assists,
                win = matchPlayer.win != 0
            });
        }

        ///<summary>
        /// Match-ID
        ///</summary>
        public long match_id { get; set; }
        
        ///<summary>
        /// Team, das das Match gewonnen hat
        ///</summary>
        public bool radiant_win { get; set; }
        
        ///<summary>
        /// Liste aller teilnehmenden Spieler
        ///</summary>
        public List<MatchDtoPlayer> players { get; set; }

        ///<summary>
        /// Teilnehmender Spieler eines Matchs
        ///</summary>
        public class MatchDtoPlayer
        {
            ///<summary>
            /// Anzahl der abgesetzten Pingbefehle des Spielers
            ///</summary>
            public int pings { get; set; }
            ///<summary>
            /// Anzahl der Toetungshilfen des Spielers
            ///</summary>
            public int assists { get; set; }
            
            ///<summary>
            /// Anzahl der Tode des Spielers
            ///</summary>
            public int deaths { get; set; }
            ///<summary>
            /// Anzahl der Toetungen des Spielers
            ///</summary>
            public int kills { get; set; }
            ///<summary>
            /// Sieg des Spielers
            ///</summary>
            public bool win { get; set; }
        }
    }
}