using System;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFieldValidatorLookup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        IFieldValidator? Get(string tag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        bool TryGet(string tag, out IFieldValidator validator);
    }
}

