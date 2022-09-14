﻿using CommunAxiom.Commons.Orleans;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    public class PublicBoardBusiness
    {
        private readonly PublicBoardRepo _publicBoardRepo;
        private readonly IGrainFactory _grainFactory;
        private readonly ILogger _logger;
        
        public PublicBoardBusiness(PublicBoardRepo repo, IGrainFactory grainFactory, ILogger logger)
        {
            _publicBoardRepo = repo;
            _grainFactory = grainFactory;
            _logger = logger;
        }


    }
}
