using System;
using System.Windows.Media;

namespace TinyMetroWpfLibrary.Utility
{
    public class SignalColorMap : ColorMapBase
    {
        public override int GetInt32Color(double byValue)
        {
            int iValue = Math.Abs((int)byValue);
            int SignalSameColorCount = (int)Constants.MAX_COLOR_LEGEND_SIGNAL_VALUE / Constants.COLOR_LEGEND_COLOR_BLOCK_COUNT;
            iValue = (iValue / SignalSameColorCount);
            return (int)ColorsDefine[iValue].Int32Value;
        }


        public override Color GetColorByColorLegendIndex(int index)
        {
            int SignalSameColorCount = (int)Constants.MAX_COLOR_LEGEND_SIGNAL_VALUE / Constants.COLOR_LEGEND_COLOR_BLOCK_COUNT;
            int iValue = (index / SignalSameColorCount);
            return ColorsDefine[iValue].ColorValue;
        }


        private static SignalColorMap _instance;
        public static SignalColorMap Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SignalColorMap(); 
                }
                return _instance;
            }
        }
    }
}
