using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workload
{
    class Props
    {
        public static FbConnectionStringBuilder ConnectionStringBuilder = new FbConnectionStringBuilder()
        {
            ServerType = FbServerType.Embedded,
            UserID = "SYSDBA",
            Password = "masterkey",
            Dialect = 3,
            Charset = "ASCII",
            ClientLibrary = System.IO.Path.Combine(AppPath, "fbembed.dll"),
            Database= System.IO.Path.Combine(AppPath, "wlbase.fdb")

        };
        
        public static System.String AppPath
        { get { return Path.GetDirectoryName( new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath); } }

        public static System.String ConnectionString
        { get { return ConnectionStringBuilder.ConnectionString; } }
    }
}
