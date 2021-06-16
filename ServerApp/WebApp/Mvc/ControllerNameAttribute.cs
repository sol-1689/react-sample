using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    /// <summary>
    /// コントローラ名を変更する。
    /// ActionNameAttributeのコントローラ版。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerNameAttribute : Attribute
    {
        public string Name { get; }

        public ControllerNameAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// ControllerNameAttributeを使用したConvention。
    /// StartupのAddMvcで設定する。
    /// </summary>
    public class ControllerNameAttributeConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNameAttribute = controller.Attributes.OfType<ControllerNameAttribute>().SingleOrDefault();
            if (controllerNameAttribute != null)
            {
                controller.ControllerName = controllerNameAttribute.Name;
            }
        }
    }

}
