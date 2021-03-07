using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Services.WorkContext;
using LiveStreamStore.Web;

namespace UsExpress.LiveStream.Web.Controllers
{
    public class BaseController : Controller
    {
        protected static readonly log4net.ILog _Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected IWorkContext _WorkContext = EngineContext.Resolve<IWorkContext>();

        public User _User { get => _WorkContext.CurrentUser; set => _WorkContext.CurrentUser = value; }
        public User _UserCookie { get => _WorkContext.UserCookie; set => _WorkContext.UserCookie = value; }

        public void SetUserCookie()
        {
            _WorkContext.SetUserCookie();
        }

        public string _Timezoneoffset
        {
            get
            {
                return _WorkContext.CurrentTimeZoneOffSet;
            }
            set { _WorkContext.CurrentTimeZoneOffSet = value; }
        }

        public string GetCurrentTimeZone()
        {
            try
            {
                return _WorkContext.CurrentTimeZoneOffSet;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw;
            }
        }

        public void SetSessionTimeZone(string timezoneoffset)
        {
            try
            {
                _WorkContext.CurrentTimeZoneOffSet = timezoneoffset;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw;
            }
        }
    }
}
