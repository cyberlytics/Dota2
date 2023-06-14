using System;

#pragma warning disable IDE1006 // Benennungsstile

namespace Backend.Domain.Models
{
    public class Account
    {
        /// <summary>
        /// Steam32 ID
        /// </summary>
        public long account_id { get; set; }

        /// <summary>
        /// Nickname
        /// </summary>
        public string personaname { get; set; }

        /// <summary>
        /// Avatar URL
        /// </summary>
        public string avatarfull { get; set; }

        /// <summary>
        /// last_match_time. May not be present or null.
        /// </summary>
        public DateTime last_match_time { get; set; }

        /// <summary>
        /// similarity
        /// </summary>
        public float similarity { get; set; }
    }
}