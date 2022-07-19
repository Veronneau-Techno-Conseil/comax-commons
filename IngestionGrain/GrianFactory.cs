using Orleans;
using System;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    public class GrianFactory : IGrainFactory
    {
        private readonly Orleans.IGrainFactory _grainFactory;

        public GrianFactory(Orleans.IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }


        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithGuidKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);

        }

        public TGrainInterface GetGrain<TGrainInterface>(string primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithStringKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);

        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithGuidCompoundKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);

        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerCompoundKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);
        }
    }
}
