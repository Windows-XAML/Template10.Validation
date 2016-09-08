using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Template10.Interfaces.Validation
{
    public interface IProperty<T> : IProperty
    {
        T OriginalValue { get; set; }

        T Value { get; set; }
    }

}