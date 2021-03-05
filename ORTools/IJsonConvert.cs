using System;
using System.Collections.Generic;
using System.Text;

namespace ORTools
{
    public interface IJsonConvert<T>
    {
        public List<T> ConvertFromJson(string name, string id);
    }
}
