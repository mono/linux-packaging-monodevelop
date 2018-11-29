using System.Collections.Generic;
using RefactoringEssentials.CSharp.Diagnostics;
using Xunit;

namespace RefactoringEssentials.Tests.CSharp.Diagnostics
{
    public class ReplaceWithSingleCallToLinqTests : CSharpDiagnosticTestBase
    {
        static readonly List<object[]> replaceableMethods = new List<object[]>
        {
            new object[] { "Any" } ,
            new object[] { "Count" } ,
            new object[] { "First" } ,
            new object[] { "FirstOrDefault" } ,
            new object[] { "Last" } ,
            new object[] { "LastOrDefault" } ,
            new object[] { "LongCount" } ,
            new object[] { "Single" } ,
            new object[] { "SingleOrDefault" } ,
        };

        public static IEnumerable<object[]> ReplaceableMethods => replaceableMethods;

        [Theory]
        [MemberData(nameof(ReplaceableMethods))]
        public void TestSimpleCase(string method)
        {
            string input = @"using System.Linq;
public class CSharpDemo {
    public void Bla () {
        int[] arr;
        var bla = $arr.Where(x => x < 4)." + method + @"()$;
    }
}";

            Analyze<ReplaceWithSingleCallToLinqAnalyzer>(input, @"using System.Linq;
public class CSharpDemo {
    public void Bla () {
        int[] arr;
        var bla = arr." + method + @"(x => x < 4);
    }
}");
        }

        [Theory]
        [MemberData(nameof(ReplaceableMethods))]
        public void TestComplexCase(string method)
        {
            string input = @"using System.Linq;
public class CSharpDemo {
    public void Bla () {
        int[] arr;
        var bla = arr.Where(x => x < 4)." + method + @"(x => x < 4);
    }
}";
            Analyze<ReplaceWithSingleCallToLinqAnalyzer>(input);
        }

        [Theory]
        [MemberData(nameof(ReplaceableMethods))]
        public void TestDisable(string method)
        {
            string input = @"using System.Linq;
public class CSharpDemo {
    public void Bla () {
        int[] arr;
#pragma warning disable " + CSharpDiagnosticIDs.ReplaceWithSingleCallToLinqAnalyzerID + @"
        var bla = arr.Where (x => x < 4)." + method + @" ();
    }
}";

            Analyze<ReplaceWithSingleCallToLinqAnalyzer>(input);
        }
    }
}
