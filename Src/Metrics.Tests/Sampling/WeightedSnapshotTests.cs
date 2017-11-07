﻿using System;
using System.Linq;
using FluentAssertions;
using Metrics.Sampling;
using Xunit;

namespace Metrics.Tests.Sampling
{
    public class WeightedSnapshotTests
    {
        private readonly WeightedSnapshot snapshot = MakeSanpshot(new long[] { 5, 1, 2, 3, 4 }, new double[] { 1, 2, 3, 2, 2 });

        private static WeightedSnapshot MakeSanpshot(long[] values, double[] weights)
        {
            if (values.Length != weights.Length)
            {
                throw new ArgumentException("values and weights must have same number of elements");
            }

            var samples = Enumerable.Range(0, values.Length).Select(i => new WeightedSample(values[i], null, weights[i]));

            return new WeightedSnapshot(values.Length, samples);
        }


        [Fact]
        public void WeightedSnapshot_SmallQuantilesAreTheFirstValue()
        {
            snapshot.GetValue(0.0).Should().Be(1.0);
        }

        [Fact]
        public void WeightedSnapshot_BigQuantilesAreTheLastValue()
        {
            snapshot.GetValue(1.0).Should().Be(5.0);
        }

        [Fact]
        public void WeightedSnapshot_HasAMedian()
        {
            snapshot.Median.Should().Be(3.0);
        }

        [Fact]
        public void WeightedSnapshot_HasAp75()
        {
            snapshot.Percentile75.Should().Be(4.0);
        }

        [Fact]
        public void WeightedSnapshot_HasAp95()
        {
            snapshot.Percentile95.Should().Be(5.0);
        }

        [Fact]
        public void WeightedSnapshot_HasAp98()
        {
            snapshot.Percentile98.Should().Be(5.0);
        }

        [Fact]
        public void WeightedSnapshot_HasAp99()
        {
            snapshot.Percentile99.Should().Be(5.0);
        }

        [Fact]
        public void WeightedSnapshot_HasAp999()
        {
            snapshot.Percentile999.Should().Be(5.0);
        }

        [Fact]
        public void WeightedSnapshot_HasValues()
        {
            snapshot.Values.Should().Equal(new long[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void WeightedSnapshot_HasSize()
        {
            snapshot.Size.Should().Be(5);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesTheMinimumValue()
        {
            snapshot.Min.Should().Be(1);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesTheMaximumValue()
        {
            snapshot.Max.Should().Be(5);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesTheMeanValue()
        {
            snapshot.Mean.Should().Be(2.7);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesTheStdDev()
        {
            snapshot.StdDev.Should().BeApproximately(1.2688, 0.0001);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesAMinOfZeroForAnEmptySnapshot()
        {
            Snapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.Min.Should().Be(0);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesAMaxOfZeroForAnEmptySnapshot()
        {
            Snapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.Max.Should().Be(0);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesAMeanOfZeroForAnEmptySnapshot()
        {
            Snapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.Mean.Should().Be(0);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesAStdDevOfZeroForAnEmptySnapshot()
        {
            Snapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.StdDev.Should().Be(0);
        }

        [Fact]
        public void WeightedSnapshot_CalculatesAStdDevOfZeroForASingletonSnapshot()
        {
            Snapshot snapshot = MakeSanpshot(new long[0], new double[0]);
            snapshot.StdDev.Should().Be(0);
        }

        [Fact]
        public void WeightedSnapshot_ThrowsOnBadQuantileValue()
        {
            ((Action)(() => snapshot.GetValue(-0.5))).Should().Throw<ArgumentException>();
            ((Action)(() => snapshot.GetValue(1.5))).Should().Throw<ArgumentException>();
            ((Action)(() => snapshot.GetValue(double.NaN))).Should().Throw<ArgumentException>();
        }
    }
}
