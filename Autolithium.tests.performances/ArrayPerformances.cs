using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Autolithium.tests.performances
{
    [TestClass]
    public class ArrayPerformances
    {
        [TestMethod]
        public void ArrayCreationTest()
        {
            Autolithium.core.LiParser.Parse(
@"$t = timerinit()
$a[5]
for $i = 0 to 1000
$a[0] = 'a'
$a[1] = 'b'
$a[2] = 123
$a[3] = 10.54
$a[4] = $t
next
consolewriteerror(timerdiff($t))
"
                ).Compile().DynamicInvoke(new string[] { });
        }

        [TestMethod]
        public void ArrayAccessTest_typeCoherent()
        {
            Autolithium.core.LiParser.Parse(
@"$t = timerinit()
$a[5]
$a[0] = 'a'
$a[1] = 'b'
$a[2] = 123
$a[3] = 10.54
$a[4] = $t
for $i = 0 to 1000
sin($a[3])
next
consolewriteerror(timerdiff($t))
"
                ).Compile().DynamicInvoke(new string[] { });
        }
        [TestMethod]
        public void ArrayAccessTest_Number2Number()
        {
            Autolithium.core.LiParser.Parse(
@"$t = timerinit()
$a[5]
$a[0] = 'a'
$a[1] = 'b'
$a[2] = 123
$a[3] = 10.54
$a[4] = $t
for $i = 0 to 1000
sin($a[2])
next
consolewriteerror(timerdiff($t))
"
                ).Compile().DynamicInvoke(new string[] { });
        }
    }
}
