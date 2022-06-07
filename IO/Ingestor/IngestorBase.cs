﻿using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Injestor
{
    public abstract class IngestorBase
    {
        protected abstract IEnumerable<ValidationError> Validate(JObject data);

    }
}

