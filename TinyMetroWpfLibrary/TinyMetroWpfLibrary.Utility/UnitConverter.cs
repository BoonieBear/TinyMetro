namespace TinyMetroWpfLibrary.Utility
{
    public class UnitConverter
    {
        public static double ConverterMeterToFeet(double meter)
        {
            return meter * Constants.GLOBAL_FEET_METER;
        }

        public static double ConverterFeetToMeter(double feet)
        {
            return feet / Constants.GLOBAL_FEET_METER;
        }
    }
}
