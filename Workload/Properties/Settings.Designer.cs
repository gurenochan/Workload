﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Workload.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ТЕФ")]
        public string Facility {
            get {
                return ((string)(this["Facility"]));
            }
            set {
                this["Facility"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Автоматизації теплоенергетичних процесів")]
        public string Department {
            get {
                return ((string)(this["Department"]));
            }
            set {
                this["Department"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>ТЕФ</string>
  <string>ФЕЛ</string>
  <string>ІПСА</string>
  <string>ФЕА</string>
  <string>ФІОТ</string>
  <string>ФММ</string>
  <string>ІФФ</string>
  <string>ММІ</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection FacilitiesList {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FacilitiesList"]));
            }
            set {
                this["FacilitiesList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>Теоретичної і промислової теплотехніки</string>
  <string>АЕС та інженерної теплофізики</string>
  <string>Теплоенергетичних установок теплових і АЕС</string>
  <string>Автоматизації теплоенергетичних процесів</string>
  <string>Автоматизації проектування енергетичих процесів і систем</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection DepartmentsList {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["DepartmentsList"]));
            }
            set {
                this["DepartmentsList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowTalisman {
            get {
                return ((bool)(this["ShowTalisman"]));
            }
            set {
                this["ShowTalisman"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        public decimal MaxHoursPerTeacher {
            get {
                return ((decimal)(this["MaxHoursPerTeacher"]));
            }
            set {
                this["MaxHoursPerTeacher"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("68, 155, 164")]
        public global::System.Drawing.Color AccentColor {
            get {
                return ((global::System.Drawing.Color)(this["AccentColor"]));
            }
        }
    }
}
