using FluentAssertions;
using NUnit.Framework;
using System;

namespace EveryeyeFeed.Test
{
    public class DateTests
    {
        [Test]
        public void GetEveryeyeDate_News()
        {
            var date = Helpers.GetEveryeyeDate("11:42, 27 Febbraio 2022");

            date.Should().Be(new DateTime(2022, 2, 27, 11, 42, 0));
        }

        [Test]
        public void GetEveryeyeDate_Article()
        {
            var date = Helpers.GetEveryeyeDate("27 Febbraio 2022");

            date.Should().Be(new DateTime(2022, 2, 27, 0, 0, 0));
        }
    }
}