﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCDiscografia.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.5.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://api.discogs.com")]
        public string DiscogsBaseURL {
            get {
                return ((string)(this["DiscogsBaseURL"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("discografia.mx/1.0")]
        public string DiscogsUserAgent {
            get {
                return ((string)(this["DiscogsUserAgent"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DXOHuxEAoxtmsQEKXkFn")]
        public string DiscogsConsumerKey {
            get {
                return ((string)(this["DiscogsConsumerKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("YjwtZniyLIiCRPCMfVCriLUKdybHnFxX")]
        public string DiscogsConsumerSecret {
            get {
                return ((string)(this["DiscogsConsumerSecret"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/releases/")]
        public string DiscogsGetReleaseUri {
            get {
                return ((string)(this["DiscogsGetReleaseUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//code.jquery.com/jquery-#version#.min.js")]
        public string jQueryCdn {
            get {
                return ((string)(this["jQueryCdn"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//cdnjs.cloudflare.com/ajax/libs/jquery-validate/#version#/jquery.validate.min.js" +
            "")]
        public string jQueryValidateCdn {
            get {
                return ((string)(this["jQueryValidateCdn"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//cdnjs.cloudflare.com/ajax/libs/modernizr/#version#/modernizr.min.js")]
        public string ModernizrCdn {
            get {
                return ((string)(this["ModernizrCdn"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//stackpath.bootstrapcdn.com/bootstrap/#version#/js/bootstrap.min.js")]
        public string BootstrapJsCdn {
            get {
                return ((string)(this["BootstrapJsCdn"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//stackpath.bootstrapcdn.com/bootstrap/#version#/css/bootstrap.min.css")]
        public string BootstrapCssCdn {
            get {
                return ((string)(this["BootstrapCssCdn"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/#version#/jquery.v" +
            "alidate.unobtrusive.min.js")]
        public string ValidateUnobtrusiveCdn {
            get {
                return ((string)(this["ValidateUnobtrusiveCdn"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//cdnjs.cloudflare.com/ajax/libs/respond.js/#version#/respond.min.js")]
        public string RespondCdn {
            get {
                return ((string)(this["RespondCdn"]));
            }
        }
    }
}
