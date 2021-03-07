using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LiveStreamStore.Lib.Services.WorkContext;
using LiveStreamStore.Lib.Utilities;

namespace LiveStreamStore.Lib.Services.Logs
{
    public class LogServices : ILogService
    {
        private IWorkContext _WorkContext;
        public LogServices(IWorkContext workContext)
        {
            _WorkContext = workContext;
        }

        public string WriteLog(MethodBase methodBase, params object[] values)
        {
            var user = _WorkContext.CurrentUser ?? _WorkContext.GetUserCookie();

            string method = methodBase.Name;
            string className = methodBase.ReflectedType.Name;
            string nameSpace = methodBase.ReflectedType.Namespace;
            DateTime createDate = DateTime.Now;

            var paramsAndValues = new List<KeyValuePair<string, object>>();
            var parameters = methodBase.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    if (i == j)
                    {
                        paramsAndValues.Add(new KeyValuePair<string, object>(parameters[i].Name, values[j]));
                        break;
                    }
                }
            }
            var jsonParamsAndValues = paramsAndValues.ToJson();
            var Message = createDate + " " + user.Fullname + " to action: " + method + ", controller: " + className + ", Parameters: " + jsonParamsAndValues;
            return Message;
        }

        public string WriteLogFrontend(MethodBase methodBase, params object[] values)
        {
            var customer = _WorkContext.CurrentCustomer ?? _WorkContext.GetCustomerCookie();
            string method = methodBase.Name;
            string className = methodBase.ReflectedType.Name;
            string nameSpace = methodBase.ReflectedType.Namespace;
            DateTime createDate = DateTime.Now;

            var paramsAndValues = new List<KeyValuePair<string, object>>();
            var parameters = methodBase.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    if (i == j)
                    {
                        paramsAndValues.Add(new KeyValuePair<string, object>(parameters[i].Name, values[j]));
                        break;
                    }
                }
            }
            var jsonParamsAndValues = paramsAndValues.ToJson();
            var Message = createDate + " " + customer.Fullname + " to action: " + method + ", controller: " + className + ", Parameters: " + jsonParamsAndValues;
            return Message;
        }
    }
}
