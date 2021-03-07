using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;

namespace LiveStreamStore.Lib.Services.Geos
{
    public interface ILocationAddressService
    {
        // State Province
        IList<StateProvince> GetAllStateProvince();
        string GetStateProvinceNameById(string id);
        // District
        IList<District> GetAllDistrict();
        IList<District> GetDistrictById(string StateProvinceId);
        string GetStateDistrictNameById(string id);

        // Ward
        IList<Ward> GetAllWard();
        IList<Ward> GetWardById(string DistrictId);
        string GetStateWardNameById(string id);
    }
}
