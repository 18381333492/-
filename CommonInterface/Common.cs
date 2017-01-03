using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterface
{
    /// <summary>
    /// 通用的方法操作类
    /// </summary>
    public  class Common
    {
        /// <summary>
        /// 读取配置文件中的appSetting
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string ReadAppSetting(string appName)
        {
            string result = string.Empty;
            try
            {
                result = ConfigurationManager.AppSettings[appName];
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
    }
}
