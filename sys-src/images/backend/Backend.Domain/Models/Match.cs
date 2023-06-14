using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

#pragma warning disable IDE1006 // Benennungsstile

namespace Backend.Domain.Models
{
    [BsonIgnoreExtraElements]
    public class Match
    {
        ///<summary>
        ///The ID number of the match assigned by Valve
        ///</summary>
        public long match_id { get; set; }

        ///<summary>
        ///Array containing information on the chat of the game
        ///</summary>
        public List<Chat> chat { get; set; }

        ///<summary>
        ///Final score for Dire (number of kills on Radiant)
        ///</summary>
        public int dire_score { get; set; }

        ///<summary>
        ///Duration of the game in seconds
        ///</summary>
        public int duration { get; set; }

        ///<summary>
        ///Time in seconds at which first blood occurred
        ///</summary>
        public int first_blood_time { get; set; }

        ///<summary>
        ///Integer corresponding to game mode played. List of constants can be found here: https://github.com/odota/dotaconstants/blob/master/json/game_mode.json
        ///</summary>
        public int game_mode { get; set; }

        ///<summary>
        ///Integer corresponding to lobby type of match. List of constants can be found here: https://github.com/odota/dotaconstants/blob/master/json/lobby_type.json
        ///</summary>
        public int lobby_type { get; set; }

        ///<summary>
        ///Object containing information on the draft. Each pick/ban contains a boolean relating to whether the choice is a pick or a ban, the hero ID, the team the picked or banned it, and the order.
        ///</summary>
        public List<Pick_Ban> picks_bans { get; set; }

        ///<summary>
        ///Final score for Radiant (number of kills on Radiant)
        ///</summary>
        public int radiant_score { get; set; }

        ///<summary>
        ///Boolean indicating whether Radiant won the match
        ///</summary>
        public bool radiant_win { get; set; }

        ///<summary>
        ///Skill bracket assigned by Valve (Normal, High, Very High)
        ///</summary>
        public int skill { get; set; }

        ///<summary>
        ///Array of information on individual players
        ///</summary>
        public List<Player> players { get; set; }

        ///<summary>
        ///Word counts of the all chat messages in the player's games
        ///</summary>
        public SortedList<string, long> all_word_counts { get; set; }

        ///<summary>
        ///Parse version, used internally by OpenDota
        ///</summary>
        public int version { get; set; } = -1;

        public class Chat
        {
            ///<summary>
            ///Time in seconds at which the message was said
            ///</summary>
            public int time { get; set; }

            ///<summary>
            ///Methode of input, chat, chatwheel, ...
            ///</summary>
            public string type { get; set; }

            ///<summary>
            ///The message the player sent
            ///</summary>
            public string key { get; set; }

            ///<summary>
            ///slot
            ///</summary>
            public int slot { get; set; }

            ///<summary>
            ///Which slot the player is in. 0-127 are Radiant, 128-255 are Dire
            ///</summary>
            public int player_slot { get; set; }
        }

        public class Pick_Ban
        {
            ///<summary>
            ///Whether the choice is a pick or ban
            ///</summary>
            public bool is_pick { get; set; }

            ///<summary>
            ///The Hero ID
            ///</summary>
            public int hero_id { get; set; }

            ///<summary>
            ///The team that picked or banned it
            ///</summary>
            public int team { get; set; }

            ///<summary>
            ///The order of the pick or bann
            ///</summary>
            public int order { get; set; }
        }

        public class Player
        {
            ///<summary>
            ///Which slot the player is in. 0-127 are Radiant, 128-255 are Dire
            ///</summary>
            public int player_slot { get; set; }

            ///<summary>
            ///account_id
            ///</summary>
            public long account_id { get; set; } = -1;

            ///<summary>
            ///The ID value of the hero played
            ///</summary>
            public int hero_id { get; set; }

            ///<summary>
            ///Number of assists the player had
            ///</summary>
            public int assists { get; set; }

            ///<summary>
            ///Number of deaths
            ///</summary>
            public int deaths { get; set; }

            ///<summary>
            ///Number of denies
            ///</summary>
            public int denies { get; set; }

            ///<summary>
            ///Gold Per Minute obtained by this player
            ///</summary>
            public int gold_per_min { get; set; }

            ///<summary>
            ///Hero Damage Dealt
            ///</summary>
            public int hero_damage { get; set; }

            ///<summary>
            ///Hero Healing Done
            ///</summary>
            public int hero_healing { get; set; }

            ///<summary>
            ///firstblood_claimed
            ///</summary>
            public int firstblood_claimed { get; set; }

            ///<summary>
            ///Object containing information about the player's killstreaks
            ///</summary>
            public SortedList<string, int> kill_streaks { get; set; }

            ///<summary>
            ///Number of kills
            ///</summary>
            public int kills { get; set; }

            ///<summary>
            ///Array containing information on which hero the player killed at what time
            ///</summary>
            public List<kill_log> kills_log { get; set; }

            ///<summary>
            ///Number of last hits
            ///</summary>
            public int last_hits { get; set; }

            ///<summary>
            ///Integer describing whether or not the player left the game. 0: didn't leave. 1: left safely. 2+: Abandoned
            ///</summary>
            public int leaver_status { get; set; }

            ///<summary>
            ///Level at the end of the game
            ///</summary>
            public int level { get; set; }

            ///<summary>
            ///net_worth
            ///</summary>
            public int net_worth { get; set; }

            ///<summary>
            ///Total number of observer wards placed
            ///</summary>
            public int obs_placed { get; set; }

            ///<summary>
            ///party_id
            ///</summary>
            public int party_id { get; set; }

            ///<summary>
            ///party_size
            ///</summary>
            public int party_size { get; set; }

            ///<summary>
            ///Total number of pings
            ///</summary>
            public int pings { get; set; }

            ///<summary>
            ///How many sentries were placed by the player
            ///</summary>
            public int sen_placed { get; set; }

            ///<summary>
            ///teamfight_participation
            ///</summary>
            public float teamfight_participation { get; set; }

            ///<summary>
            ///Total tower damage done by the player
            ///</summary>
            public int tower_damage { get; set; }

            ///<summary>
            ///Total number of tower kills the player had
            ///</summary>
            public int towers_killed { get; set; }

            ///<summary>
            ///Experience Per Minute obtained by the player
            ///</summary>
            public int xp_per_min { get; set; }

            ///<summary>
            ///Boolean for whether or not the player is on Radiant
            ///</summary>
            public bool isRadiant { get; set; }

            ///<summary>
            ///Binary integer representing whether or not the player won
            ///</summary>
            public int win { get; set; }

            ///<summary>
            ///Binary integer representing whether or not the player lost
            ///</summary>
            public int lose { get; set; }

            ///<summary>
            ///Total gold at the end of the game
            ///</summary>
            public int total_gold { get; set; }

            ///<summary>
            ///Total experience at the end of the game
            ///</summary>
            public int total_xp { get; set; }

            ///<summary>
            ///Number of kills per minute
            ///</summary>
            public double kills_per_min { get; set; }

            ///<summary>
            ///kda
            ///</summary>
            public double kda { get; set; }

            ///<summary>
            ///abandons
            ///</summary>
            public int abandons { get; set; }

            ///<summary>
            ///Total number of neutral creeps killed
            ///</summary>
            public int neutral_kills { get; set; }

            ///<summary>
            ///Total number of lane creeps killed by the player
            ///</summary>
            public int lane_kills { get; set; }

            ///<summary>
            ///Total number of observer wards killed by the player
            ///</summary>
            public int observer_kills { get; set; }

            ///<summary>
            ///Total number of sentry wards killed by the player
            ///</summary>
            public int sentry_kills { get; set; }

            ///<summary>
            ///Total number of Ancient creeps killed by the player
            ///</summary>
            public int ancient_kills { get; set; }

            ///<summary>
            ///lane_efficiency
            ///</summary>
            public double lane_efficiency { get; set; }

            ///<summary>
            ///Integer referring to which lane the hero laned in
            ///</summary>
            public int lane { get; set; }

            ///<summary>
            ///lane_role
            ///</summary>
            public int lane_role { get; set; }

            ///<summary>
            ///Boolean referring to whether or not the player roamed
            ///</summary>
            public bool is_roaming { get; set; }

            ///<summary>
            ///Actions per minute
            ///</summary>
            public int actions_per_min { get; set; }

            ///<summary>
            ///Total number of TP scrolls purchased by the player
            ///</summary>
            public int purchase_tpscroll { get; set; }

            ///<summary>
            ///The rank tier of the player. Tens place indicates rank, ones place indicates stars.
            ///</summary>
            public int rank_tier { get; set; }

            public class kill_log
            {
                ///<summary>
                ///Time in seconds the player killed the hero
                ///</summary>
                public int time { get; set; }

                ///<summary>
                ///Hero killed
                ///</summary>
                public string key { get; set; }
            }
        }
    }
}