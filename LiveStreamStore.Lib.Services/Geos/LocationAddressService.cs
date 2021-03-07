using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Services.Caching;

namespace LiveStreamStore.Lib.Services.Geos
{
    public class LocationAddressService : ILocationAddressService
    {
        #region cacheKey

        //State Province
        public static string StateProvincePrefix => "StateProvince";
        public static CacheKey AllStateProvinceCacheKey => new CacheKey("StateProvince-All", StateProvincePrefix);

        //District
        public static string DistrictPrefix => "District";
        public static CacheKey AllDistrictCacheKey => new CacheKey("District-All", DistrictPrefix);

        //Ward
        public static string WardPrefix => "Ward";
        public static CacheKey AllWardCacheKey => new CacheKey("Ward-All", WardPrefix);

        #endregion

        private ICacheService _CacheService;

        public LocationAddressService(ICacheService cacheService)
        {
            _CacheService = cacheService;
        }

        public IList<StateProvince> GetAllStateProvince()
        {
            try
            {
                return _CacheService.Get(AllStateProvinceCacheKey, () =>
                {
                    using var db = new LiveStreamStoreContext();
                    return db.StateProvince.Where(x => x.DisplayOrder == 0).ToList();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //District
        public IList<District> GetAllDistrict()
        {
            try
            {
                return _CacheService.Get(AllDistrictCacheKey, () =>
                {
                    using var db = new LiveStreamStoreContext();
                    return db.District.ToList();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<District> GetDistrictById(string StateProvinceId)
        {
            try
            {
                var a = GetAllDistrict().Where(x => x.StateProvinceId == StateProvinceId).ToList();
                return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Ward
        public IList<Ward> GetAllWard()
        {
            try
            {
                return _CacheService.Get(AllWardCacheKey, () =>
                {
                    using var db = new LiveStreamStoreContext();
                    return db.Ward.ToList();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IList<Ward> GetWardById(string DistrictId)
        {
            try
            {
                var a = GetAllWard().Where(x => x.DistrictId == DistrictId).ToList();
                return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetStateProvinceNameById(string id)
        {
            try
            {
                var ProvinceName = GetAllStateProvince().FirstOrDefault(x => x.Id == id).Name;
                return ProvinceName;
            }
            catch (Exception ex)
            {               
                throw ex;
            }
        }
        public string GetStateDistrictNameById(string id)
        {
            try
            {
                var districtName = GetAllDistrict().FirstOrDefault(x =>x.Id == id).Name;
                return districtName;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetStateWardNameById(string id)
        {
            try
            {
                var wardName = GetAllWard().FirstOrDefault(x => x.Id == id).WardName;
                return wardName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
