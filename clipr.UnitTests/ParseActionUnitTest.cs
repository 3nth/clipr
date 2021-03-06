﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using clipr.Utils;

namespace clipr.UnitTests
{
    [TestClass]
    public class ParseActionUnitTest
    {
        #region Count Action

        internal class NamedArgumentCount
        {
            [NamedArgument('v', "verbose", Action = ParseAction.Count)]
            public int Verbosity { get; set; }
        }

        [TestMethod]
        public void Argument_WithParseActionCount_AccumulatesCount()
        {
            var opt = CliParser.Parse<NamedArgumentCount>("-v -v -v".Split());
            Assert.AreEqual(3, opt.Verbosity);
        }

        #endregion

        #region StoreTrue Action

        internal class StoreTrueAction
        {
            [NamedArgument('b', Action = ParseAction.StoreTrue)]
            public bool IsSet { get; set; }
        }

        [TestMethod]
        public void Argument_WithStoreTrueAction_StoresBoolAsTrue()
        {
            var opt = CliParser.Parse<StoreTrueAction>("-b".Split());
            Assert.IsTrue(opt.IsSet);
        }

        internal class StoreTrueActionNullable
        {
            [NamedArgument('b', Action = ParseAction.StoreTrue)]
            public bool? IsSet { get; set; }
        }

        [TestMethod]
        public void Argument_WithStoreTrueActionAndNullable_StoresBoolAsTrue()
        {
            var opt = CliParser.Parse<StoreTrueActionNullable>("-b".Split());
            Assert.IsTrue(opt.IsSet.GetValueOrDefault());
        }

        #endregion

        #region StoreFalse Action

        internal class StoreFalseAction
        {
            public StoreFalseAction()
            {
                IsSet = true;
            }

            [NamedArgument('b', Action = ParseAction.StoreFalse)]
            public bool IsSet { get; set; }
        }

        [TestMethod]
        public void Argument_WithStoreFalseAction_StoresBoolAsFalse()
        {
            var opt = CliParser.Parse<StoreFalseAction>("-b".Split());
            Assert.IsFalse(opt.IsSet);
        }

        internal class StoreFalseActionNullable
        {
            [NamedArgument('b', Action = ParseAction.StoreFalse)]
            public bool? IsSet { get; set; }
        }

        [TestMethod]
        public void Argument_WithStoreFalseActionAndNullable_StoresBoolAsFalse()
        {
            var opt = CliParser.Parse<StoreFalseActionNullable>("-b".Split());
            Assert.IsFalse(opt.IsSet.GetValueOrDefault());
        }

        #endregion

        #region Store Action

        internal class StoreValue
        {
            [NamedArgument('s', Action = ParseAction.Store)]
            public int Value { get; set; }
        }

        [TestMethod]
        public void Argument_WithStoreAction_StoresValue()
        {
            var opt = CliParser.Parse<StoreValue>("-s 10".Split());
            Assert.AreEqual(10, opt.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void Argument_WithStoreActionAndInconvertibleValue_ThrowsException()
        {
            CliParser.Parse<StoreValue>("-s bbb".Split());
        }

        #endregion

        #region StoreConst Action

        internal class StoreConst
        {
            [NamedArgument('s', Action = ParseAction.StoreConst, Const = 10)]
            public int Value { get; set; }
        }

        [TestMethod]
        public void Argument_WithStoreConstValue_StoresValue()
        {
            var opt = CliParser.Parse<StoreConst>("-s".Split());
            Assert.AreEqual(10, opt.Value);
        }

        internal class StoreConstWrongConstValue
        {
            [NamedArgument('s', Action = ParseAction.StoreConst, Const = "b")]
            public int Value { get; set; }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void Argument_WithStoreConstValueAndInconvertibleConst_ThrowsException()
        {
            CliParser.Parse<StoreConstWrongConstValue>("-s bbb".Split());
        }

        #endregion

        #region Append Action

        internal class Append
        {
            [NamedArgument('i', Action = ParseAction.Append)]
            public List<int> Values { get; set; }
        }

        [TestMethod]
        public void Append_WithMultipleOfSameArgument_AppendsValues()
        {
            var expected = new List<int> {1, 2, 3};
            var opt = CliParser.Parse<Append>("-i1 -i 2 -i 3".Split());
            CollectionAssert.AreEqual(expected, opt.Values);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void Append_WithOneValueInconvertible_ThrowsException()
        {
            var expected = new List<int> { 1, 2, 3 };
            var opt = CliParser.Parse<Append>("-i1 -i oops -i 3".Split());
            CollectionAssert.AreEqual(expected, opt.Values);
        }

        #endregion

        #region AppendConst Action

        internal class AppendConst
        {
            [NamedArgument('i', Action = ParseAction.AppendConst, Const = 6)]
            public List<int> Values { get; set; }
        }

        [TestMethod]
        public void AppendConst_WithMultipleOfSameArgument_AppendsValues()
        {
            var expected = new List<int> { 6, 6, 6 };
            var opt = CliParser.Parse<AppendConst>("-i -i -i".Split());
            CollectionAssert.AreEqual(expected, opt.Values);
        }

        #endregion
    }
}
