using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Models.Enums
{
    /// <summary>
    /// Specifies the type of storage used for data persistence or retrieval.
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// Indicates that data is stored in a temporary cache.
        /// </summary>
       Cache,

        /// <summary>
        /// Indicates that data is stored in a file system.
        /// </summary>
       File,

        /// <summary>
        /// Indicates that data is stored in a database.
        /// </summary>
       Database
    }
}
