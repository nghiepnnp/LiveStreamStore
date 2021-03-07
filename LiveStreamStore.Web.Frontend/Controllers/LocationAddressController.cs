using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveStream.Web.Frontend.Controllers;
using LiveStreamStore.Lib.Services.Geos;

namespace LiveStreamStore.Web.Frontend.Controllers
{
    public class LocationAddressController : BaseController
    {
        private ILocationAddressService _location;
        private IAddressService _address;
        public LocationAddressController(ILocationAddressService location, IAddressService address)
        {
            _location = location;
            _address = address;
        }

        [HttpPost]
        public ActionResult GetListDistrictByStateProvinceId(string StateProvinceId)
        {
            try
            {
                return Json(_location.GetDistrictById(StateProvinceId));
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(ex);
            }
        }

        [HttpPost]
        public ActionResult GetListWardByDistrictId(string DistrictId)
        {
            try
            {
                return Json(_location.GetWardById(DistrictId));
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(ex);
            }
        }
    }
}
