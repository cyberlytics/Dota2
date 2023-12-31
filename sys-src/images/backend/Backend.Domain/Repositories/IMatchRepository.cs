﻿using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.Services
{
    public interface IMatchRepository
    {
        List<Match> Get();
        long GetCount(long id);
        Match Get(long id);
        Match Create(Match match);
        
        void Update(long id, Match match);
        void Remove(Match match);
        void Remove(long id);
    }
}
