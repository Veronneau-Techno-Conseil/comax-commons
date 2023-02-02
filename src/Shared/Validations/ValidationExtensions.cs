using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared.Validations
{
    public static class ValidationExtensions
    {
        public static Validatable<TObject> Validatable<TObject>(this TObject o) where TObject : class
        {
            return new Validatable<TObject>(o);
        }
    }
}
