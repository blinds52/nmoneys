﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NMoneys.Change
{
	[CLSCompliant(false)]
	public class OptimalChangeSolution : IEnumerable<QuantifiedDenomination>
	{
		private readonly QuantifiedDenomination[] _denominations;

		internal OptimalChangeSolution(long toChange, Currency operationCurrency, ushort[] table, IntegralDenomination?[] usedDenominations)
		{
			_denominations = new QuantifiedDenomination[0];

			ushort possibleSolution = table.Last();
			if (possibleSolution != ushort.MaxValue)
			{
				// at most as many denominations as used
				List<Denomination> denominations = new List<Denomination>(usedDenominations.Length);
				IntegralDenomination defaultDenomination = IntegralDenomination.Default(operationCurrency);

				long denomination = toChange;
				while (denomination > 0)
				{
					IntegralDenomination usedDenomination = usedDenominations[denomination]
						.GetValueOrDefault(defaultDenomination);
					denominations.Add(usedDenomination.Denomination);
					denomination -= usedDenomination.IntegralAmount;
				}
				_denominations = QuantifiedDenomination.Aggregate(denominations.OrderByDescending(d => d.Value))
					.ToArray();
			}
		}

		public IEnumerator<QuantifiedDenomination> GetEnumerator()
		{
			return _denominations.Cast<QuantifiedDenomination>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public QuantifiedDenomination this[int idx] => _denominations[idx];
		public uint Count => (uint)_denominations.Length;
		public uint TotalCount => (uint)_denominations.Sum(d => d.Quantity);

		public bool IsSolution => Count > 0;
	}
}