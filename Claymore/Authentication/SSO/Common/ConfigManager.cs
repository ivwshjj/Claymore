using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace Claymore.Authentication.SSO.Common
{
    public class ConfigManager
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public static string SiteID
        {
            get
            {
                return CurrentSSOConfiguration.AppSettings.Settings["SiteID"].Value;
            }
        }

        /// <summary>
        /// 加密密钥
        /// </summary>
        public static string Key
        {
            get
            {
                return CurrentSSOConfiguration.AppSettings.Settings["Key"].Value;
            }
        }

        /// <summary>
        /// 加密密匙
        /// </summary>
        public static string IV
        {
            get
            {
                return CurrentSSOConfiguration.AppSettings.Settings["IV"].Value;
            }
        }

        /// <summary>
        /// SSO请求地址
        /// </summary>
        public static string KeeperUrl
        {
            get
            {
                return CurrentSSOConfiguration.AppSettings.Settings["KeeperUrl"].Value;
            }
        }

        /// <summary>
        /// 退出地址
        /// </summary>
        public static string SignOutUrl
        {
            get
            {
                return CurrentSSOConfiguration.AppSettings.Settings["SignOutUrl"].Value;
            }
        }

        /// <summary>
        /// 请求的KEY
        /// </summary>
        public static string SSOKey
        {
            get
            {
                return CurrentSSOConfiguration.AppSettings.Settings["SSOKey"].Value;
            }
        }

        /// <summary>
        /// 读取配置信息的方法
        /// </summary>
        /// <returns></returns>
        private static Configuration CurrentSSOConfiguration
        {
            get
            {
                ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();

                //NOTE: 这个时候读取的路径是应用程序/网站下面的bin目录, 或者应用程序/网站编译运行dll的目录(自定义输出路径的情况)
                //而不是类库的生成dll的输出路径! 
                //所以, 如果采用引用项目的方式引用, 要将配置文件一同复制到和dll同个目录(bin目录), 否则读取不到配置信息; 修改配置
                //文件后同样记得此目录下覆盖原文件
                configFile.ExeConfigFilename = SSOConfigFilePath;
                return ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);

            }
        }

        /// <summary>
        /// 封装多一遍, 确保调用本方法的方法为Dll内部方法, 从而取得正确的Dll配置文件路径
        /// 否则可能取得的是执行程序(主程序)的路径
        /// 详细参考Assembly.GetCallingAssembly()的原理
        /// </summary>
        private static string SSOConfigFilePath
        {
            get
            {
                Assembly t_assembly = Assembly.GetCallingAssembly();
                Uri t_uri = new Uri(Path.GetDirectoryName(t_assembly.CodeBase));

                //NOTE: 这个时候读取的路径是应用程序/网站下面的bin目录, 或者应用程序/网站编译运行dll的目录(自定义输出路径的情况)
                //而不是类库的生成dll的输出路径! 
                //所以, 如果采用引用项目的方式引用, 要将配置文件一同复制到和dll同个目录(bin目录), 否则读取不到配置信息; 修改配置
                //文件后同样记得此目录下覆盖原文件
                return Path.Combine(t_uri.LocalPath, "SSO.config");
            }
        }

    }
}
