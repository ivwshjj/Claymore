using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Xml;

namespace Claymore
{
    
    internal static class ReflectionHelper
    {
        public static readonly Type VoidType = typeof(void);

        private static Dictionary<string, ActionDescription> s_PageActionDict=new Dictionary<string,ActionDescription>();

        private static Dictionary<string, ControllerDescription> s_ServiceFullNameDict=new Dictionary<string,ControllerDescription>();
        private static Dictionary<string, ControllerDescription> s_ServiceShortNameDict=new Dictionary<string,ControllerDescription>();

        private static Hashtable s_ServiceActionTable = Hashtable.Synchronized(
                                                new Hashtable(4096, StringComparer.OrdinalIgnoreCase));

        private static readonly BindingFlags ActionBindingFlags =
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        static ReflectionHelper()
        {
            InitControllers();
        }


        private static BaseActionHandlerFactory[] GetConfigBaseActionHandlerFactory()
        {
            bool useIntegratedPipeline = (bool)typeof(HttpRuntime).InvokeMember("UseIntegratedPipeline",
                BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.NonPublic,
                null, null, null);

            string xpath = useIntegratedPipeline
                ? "/configuration/system.webServer/handlers/add"
                : "/configuration/system.web/httpHandlers/add";

            XmlDocument doc = new XmlDocument();
            doc.Load(HttpRuntime.AppDomainAppPath + "web.config");
            XmlNodeList nodes = doc.SelectNodes(xpath);

            List<BaseActionHandlerFactory> result = new List<BaseActionHandlerFactory>();

            foreach (XmlNode node in nodes)
            {
                string typeName = node.Attributes["type"].Value;
                Type t = System.Web.Compilation.BuildManager.GetType(typeName, true, false);
                if (t.IsSubclassOf(typeof(BaseActionHandlerFactory)))
                    result.Add((BaseActionHandlerFactory)Activator.CreateInstance(t));
            }

            return result.ToArray();
        }


        private static void InitControllers()
        {
            BaseActionHandlerFactory[] baseActionHandlerFactoryList = GetConfigBaseActionHandlerFactory();

            List<ControllerDescription> serviceControllerList = new List<ControllerDescription>(4096);
            List<ControllerDescription> pageControllerList = new List<ControllerDescription>(4096);

            ICollection assemblies = BuildManager.GetReferencedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (Type t in assembly.GetExportedTypes())
                {
                    if (t.IsClass == false)
                        continue;

                    if (t.Name.EndsWith("Controller"))
                        pageControllerList.Add(new ControllerDescription(t));

                    for (int i = 0; i < baseActionHandlerFactoryList.Length; i++)
                        if (baseActionHandlerFactoryList[i].TypeIsService(t))
                        {
                            serviceControllerList.Add(new ControllerDescription(t));
                            break;
                        }

                }
            }

            foreach (ControllerDescription c in serviceControllerList)
            {
                if (c == null || c.ControllerType == null)
                    continue;
                s_ServiceFullNameDict.Add(c.ControllerType.FullName, c);
            }

            s_ServiceShortNameDict = new Dictionary<string, ControllerDescription>(s_ServiceFullNameDict.Count, StringComparer.OrdinalIgnoreCase);
            foreach (ControllerDescription description in serviceControllerList)
            {
                try
                {
                    s_ServiceShortNameDict.Add(description.ControllerType.Name, description);
                }
                catch (ArgumentException)
                {
                    s_ServiceShortNameDict[description.ControllerType.Name] = null;
                }
            }



            s_PageActionDict = new Dictionary<string, ActionDescription>(4096, StringComparer.OrdinalIgnoreCase);

            foreach (ControllerDescription controller in pageControllerList)
            {
                foreach (MethodInfo m in controller.ControllerType.GetMethods(ActionBindingFlags))
                {
                    PageUrlAttribute[] pageUrlAttrs = ReflectionExtensions2.GetMyAttributes<PageUrlAttribute>(m);
                    ActionAttribute actionAttr = ReflectionExtensions2.GetMyAttribute<ActionAttribute>(m);

                    if (pageUrlAttrs.Length > 0 && actionAttr != null)
                    {
                        ActionDescription actionDescription =
                            new ActionDescription(m, actionAttr) { PageController = controller };

                        foreach (PageUrlAttribute attr in pageUrlAttrs)
                        {
                            if (string.IsNullOrEmpty(attr.Url) == false)
                                s_PageActionDict.Add(attr.Url, actionDescription);
                        }
                    }
                }
            }


        }

        private static ControllerDescription GetServiceController(string controller)
        {
            if (string.IsNullOrEmpty(controller))
                throw new ArgumentNullException("controller");

            ControllerDescription description = null;


            if (controller.IndexOf('.') > 0)
                s_ServiceFullNameDict.TryGetValue(controller, out description);
            else
                s_ServiceShortNameDict.TryGetValue(controller, out description);

            return description;
        }

        private static ActionDescription GetServiceAction(Type controller, string action, HttpRequest request)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            if (string.IsNullOrEmpty(action))
                throw new ArgumentNullException("action");

            // 首先尝试从缓存中读取
            string key = request.HttpMethod + "#" + controller.FullName + "@" + action;
            ActionDescription mi = (ActionDescription)s_ServiceActionTable[key];

            if (mi == null)
            {
                bool saveToCache = true;

                MethodInfo method = FindAction(action, controller, request);

                if (method == null)
                {
                    if (StringExtensions.IsSame(action,"submit")  && StringExtensions.IsSame(request.HttpMethod,"POST"))
                    {
                        // 自动寻找Action
                        method = FindSubmitAction(controller, request);
                        saveToCache = false;
                    }
                }

                if (method == null)
                    return null;

                ActionAttribute attr = ReflectionExtensions2.GetMyAttribute<ActionAttribute>(method);
                mi = new ActionDescription(method, attr);

                if (saveToCache)
                    s_ServiceActionTable[key] = mi;
            }

            return mi;
        }


        private static MethodInfo FindAction(string action, Type controller, HttpRequest request)
        {
            foreach (MethodInfo method in controller.GetMethods())
            {
                if (StringExtensions.IsSame(method.Name, action)) //method.Name.IsSame(action)
                {
                    if (MethodActionIsMatch(method, request))
                        return method;
                }
            }
            return null;
        }

        private static MethodInfo FindSubmitAction(Type controller, HttpRequest request)
        {
            //string[] keys = request.Form.AllKeys;
            List<string> keys = new List<string>(request.Form.AllKeys);
            foreach (MethodInfo method in controller.GetMethods())
            {
                string key = keys.Find(x => StringExtensions.IsSame(method.Name, x));// method.Name.IsSame(x)
                if (key != null && MethodActionIsMatch(method, request))
                    return method;
            }

            return null;
        }

        private static bool MethodActionIsMatch(MethodInfo method, HttpRequest request)
        {
            ActionAttribute attr= ReflectionExtensions2.GetMyAttribute<ActionAttribute>(method);
            if (attr != null)
            {
                if (attr.AllowExecute(request.HttpMethod))
                    return true;
            }
            return false;
        }

        public static InvokeInfo GetActionInvokeInfo(ControllerActionPair pair, HttpRequest request)
        {
            if (pair == null)
                throw new ArgumentNullException("pair");

            InvokeInfo vkInfo = new InvokeInfo();

            vkInfo.Controller = GetServiceController(pair.Controller);
            if (vkInfo.Controller == null)
                return null;

            vkInfo.Action = GetServiceAction(vkInfo.Controller.ControllerType, pair.Action, request);
            if (vkInfo.Action == null)
                return null;

            if (vkInfo.Action.MethodInfo.IsStatic == false)
            {
                vkInfo.Instance = ReflectionExtensions.FastNew(vkInfo.Controller.ControllerType);
            }

            return vkInfo;
        }

        public static InvokeInfo GetActionInvokeInfo(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            ActionDescription action = null;
            if (s_PageActionDict.TryGetValue(url, out action) == false)
                return null;

            InvokeInfo vkInfo = new InvokeInfo();
            vkInfo.Controller = action.PageController;
            vkInfo.Action = action;

            if (vkInfo.Action.MethodInfo.IsStatic == false)
            {
                vkInfo.Instance = ReflectionExtensions.FastNew(vkInfo.Controller.ControllerType);
            }

            return vkInfo;
        }


        private static Hashtable s_modelTable = Hashtable.Synchronized(
                                            new Hashtable(4096, StringComparer.OrdinalIgnoreCase));

        public static ModelDescripton GetModelDescripton(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string key = type.FullName;
            ModelDescripton mm = (ModelDescripton)s_modelTable[key];

            if (mm == null)
            {
                List<DataMember> list = new List<DataMember>();
                foreach (PropertyInfo p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    list.Add(new PropertyMember(p));
                }
                
                foreach (FieldInfo p in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    list.Add(new FieldMember(p));
                }
                mm = new ModelDescripton { Fields = list };
                s_modelTable[key] = mm;
            }
            return mm;
        }

        public static DataMember GetMemberByName(Type type, string name, bool ifNotFoundThrowException)
        {
            ModelDescripton description = GetModelDescripton(type);
            foreach (DataMember member in description.Fields)
                if (member.Name == name)
                    return member;

            if (ifNotFoundThrowException)
                throw new ArgumentOutOfRangeException(
                        string.Format("指定的成员 {0} 在类型 {1} 中并不存在。", name, type.ToString()));

            return null;
        }




    }
}
