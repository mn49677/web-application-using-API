using System;
using System.Collections.Generic;
using System.Text;

namespace ORTools
{
    interface IORToolsMapper<T> where T : class
    {
        public DataModel Map(T model);
    }
}
