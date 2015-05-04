using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common.Tests
{
    [TestFixture]
    public class CommandLineParserTests
    {
        CommandLineParser _parser;

        [SetUp]
        public void SetUp()
        {
            var options = new List<CommandLineOption>()
            {
                new CommandLineOption { ShortNotation = 'l', LongNotation = "longoption", ParameterRequired = false },
                new CommandLineOption { ShortNotation = 'p', LongNotation = "param", ParameterRequired = true },
                new CommandLineOption { ShortNotation = 'o', LongNotation = "other", ParameterRequired = false },
                new CommandLineOption { ShortNotation = 'x', LongNotation = "x", ParameterRequired = false },
            };

            _parser = new CommandLineParser(options);
        }

        [Test]
        public void Parse_LongOptionNoParameter_Found()
        {
            var commandLine = "-longoption";
            _parser.Parse(commandLine.Split(' '));

            string longoption = null;
            var result = _parser.TryGet("longoption", out longoption);

            Assert.IsTrue(result);
            Assert.IsNull(longoption);
        }

        [Test]
        public void Parse_OptionShortNotationNoParameter_Found()
        {
            var commandLine = "l";
            _parser.Parse(commandLine.Split(' '));

            string longoption = null;
            var result = _parser.TryGet("longoption", out longoption);

            Assert.IsTrue(result);
            Assert.IsNull(longoption);
        }

        [Test]
        public void Parse_OptionLongNotationWithParameter_Found()
        {
            var commandLine = "-param parameter";
            _parser.Parse(commandLine.Split(' '));

            string param = null;
            var result = _parser.TryGet("param", out param);

            Assert.IsTrue(result);
            Assert.AreEqual(param, "parameter");
        }

        [Test]
        public void Parse_OptionShortNotationWithParameter_Found()
        {
            var commandLine = "p parameter";
            _parser.Parse(commandLine.Split(' '));

            string param = null;
            var result = _parser.TryGet("param", out param);

            Assert.IsTrue(result);
            Assert.AreEqual(param, "parameter");
        }

        [TestCase("-longoption -param parameter")]
        [TestCase("-param parameter -longoption")]
        public void Parse_LongNoParameterAndWithParameterMix_Found(string commandLine)
        {
            _parser.Parse(commandLine.Split(' '));

            string param = null, longoption = null;

            var resultParam = _parser.TryGet("param", out param);
            var resultLongOpt = _parser.TryGet("longoption", out longoption);

            Assert.IsTrue(resultParam);
            Assert.IsTrue(resultLongOpt);
            Assert.AreEqual(param, "parameter");
            Assert.IsNull(longoption);
        }

        [TestCase("l p parameter")]
        [TestCase("p parameter l")]
        [TestCase("lp parameter")]
        public void Parse_ShortNoParameterAndWithParameterMix_Found(string commandLine)
        {
            _parser.Parse(commandLine.Split(' '));

            string param = null, longoption = null;

            var resultParam = _parser.TryGet("param", out param);
            var resultLongOpt = _parser.TryGet("longoption", out longoption);

            Assert.IsTrue(resultParam);
            Assert.IsTrue(resultLongOpt);
            Assert.AreEqual(param, "parameter");
            Assert.IsNull(longoption);
        }

        [TestCase("x")]
        [TestCase("-x")]
        public void Parse_ShortAndLongIdentical_Found(string commandLine)
        {
            _parser.Parse(commandLine.Split(' '));

            string param = null;
            var resultParam = _parser.TryGet("x", out param);

            Assert.IsTrue(resultParam);
            Assert.IsNull(param);
        }
    }
}
