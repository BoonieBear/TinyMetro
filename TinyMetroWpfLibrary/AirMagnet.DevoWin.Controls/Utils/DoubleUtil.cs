using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirMagnet.AircheckWifiTester.Controls
{
    public class DoubleUtil
    {
        // Fields 浮点型的误差
        private const double DOUBLE_DELTA = 1E-06;

        public static bool AreEqual(double value1, double value2)
        {
            return (value1 == value2) || Math.Abs(value1 - value2) < DOUBLE_DELTA;
        }

        public static bool GreaterThan(double value1, double value2)
        {
            return ((value1 > value2) && !AreEqual(value1, value2));
        }

        public static bool GreaterThanOrEqual(double value1, double value2)
        {
            return (value1 > value2) || AreEqual(value1, value2);
        }

        public static bool IsZero(double value)
        {
            return (Math.Abs(value) < DOUBLE_DELTA);
        }

        public static bool LessThan(double value1, double value2)
        {
            return ((value1 < value2) && !AreEqual(value1, value2));
        }

        public static bool LessThanOrEqual(double value1, double value2)
        {
            return (value1 < value2) || AreEqual(value1, value2);
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 > value2)
            {
                return true;
            }
            else
            {
                return DoubleUtil.AreClose(value1, value2);
            }
        }

        public static bool AreClose(double value1, double value2)
        {
            if (value1 != value2)
            {
                double num = (Math.Abs(value1) + Math.Abs(value2) + 10) * 2.22044604925031E-16;
                double num1 = value1 - value2;
                if (-num >= num1)
                {
                    return false;
                }
                else
                {
                    return num > num1;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
