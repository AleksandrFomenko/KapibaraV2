using Autodesk.Revit.DB;

namespace KapibaraCore.Parameters
{
    internal static class InternalUnits
    {
        public static double Convert(Parameter parameter, double value)
        {
#if REVIT2021_OR_GREATER

            ForgeTypeId unitType = parameter.GetUnitTypeId();

            if (unitType.Equals(UnitTypeId.Millimeters))
            {
                return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters);
            }
            if (unitType.Equals(UnitTypeId.Centimeters))
            {
                return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Centimeters);
            }
            if (unitType.Equals(UnitTypeId.Meters))
            {
                return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Meters);
            }
            if (unitType.Equals(UnitTypeId.CubicMeters))
            {
                return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.CubicMeters);
            }
            if (unitType.Equals(UnitTypeId.SquareMeters))
            {
                return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.SquareMeters);
            }

            return value;
#else
            DisplayUnitType unitType = parameter.DisplayUnitType;

            switch (unitType)
            {
                case DisplayUnitType.DUT_MILLIMETERS:
                    return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_MILLIMETERS);

                case DisplayUnitType.DUT_CENTIMETERS:
                    return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_CENTIMETERS);

                case DisplayUnitType.DUT_METERS:
                    return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_METERS);

                case DisplayUnitType.DUT_CUBIC_METERS:
                    return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_CUBIC_METERS);

                case DisplayUnitType.DUT_SQUARE_METERS:
                    return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_SQUARE_METERS);

                default:
                    return value;
            }
#endif
        }
    }
}
