using System.Collections.Generic;
using System.Linq;
using RefactoringEssentials.CSharp.Diagnostics;
using Xunit;

namespace RefactoringEssentials.Tests.CSharp.Diagnostics
{
    public class ReplaceWithOfTypeLinqTests : CSharpDiagnosticTestBase
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
            new object[] { "Where" } ,
        };

        public static IEnumerable<object[]> ReplaceableMethods => replaceableMethods;

        [Theory]
        [MemberData(nameof(ReplaceableMethods))]
        public void TestCaseBasicWithFollowUpExpresison(string method)
        {
            Analyze<ReplaceWithOfTypeLinqAnalyzer>(@"using System.Linq;
class Test
{
    public bool Foo(object[] obj)
    {
        $obj.Select(q => q as Test)." + method + @"(q => q != null && Foo(new object[] { q }))$;
        return true;
    }
}", @"using System.Linq;
class Test
{
    public bool Foo(object[] obj)
    {
        obj.OfType<Test>()." + method + @"(q => Foo(new object[] { q }));
        return true;
    }
}");
        }

        [Theory]
        [MemberData(nameof(ReplaceableMethods))]
        public void TestDisable(string method)
        {
            Analyze<ReplaceWithOfTypeLinqAnalyzer>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
#pragma warning disable " + CSharpDiagnosticIDs.ReplaceWithOfTypeLinqAnalyzerID + @"
		obj.Select (q => q as Test)." + method + @" (q => q != null);
	}
}");
        }

        [Theory]
        [MemberData(nameof(ReplaceableMethods))]
        public void TestJunk(string method)
        {
            Analyze<ReplaceWithOfTypeLinqAnalyzer>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.Select (x => q as Test)." + method + @" (q => q != null && true);
	}
}");
            Analyze<ReplaceWithOfTypeLinqAnalyzer>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.Select (q => q as Test)." + method + @" (q => 1 != null && true);
	}
}");

        }

    }
}

