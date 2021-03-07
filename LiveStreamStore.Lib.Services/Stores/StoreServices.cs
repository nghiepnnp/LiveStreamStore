using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;

namespace LiveStreamStore.Lib.Services.Stores
{
    public class StoreServices: IStoreService
    {
        public List<Store> GetAllStore()
        {
            try
            {             
                    using var db = new LiveStreamStoreContext();
                    return db.Store.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
