﻿using System;
using System.Linq;
using NMoneys.Change;
using NMoneys.Extensions;
using NUnit.Framework;

namespace NMoneys.Tests.Change
{
	[TestFixture]
	public class MinChangeTester
	{
		[Test]
		[TestCase(0), TestCase(-1)]
		public void MinChange_NotPositive_Exception(decimal notPositive)
		{
			Denomination[] any = { new Denomination(1m) };

			Assert.That(()=> new Money(notPositive).MinChange(any), Throws.InstanceOf<ArgumentOutOfRangeException>());
		}

		[Test]
		public void MinChange_SameDenominationAsAmount_OneDenominationAndNoRemainder()
		{
			var fiveX = new Money(5m);
			var fiver = new Denomination(5m);
			var oneFiver = fiveX.MinChange(fiver);

			Assert.That(oneFiver, Has.Count.EqualTo(1));
			Assert.That(oneFiver[0].Quantity, Is.EqualTo(1u));
			Assert.That(oneFiver[0].Denomination, Is.EqualTo(fiver));
			Assert.That(oneFiver.Remainder, Is.EqualTo(0m.Xxx()));
		}

		[Test]
		public void MinChange_LessAmountThanMinimalDenomination_AmountRemainder()
		{
			var threeX = new Money(3m);
			var noSolution = threeX.MinChange(5m);

			Assert.That(noSolution, Has.Count.EqualTo(0));
			Assert.That(noSolution.Remainder, Is.EqualTo(threeX));
		}

		[Test]
		public void MinChange_WhenRemainder_RemainderSameCurrencyAsInitial()
		{
			var threeEuro = 3m.Eur();
			var noSolution = threeEuro.MinChange(5m);

			Assert.That(noSolution.Remainder.CurrencyCode, Is.EqualTo(threeEuro.CurrencyCode));
		}

		[Test]
		public void MinChange_ChangePossible_MultipleDenominationsNoRemainder()
		{
			var changeable = new Money(5m);
			var wholeSolution = changeable.MinChange(1m, 3m, 2m);

			Assert.That(wholeSolution, Has.Count.EqualTo(2));
			Assert.That(wholeSolution.Remainder, Is.EqualTo(0m.Xxx()));

			Assert.That(wholeSolution[0].Quantity, Is.EqualTo(1u));
			Assert.That(wholeSolution[0].Denomination.Value, Is.EqualTo(3m));

			Assert.That(wholeSolution[1].Quantity, Is.EqualTo(1u));
			Assert.That(wholeSolution[1].Denomination.Value, Is.EqualTo(2m));
		}

		private static readonly object[] greedySuboptimal =
		{
			new object[]
			{
				30m, new[] {1m, 15m, 25m}, new[]
				{
					Tuple.Create(1u, 25m), Tuple.Create(5u, 1m)
				}
			},
			new object[]
			{
				40m, new[] {1m, 5m, 10m, 20m, 25m}, new[]
				{
					Tuple.Create(1u, 25m), Tuple.Create(1u, 10m), Tuple.Create(1u, 5m)
				}
			},
			new object[]
			{
				6m, new[] {1m, 3m, 4m}, new[]
				{
					Tuple.Create(1u, 4m), Tuple.Create(2u, 1m)
				}
			}
		};

		[Test, TestCaseSource(nameof(greedySuboptimal))]
		public void MinChange_NonOptimalForGreedy_SubOptimalSolution(decimal amount, decimal[] denominationValues, Tuple<uint, decimal>[] solution)
		{
			var changeable = new Money(amount);

			var subOptimalSolution = changeable.MinChange(denominationValues);

			Assert.That(subOptimalSolution, Has.Count.EqualTo(solution.Length));
			Assert.That(subOptimalSolution.Remainder, Is.EqualTo(0m.Xxx()));

			for (int i = 0; i < solution.Length; i++)
			{
				Assert.That(subOptimalSolution[i].Quantity, Is.EqualTo(solution[i].Item1));
				Assert.That(subOptimalSolution[i].Denomination.Value, Is.EqualTo(solution[i].Item2));
			}
		}

		[Test]
		public void MinChange_NotCompleteSolution_QuantifiedAndRemainder()
		{
			var notCompletelyChangeable = new Money(7m);

			var subOptimalSolution = notCompletelyChangeable.MinChange(4m, 2m);

			Assert.That(subOptimalSolution, Has.Count.EqualTo(2));
			Assert.That(subOptimalSolution.Remainder, Is.EqualTo(1m.Xxx()));

			Assert.That(subOptimalSolution[0].Quantity, Is.EqualTo(1u));
			Assert.That(subOptimalSolution[0].Denomination.Value, Is.EqualTo(4m));

			Assert.That(subOptimalSolution[1].Quantity, Is.EqualTo(1u));
			Assert.That(subOptimalSolution[1].Denomination.Value, Is.EqualTo(2m));
		}

		[Test]
		public void MinChange_NoDenominations_EmptySolution()
		{
			var toBeChanged = new Money(5m);

			var emptySolution = toBeChanged.MinChange(Enumerable.Empty<Denomination>());

			Assert.That(emptySolution, Is.Empty);
			Assert.That(emptySolution.Count, Is.EqualTo(0));

			Assert.That(emptySolution.Remainder, Is.EqualTo(toBeChanged));
		}
	}
}