using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Uno;
public class FontFamily : Microsoft.UI.Xaml.Media.FontFamily
{
    public FontFamily(string familyName) : base(AssetExtensions.AssetPath(familyName)) 
    { 

    }
}
