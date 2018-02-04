namespace StringCalculatorTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NUnit.Framework;

    [TestFixture]
    class StringCalculatorShould
    {

        [Test]
        public void Return_Default_Value_When_Add_Is_Called_With_Empty_String()
        {
            const int DefaultValue = 0;

            var sc = new StringCalculator();

            var result = sc.Add("");

            Assert.That(result, Is.EqualTo(DefaultValue));
        }

        [Test]
        [TestCase("1", 1)]
        [TestCase("2", 2)]
        [TestCase("1,2", 3)]
        [TestCase("1,3", 4)]
        [TestCase("1,2,3", 6)]
        [TestCase("4,5,6", 15)]
        [TestCase("4\n5,6", 15)]
        [TestCase("4\n5\n6", 15)]
        public void Return_Sum_Of_Number_When_Add_Is_Called_With_String(string input, int sum)
        {
            var sc = new StringCalculator();
            
            var result = sc.Add(input);

            Assert.That(result, Is.EqualTo(sum));
        }

        [Test]
        [TestCase("//;\n1;2", 3)]
        [TestCase("//?\n1?2", 3)]
        [TestCase("//?\n1?2?3", 6)]
        public void Return_Sum_Of_Number_When_Add_Is_Called_With_String_Containing_Numbers_Saperated_By_Delimeter(string input, int sum)
        {
            var sc = new StringCalculator();

            var result = sc.Add(input);

            Assert.That(result, Is.EqualTo(sum));
        }

        [Test]
        [TestCase("-1", "negatives not allowed | -1")]
        [TestCase("-2", "negatives not allowed | -2")]
        [TestCase("-1,-2", "negatives not allowed | -1 -2")]
        [TestCase("//?\n-1?-2", "negatives not allowed | -1 -2")]
        [TestCase("//?\n-1?2?-3", "negatives not allowed | -1 -3")]
        public void Throw_When_Add_Is_Called_With_String_Containing_Negative_Numbers(string input, string errorMessage)
        {
            var sc = new StringCalculator();

            var result = Assert.Throws<ArgumentException>(
                () =>
                {
                    var sum = sc.Add(input);
                });

            Assert.That(result.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase("1,1001", 1)]
        [TestCase("2,2002", 2)]
        [TestCase("1,2002,3", 4)]
        public void Return_Sum_Of_Number_Ignoring_Big_Numbers_When_Add_Is_Called_With_String(string input, int sum)
        {
            var sc = new StringCalculator();

            var result = sc.Add(input);

            Assert.That(result, Is.EqualTo(sum));
        }
    }

    internal class StringCalculator
    {
        private const int DEFAULT_VALUE = 0;
        private const string CUSTOM_DELIMITER_INDICATOR = "//";
        private readonly static int CUSTOM_DELIMITER_POSITION = CUSTOM_DELIMITER_INDICATOR.Length;
        private static char[] defaultDelimiters = { ',', '\n' };

        internal int Add(string input)
        {
            if (IsInvalidInput(input))
            {
                return DEFAULT_VALUE;
            }
            if (IsSingleNumber(input))
            {
                return ParseSingleNumber(input); 
            }
            return SumMultipleNumbers(input);
        }

        private static bool IsInvalidInput(string input)
        {
            return string.IsNullOrEmpty(input);
        }

        private static bool IsSingleNumber(string input)
        {
            int number;
            return int.TryParse(input, out number);
        }

        private static int ParseSingleNumber(string input)
        {
            var number = int.Parse(input);

            ThrowIfNegativeNumber(number);

            return number;
        }

        private static int SumMultipleNumbers(string input)
        {
            var numberStrings = GetStringNumbersArray(input);

            var numbers = ConvertStringNumbersToIntNumbers(numberStrings);

            return SumAllValidIntegers(numbers);
        }

        private static int SumAllValidIntegers(IEnumerable<int> numbers)
        {
            return numbers.Where(n => n < 1000).Sum();
        }

        private static string[] GetStringNumbersArray(string input)
        {
            if (input.StartsWith(CUSTOM_DELIMITER_INDICATOR))
            {
                var delimiter = input[CUSTOM_DELIMITER_POSITION];
                var inputNumbers = input.Substring(input.IndexOf('\n'));
                return inputNumbers.Split(delimiter);
            }
            return input.Split(defaultDelimiters);
        }

        private static IEnumerable<int> ConvertStringNumbersToIntNumbers(string[] numberStrings)
        {
            var numbers = numberStrings.Select(ns => int.Parse(ns));
            
            ThrowIfAnyNegativeNumbers(numbers);

            return numbers;
        }

        private static IEnumerable<int> GetNegativeNumbers(IEnumerable<int> numbers)
        {
            return numbers.Where(n => n < 0);
        }

        private static void ThrowIfAnyNegativeNumbers(IEnumerable<int> numbers)
        {
            var negativeNumbers = GetNegativeNumbers(numbers);

            if (negativeNumbers.Count() > 0)
            {
                throw new ArgumentException("negatives not allowed | " + string.Join(" ", negativeNumbers));
            }
        }

        private static void ThrowIfNegativeNumber(int number)
        {
            if (number < 0)
            {
                throw new ArgumentException("negatives not allowed | " + number);
            }
        }
    }
}