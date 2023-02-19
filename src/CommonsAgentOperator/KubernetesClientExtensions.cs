

using IdentityModel;
using k8s;
using k8s.Models;
using KubeOps.KubernetesClient;

namespace CommunAxiom.Commons.Client.Hosting.Operator
{
    public static class KubernetesClientExtensions
    {
        public static async Task<IKubernetesObject<V1ObjectMeta>> CreateObject(this IKubernetesClient cl, IKubernetesObject<V1ObjectMeta> obj)
        {
            if(obj is V1Deployment)
            {
                var depl = (V1Deployment)obj;
                return await cl.Create(depl);
            }
            else if (obj is V1Service) 
            {
                var svc = (V1Service)obj;
                return await cl.Create(svc);
            }
            else if (obj is V1ServiceAccount)
            {
                var sa = (V1ServiceAccount)obj;
                return await cl.Create(sa);
            }
            else if (obj is V1Ingress)
            {
                var ing = (V1Ingress)obj;
                return await cl.Create(ing);
            }
            else if (obj is V1ConfigMap)
            {
                var cm = (V1ConfigMap)obj;
                return await cl.Create(cm);
            }

            throw new NotImplementedException();
        }

        public static async Task<IKubernetesObject<V1ObjectMeta>> UpdateObject(this IKubernetesClient cl, IKubernetesObject<V1ObjectMeta> obj)
        {
            if (obj is V1Deployment)
            {
                var depl = (V1Deployment)obj;
                return await cl.Update(depl);
            }
            else if (obj is V1Service)
            {
                var svc = (V1Service)obj;
                return await cl.Update(svc);
            }
            else if (obj is V1ServiceAccount)
            {
                var sa = (V1ServiceAccount)obj;
                return await cl.Update(sa);
            }
            else if (obj is V1Ingress)
            {
                var ing = (V1Ingress)obj;
                return await cl.Update(ing);
            }
            else if (obj is V1ConfigMap)
            {
                var cm = (V1ConfigMap)obj;
                return await cl.Update(cm);
            }

            throw new NotImplementedException();
        }

        public static async Task DeleteObject<TObj>(this IKubernetesClient cl, ILogger logger, string nameSpace, string name) where TObj : class, IKubernetesObject<V1ObjectMeta>
        {
            var objRef = await cl.Get<TObj>(
                name,
                nameSpace
            );

            if (objRef == null)
            {
                logger.LogError(
                    "Failed to find agent referee for resource {Name} in namespace {Namespace}",
                    name,
                    nameSpace
                );
            }
            else
            {
                await cl.Delete(objRef);
                logger.LogInformation(
                    "Removed Agent Referee for {Name} in namespace {Namespace}",
                    name,
                    nameSpace
                );
            }
        }
    }
}
