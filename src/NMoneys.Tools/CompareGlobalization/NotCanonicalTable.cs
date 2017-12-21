﻿using System.Linq;
using NMoneys.Support;

namespace NMoneys.Tools.CompareGlobalization
{
	internal sealed class NotCanonicalTable : ComparisonTable
	{
		public NotCanonicalTable() : base("Code", "Globalization") { }

		protected override void BuildTable(GlobalizationCurrencyInfo[] globalizationInfo, CurrencyInfo[] configurationInfo)
		{
			var notDecorated = configurationInfo
				.Where(i => !Enumeration.HasAttribute<CurrencyIsoCode, CanonicalCultureAttribute>(i.Code))
				.ToArray();

			foreach (var fromConfiguration in notDecorated)
			{
				var fromGlobalization = globalizationInfo.Where(i => Currency.Code.Comparer.Equals(i.Info.Code, fromConfiguration.Code)).ToArray();
				for (int i = 0; i < fromGlobalization.Length; i++)
				{
					if (i == 0)
					{
						AddRow(fromConfiguration.Code, string.Empty);
					}
					string fromGlobalizationColumn = $"{fromGlobalization[i].Culture.Name} [{fromGlobalization[i].Culture.EnglishName}]";
					AddRow(string.Empty, fromGlobalizationColumn);
				}
			}
		}
	}
}