using o = Orleans;
using System;
using Orleans.Streams;
using System.Threading.Tasks;
using Orleans;

namespace CommunAxiom.Commons.Orleans
{
    public delegate IStreamProvider GetStreamProvider(string name);
    public class GrainFactory : IComaxGrainFactory
    {
        private readonly o.IGrainFactory _grainFactory;
        private readonly GetStreamProvider _getStreamProviderDelegate;

        public GrainFactory(o.IGrainFactory grainFactory, GetStreamProvider getStreamProviderDelegate)
        {
            _grainFactory = grainFactory;
            _getStreamProviderDelegate = getStreamProviderDelegate;
        }

        public Task<TGrainObserverInterface> CreateObjectReference<TGrainObserverInterface>(IGrainObserver obj) where TGrainObserverInterface : IGrainObserver
        {
            return _grainFactory.CreateObjectReference<TGrainObserverInterface>(obj);
        }

        public Task DeleteObjectReference<TGrainObserverInterface>(IGrainObserver obj) where TGrainObserverInterface : IGrainObserver
        {
            return _grainFactory.DeleteObjectReference<TGrainObserverInterface>(obj);
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

        public IStreamProvider GetStreamProvider(string name)
        {
            return _getStreamProviderDelegate(name);
        }
    }
}
