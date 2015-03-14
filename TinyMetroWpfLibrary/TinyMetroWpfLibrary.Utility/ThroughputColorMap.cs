using System;
using System.Windows.Media;

namespace TinyMetroWpfLibrary.Utility 
{
    public class ThroughputColorMap : ColorMapBase
    {
        

        public override int GetInt32Color(double byValue)
        {
            int index = ConverterThroughputToIndex(byValue);
            if (index >= 0)
            {
                return (int)ColorsDefine[index].Int32Value;
            }
            else
            {
                return Constants.COLOR_LEGEND_TransParentInt32Color;
            }
        }

        public override Color GetColorByColorLegendIndex(int index)
        {
            int ThroughputSameColorCount = (int)Constants.COLOR_LEGEND_MAX_COLOR_INDEX / Constants.COLOR_LEGEND_COLOR_BLOCK_COUNT;
            int iValue = (index / ThroughputSameColorCount);
            return ColorsDefine[iValue].ColorValue;
        }

        private double[] _throughputTicks = new double[21] 
        { 
            Constants.MAX_COLOR_LEGEND_THROUTHPUT_VALUE, 
                300, 
            200, 
                100, 
            90,
                80,    
            70,
                60,
            50,
                40,
            30,
                20,
            15,
                10,
            5,
                4,
            3,
                2,
            1,
                0.5,            
            Constants.MIN_COLOR_LEGEND_THROUGHPUT_VALUE 
        };

        public double ConverterIndexToThroughput(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            int ThroughputSameColorCount = (int)Constants.COLOR_LEGEND_MAX_COLOR_INDEX / Constants.COLOR_LEGEND_COLOR_BLOCK_COUNT;
            int ColorMapindex = (int)(index / ThroughputSameColorCount);

            if (ColorMapindex >= _throughputTicks.Length-1)
            {
                return _throughputTicks[_throughputTicks.Length - 1];
            }
            if (index % ThroughputSameColorCount == 0)
            {
                return _throughputTicks[ColorMapindex];
            }
            else
            {
                double rangeUpperValue = _throughputTicks[ColorMapindex];
                double rangeLowerValue = _throughputTicks[ColorMapindex + 1];
                double range = rangeUpperValue - rangeLowerValue;
                double disteny = range / ThroughputSameColorCount;
                return rangeLowerValue + (ThroughputSameColorCount-(index % ThroughputSameColorCount)) * disteny;
            }
            
        }

        public int ConverterThroughputToIndex(double throughput)
        {
            if (throughput <= 0)
            {
                return -1;
            }

            if (throughput > Constants.MAX_COLOR_LEGEND_THROUTHPUT_VALUE)
            {
                return 0;
            }

            int index = 0;
            for (index = _throughputTicks.Length - 1; index > 0; index--)
            {
                if (throughput > _throughputTicks[index] && throughput <= _throughputTicks[index - 1])
                {
                    break;
                }
            }

            index = Math.Max(0, Math.Min(index - 1, _throughputTicks.Length - 2));
            return index;
        }

        private static ThroughputColorMap _instance;
        public static ThroughputColorMap Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ThroughputColorMap();
                }
                return _instance;
            }
        }
    }
}
