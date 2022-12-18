using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pirate_War_v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pirate_War_v1.Tests
{
    [TestClass()]
    public class GameTableTests
    {
        [TestMethod()]
        public void aiAllDeadZonesTest()
        {
            GameTable testTable = new GameTable("Test");
            testTable.getCoordinate(1, 1).Value = 1;
            testTable.getCoordinate(1, 2).Value = 1;
            List<Coordinates> actual = testTable.aiAllDeadZones();
            List<Coordinates> expected = new List<Coordinates>();
            expected.Add(testTable.getCoordinate(1,1));
            expected.Add(testTable.getCoordinate(1,2));

            var jsonString = JsonSerializer.Serialize(actual);
            var jsonString2 = JsonSerializer.Serialize(expected);

            Assert.AreEqual(jsonString, jsonString2);
        }
    }
}