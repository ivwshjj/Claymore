using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Claymore.Authentication.SSO.Common
{
    /// <summary>
    /// 密钥的创建与管理类,文件的后缀默认为.config
    /// 路径默认为网站根目录下 POSSite 文件夹下
    /// </summary>
    public class KeyManager
    {
        private static string storebase;
        private static string suffix = ".config";
        static KeyManager()
        {
            storebase = AppDomain.CurrentDomain.BaseDirectory + "PSOSite\\";
            if (!Directory.Exists(storebase))
            {
                Directory.CreateDirectory(storebase);
            }
        }

        /// <summary>
        /// 根据网站的ID获取KEY与IV密钥
        /// </summary>
        /// <param name="ID">网站ID,在PSO站点定义</param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        public static void GetKeyBySiteID(string ID, out string Key, out string IV)
        {
            string path = storebase + ID + suffix;
            if (File.Exists(path))      //配制文件是否存在
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    Key = reader.ReadLine();
                    IV = reader.ReadLine();
                    reader.Close();
                }
            }
            else
            {
                Key = "";
                IV = "";
            }
        }

        /// <summary>
        /// 根据站点创建密钥文件
        /// </summary>
        /// <param name="SiteID"></param>
        public static void UpdateKey(string SiteID)
        {
            string path = storebase + SiteID + suffix;
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Default))
            {
                Cryptography.KeyMaker km = new Cryptography.KeyMaker();
                writer.WriteLine(km.Key);
                writer.WriteLine(km.Iv);
                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// 删除密钥文件
        /// </summary>
        /// <param name="SiteID"></param>
        public static void DeleteKey(string SiteID)
        {
            string path = storebase + SiteID + suffix;
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
