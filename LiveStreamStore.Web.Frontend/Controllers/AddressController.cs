using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveStream.Web.Frontend.Controllers;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Services.Geos;
using LiveStreamStore.Lib.Services.Logs;

namespace LiveStreamStore.Web.Frontend.Controllers
{
    public class AddressController : BaseController
    {
        private readonly IAddressService _AddressService;
        private readonly ILogService _LogService;

        public AddressController(IAddressService addressService, ILogService logService)
        {
            _AddressService = addressService;
            _LogService = logService;
        }

        [HttpPost]
        public IActionResult CreateOrUpdate(Address address)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (address.Id == 0)
                {
                    error = _AddressService.CreateAddress(address);
                }
                else
                {
                    error = _AddressService.UpdateAddress(address);
                }
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLogFrontend(method, address));
            }
        }
    }
}
