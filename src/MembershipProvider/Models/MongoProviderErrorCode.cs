using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MembershipProvider.Models
{
    internal enum MongoProviderErrorCode
    {
        ProvidersBase = 900000,

        GrainStorageOperations = ProvidersBase + 100,
        StorageProvider_Reading = GrainStorageOperations + 4,
        StorageProvider_Writing = GrainStorageOperations + 5,
        StorageProvider_Deleting = GrainStorageOperations + 6,

        MembershipTable_Operations = ProvidersBase + 200,

        StatisticsPublisher_Operations = ProvidersBase + 300,

        Reminders_Operations = ProvidersBase + 400
    }
}
