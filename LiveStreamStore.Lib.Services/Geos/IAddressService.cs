using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Geos
{
    public interface IAddressService
    {
        Address GetAddressByCustomerId(int id);
        ErrorObject CreateAddress(Address address);
        ErrorObject UpdateAddress(Address address);
    }
}
