using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;

namespace LiveStreamStore.Lib.Services.Stores
{
    public interface IStoreService
    {
        List<Store> GetAllStore();
    }
}
