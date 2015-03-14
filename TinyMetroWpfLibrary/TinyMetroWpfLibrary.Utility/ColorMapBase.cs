using System.Windows.Media;

namespace TinyMetroWpfLibrary.Utility
{
    public class ColorMapBase
    {
        protected struct ColorItem
        {
            public  Color ColorValue;
            public uint Int32Value;
            public ColorItem(Color cValue, uint iValue)
            {
                ColorValue = cValue;
                Int32Value = iValue;
            }
        }
        public static int Color2Int(Color color)
        {
            int argb = 0;
            argb |= color.A << 24;
            argb |= color.R << 16;
            argb |= color.G << 8;
            argb |= color.B;
            return argb;
        }

        public static Color Int2Color(int intColor)
        {

            return Color.FromArgb((byte)((intColor >> 0x18) & 0xff),
                           (byte)((intColor >> 0x10) & 0xff),
                           (byte)((intColor >> 8) & 0xff),
                           (byte)(intColor & 0xff));
        }
        

        public virtual Color GetColorByColorLegendIndex(int byValue) { return Color.FromArgb(0, 0, 0, 0); }
        public virtual int GetInt32Color(double byValue) { return Constants.COLOR_LEGEND_TransParentInt32Color; }
        public virtual Brush GetBrush(double byValue) 
        {
            int iColor = GetInt32Color(byValue);
            return new SolidColorBrush(Int2Color(iColor));
        }

        protected ColorItem[] ColorsDefine = new ColorItem[Constants.COLOR_LEGEND_COLOR_BLOCK_COUNT + 1]
        {
            new ColorItem(Color.FromRgb(1,50,187),  0xff0132bb), 
            new ColorItem(Color.FromRgb(0,101,202), 0xff0065ca),
            new ColorItem(Color.FromRgb(0,151,219), 0xff0097db),
            new ColorItem(Color.FromRgb(0,200,233), 0xff00c8e9),
            new ColorItem(Color.FromRgb(4,229,247), 0xff04e5f7),
            new ColorItem(Color.FromRgb(2,255,255), 0xff02ffff),
            new ColorItem(Color.FromRgb(1,252,207), 0xff01fccf),
            new ColorItem(Color.FromRgb(1,254,161), 0xff01fea1),
            new ColorItem(Color.FromRgb(2,252,126), 0xff02fc7e),
            new ColorItem(Color.FromRgb(2,253,78),  0xff02fd4e),
            new ColorItem(Color.FromRgb(12,254,0),  0xff0cfe00),
            new ColorItem(Color.FromRgb(88,253,0),  0xff58fd00),
            new ColorItem(Color.FromRgb(141,255,4), 0xff8dff04),
            new ColorItem(Color.FromRgb(198,254,5), 0xffc6fe05),
            new ColorItem(Color.FromRgb(229,249,27),0xffe5f91b),
            new ColorItem(Color.FromRgb(234,255,0), 0xffeaff00),
            new ColorItem(Color.FromRgb(239,226,0), 0xffefe200),
            new ColorItem(Color.FromRgb(247,199,15),0xfff7c70f),
            new ColorItem(Color.FromRgb(252,180,16),0xfffcb410),
            new ColorItem(Color.FromRgb(241,169,58),0xfff1a93a),
            new ColorItem(Color.FromRgb(241,169,58),0xfff1a93a),
        };
     
    }
}
