/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Awesome.Library.Utilities {

	public enum WeightUnit {
		Gram,
		KiloGram,
		Grain,
		Ounce,
		OunceTroy,
		Pound,
		Carat,
		Dram,
		Stone,
		Pennyweight
	}

	public enum MeasureUnit {
		MicroMeter,
		Millimeter,
		Centimeter,
		Meter,
		KiloMeter,
		Foot,
		Inch,
		Furlong,
		Leauge,
		Mile,
		Yard
	}

	public static class UnitConversion {

		/// <summary>
		/// Converts the input weight in specified input units to the output unit type
		/// </summary>
		/// <param name="weight">The measurement</param>
		/// <param name="inputUnit">The unit type of the inputted weight</param>
		/// <param name="outputUnit">The desired output weight once converted</param>
		/// <returns>The converted weight</returns>
		public static decimal ConvertWeight( decimal inputWeight, WeightUnit inputUnit, WeightUnit outputUnit ) {
			decimal baseNumber = 0;
			switch ( inputUnit ) {
				case WeightUnit.Gram: baseNumber = inputWeight; break;
				case WeightUnit.KiloGram: baseNumber = inputWeight * 1000; break;
				case WeightUnit.Grain: baseNumber = inputWeight * 0.064798903M; break;
				case WeightUnit.Ounce: baseNumber = inputWeight * 28.349523125M; break;
				case WeightUnit.OunceTroy: baseNumber = inputWeight * 31.1034768M; break;
				case WeightUnit.Pound: baseNumber = inputWeight * 453.59237M; break;
				case WeightUnit.Carat: baseNumber = inputWeight * 0.2M; break;
				case WeightUnit.Dram: baseNumber = inputWeight * 0.5643833911933M; break;
				case WeightUnit.Stone: baseNumber = inputWeight * 6350.29318M; break;
				case WeightUnit.Pennyweight: baseNumber = inputWeight * 1.55517384M; break;
			}
			switch ( outputUnit ) {
				case WeightUnit.Gram: return baseNumber;
				case WeightUnit.KiloGram: return baseNumber / 1000;
				case WeightUnit.Grain: return baseNumber / 0.064798903M;
				case WeightUnit.Ounce: return baseNumber / 28.349523125M;
				case WeightUnit.OunceTroy: return baseNumber / 31.1034768M;
				case WeightUnit.Pound: return baseNumber / 453.59237M;
				case WeightUnit.Carat: return baseNumber / 0.2M;
				case WeightUnit.Dram: return baseNumber / 0.5643833911933M;
				case WeightUnit.Stone: return baseNumber / 6350.29318M;
				case WeightUnit.Pennyweight: return baseNumber / 1.55517384M;
			}
			return 0;
		}

		/// <summary>
		/// Converts the input measure in specified input units to the output unit type
		/// </summary>
		public static decimal ConvertMeasure( decimal inputMeasure, MeasureUnit inputUnit, MeasureUnit outputUnit ) {
			decimal baseNumber = 0;
			switch ( inputUnit ) {
				case MeasureUnit.MicroMeter: baseNumber = inputMeasure / 10000; break;
				case MeasureUnit.Millimeter: baseNumber = inputMeasure / 10; break;
				case MeasureUnit.Centimeter: baseNumber = inputMeasure; break;
				case MeasureUnit.Meter: baseNumber = inputMeasure * 100; break;
				case MeasureUnit.KiloMeter: baseNumber = inputMeasure * 10000; break;
				case MeasureUnit.Inch: baseNumber = inputMeasure * 2.54M; break;
				case MeasureUnit.Foot: baseNumber = inputMeasure * 30.48M; break;
				case MeasureUnit.Furlong: baseNumber = inputMeasure * 20116.79M; break;
				case MeasureUnit.Leauge: baseNumber = inputMeasure * 555600; break;
				case MeasureUnit.Mile: baseNumber = inputMeasure * 160934.4M; break;
				case MeasureUnit.Yard: baseNumber = inputMeasure * 91.439M; break;
			}
			switch ( outputUnit ) {
				case MeasureUnit.MicroMeter: return baseNumber * 10000;
				case MeasureUnit.Millimeter: return baseNumber * 10;
				case MeasureUnit.Centimeter: return baseNumber;
				case MeasureUnit.Meter: return baseNumber / 100;
				case MeasureUnit.KiloMeter: return baseNumber / 10000;
				case MeasureUnit.Inch: return baseNumber / 2.54M;
				case MeasureUnit.Foot: return baseNumber / 30.48M;
				case MeasureUnit.Furlong: return baseNumber / 20116.79M;
				case MeasureUnit.Leauge: return baseNumber / 555600;
				case MeasureUnit.Mile: return baseNumber / 160934.4M;
				case MeasureUnit.Yard: return baseNumber / 91.439M;
			}
			return 0;
		}

	}

}

