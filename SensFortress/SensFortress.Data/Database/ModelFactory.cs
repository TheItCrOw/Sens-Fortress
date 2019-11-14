using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Data.Database
{
    /// <summary>
    /// Builds models out of data.
    /// </summary>
    public static class ModelFactory<T>
    {

        public static Queue<T> CreationQueue = new Queue<T>();

    }
}
