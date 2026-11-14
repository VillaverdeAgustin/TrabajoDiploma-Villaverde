namespace Presentacion_575_AV.Properties
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources
    {
        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if ((resourceMan == null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Presentacion_575_AV.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static global::System.Drawing.Bitmap LogoConectAR
        {
            get
            {
                object obj = ResourceManager.GetObject("LogoConectAR", resourceCulture);
                return ((global::System.Drawing.Bitmap)(obj));
            }
        }

        internal static global::System.Drawing.Bitmap IsotipoConectAR
        {
            get
            {
                object obj = ResourceManager.GetObject("IsotipoConectAR", resourceCulture);
                return ((global::System.Drawing.Bitmap)(obj));
            }
        }

        internal static global::System.Drawing.Icon IconoConectAR
        {
            get
            {
                object obj = ResourceManager.GetObject("IconoConectAR", resourceCulture);
                return ((global::System.Drawing.Icon)(obj));
            }
        }

        internal static global::System.Drawing.Bitmap MarcaAguaConectAR
        {
            get
            {
                object obj = ResourceManager.GetObject("MarcaAguaConectAR", resourceCulture);
                return ((global::System.Drawing.Bitmap)(obj));
            }
        }
    }
}
