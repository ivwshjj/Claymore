using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public class Friend
    {
        public string XM { get; set; }
        public string addr { get; set; }
    }
    public class Test
    {
        public List<string> strList { get; set; }
        public string Name { get; set; }
        public DateTime birthDay { get; set; }
        public int Age { get; set; }
        public List<Friend> FF { get; set; }

    }
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Test t = new Test();
            t.strList = new List<string>();
            t.FF = new List<Friend>();
            for (int i = 0; i < 10; i++)
            {
                t.strList.Add(i.ToString());
                
            }
            t.FF.Add(new Friend() { XM = "小王", addr = "大王小区" });
            t.FF.Add(new Friend() { XM = "小张", addr = "大王小区" });
            t.Name = "梁卓";
            t.birthDay = DateTime.Now;
            t.Age = 30;

            //this.Response.Write(Claymore.Helper.MyJsonHelper.SerializeObject(t));

            string json = Claymore.Helper.MyJsonHelper.SerializeObject(t);
            Test temp = Claymore.Helper.MyJsonHelper.DeserializeObject<Test>(json); //Newtonsoft.Json.JsonConvert.DeserializeObject<Test>(json);
            //this.Response.Write(Claymore.XmlHelper.XmlSerialize(t.strList, Encoding.UTF8));
            //Claymore.Authentication.SSO.Common.KeyManager.UpdateKey("Default");
            
        }
    }
}