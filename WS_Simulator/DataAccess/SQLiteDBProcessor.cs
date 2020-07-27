using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_Simulator.Models;

namespace WS_Simulator.DataAccess
{
    public static class SQLiteDBProcessor
    {
        private static SQLiteContext _sQLiteContext = new SQLiteContext();
        static SQLiteDBProcessor()
        {
            _sQLiteContext.Database.EnsureCreated();
        }

        public static void SaveDataToDB(TestRepository testRepository)
        {
            _sQLiteContext.TestRespositories.Add(testRepository);
            _sQLiteContext.SaveChanges();
        }

        public static bool CheckRepositoryName(string name)
        {
            return (_sQLiteContext.TestRespositories.Where(x => x.RepositoryName == name).Count() > 0);
        }

    }
}
