using o = Orleans;
using System;

namespace CommunAxiom.Commons.Orleans
{
    public class GrainFactory : IGrainFactory
    {
        private readonly o.IGrainFactory _grainFactory;

        public GrainFactory(o.IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }


        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string grainClassNamePrefix = null)
            where TGrainInterface : o.IGrainWithGuidKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string grainClassNamePrefix = null) 
            where TGrainInterface : o.IGrainWithIntegerKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);

        }

        public TGrainInterface GetGrain<TGrainInterface>(string primaryKey, string grainClassNamePrefix = null) 
            where TGrainInterface : o.IGrainWithStringKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);

        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension, string grainClassNamePrefix = null)
            where TGrainInterface : o.IGrainWithGuidCompoundKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);

        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension, string grainClassNamePrefix = null) 
            where TGrainInterface : o.IGrainWithIntegerCompoundKey
        {
            return _grainFactory.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);
        }
    }
}
