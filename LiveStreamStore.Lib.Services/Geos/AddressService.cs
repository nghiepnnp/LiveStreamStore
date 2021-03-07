using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Geos
{
    public class AddressService : IAddressService
    {
        private ILocationAddressService _LocationAddressService;
        public AddressService(ILocationAddressService locationAddressService)
        {
            _LocationAddressService = locationAddressService;
        }
        public Address GetAddressByCustomerId(int id)
        {
            using (var context = new LiveStreamStoreContext())
            {
                var a = context.Address.Where(x => x.CustomerId == id).Include(x => x.Customer).FirstOrDefault();
                return a;
            }
        }

        public ErrorObject CreateAddress(Address address)
        {
            var error = Error.Success();
            try 
            {
                using (var context = new LiveStreamStoreContext())
                {
                    address.CustomerId = address.CustomerId;
                    address.StateProvinceId = address.StateProvinceId;
                    address.DistrictId = address.DistrictId;
                    address.WardId = address.WardId;
                    address.Address1 = address.Address1;
                    address.StateProvinceName = address.StateProvinceName;
                    address.DistrictName = address.DistrictName;
                    address.WardName = address.WardName;

                    address.CreatedDateUtc = DateTime.Now;
                    context.Address.Add(address);
                    return context.SaveChanges() > 0 ? error : error.Failed("Create CustomerAddress failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject UpdateAddress(Address address)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var addr = context.Address.Where(x => x.Id == address.Id).FirstOrDefault();
                    addr.StateProvinceId = address.StateProvinceId;
                    addr.DistrictId = address.DistrictId;
                    addr.WardId = address.WardId;
                    addr.Address1 = address.Address1;
                    addr.StateProvinceName = _LocationAddressService.GetStateProvinceNameById(address.StateProvinceId);
                    addr.DistrictName =  _LocationAddressService.GetStateDistrictNameById(address.DistrictId);
                    addr.WardName =  _LocationAddressService.GetStateWardNameById(address.WardId);

                    context.Address.Update(addr);
                    return context.SaveChanges() > 0 ? error : error.Failed("Create CustomerAddress failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
