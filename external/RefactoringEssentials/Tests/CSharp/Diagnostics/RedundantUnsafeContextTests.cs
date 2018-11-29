using RefactoringEssentials.CSharp.Diagnostics;
using Xunit;

namespace RefactoringEssentials.Tests.CSharp.Diagnostics
{
    public class RedundantUnsafeContextTests : CSharpDiagnosticTestBase
    {
        [Fact]
        public void TestUnsafeClass()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"$unsafe$ class Foo
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine(""Hello World!"");
    }
}
", @"class Foo
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine(""Hello World!"");
    }
}
");
        }

        [Fact]
        public void TestUnsafeStatement()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
class Foo
{
    public static void Main(string[] args)
    {
        $unsafe$ {
            System.Console.WriteLine(""Hello World1!"");
            System.Console.WriteLine(""Hello World2!"");
        }
    }
}
", @"
class Foo
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine(""Hello World1!"");
        System.Console.WriteLine(""Hello World2!"");
    }
}
");
        }

        [Fact]
        public void TestNestedUnsafeStatement()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"$unsafe$ class Program
{
    static void Main(string str)
    {
        unsafe
        {
            fixed (char* charPtr = &str)
            {
                *charPtr = 'A';
            }
        }
    }
}
", @"class Program
{
    static void Main(string str)
    {
        unsafe
        {
            fixed (char* charPtr = &str)
            {
                *charPtr = 'A';
            }
        }
    }
}
");
        }

        [Fact]
        public void TestNestedUnsafeStatements()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"class Program
{
    static void Main(string str)
    {
        $unsafe$
        {
            unsafe
            {
                fixed (char* charPtr = &str)
                {
                    *charPtr = 'A';
                }
            }
        }
    }
}
", @"class Program
{
    static void Main(string str)
    {
        unsafe
        {
            fixed (char* charPtr = &str)
            {
                *charPtr = 'A';
            }
        }
    }
}
");
        }

        [Fact]
        public void TestValidFixedPointer()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
unsafe struct Foo {
    public fixed char fixedBuffer[128];
}
");
        }


        [Fact]
        public void TestDisable()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
#pragma warning disable " + CSharpDiagnosticIDs.RedundantUnsafeContextAnalyzerID + @"
unsafe class Foo
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine(""Hello World!"");
    }
}
");
        }

        [Fact]
        public void TestSizeOf()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
public static class TestClass
{
    struct TestStruct {
    }
    public static void Main(String[] args)
    {
        unsafe {
            int a = sizeof(TestStruct);
        }
    }
}");
        }

        [Fact]
        public void TestFixed()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
class Foo
{
    unsafe struct TestStruct
    {
        public fixed byte TestVar[32];
    }
}
");
        }

        [Fact]
        public void TestUnsafeProperty()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
class Foo
{
    class TestStruct
    {
        unsafe byte* UnsafePointer
        {
            get {
                return null;
            }
        }

        public IntPtr SafeData {
            get {
                unsafe {
                    return new IntPtr(UnsafePointer);
                }
            }
        }
    }
}
");
        }

        [Fact]
        public void TestField()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"$unsafe$ class Foo
{
    int field;
}
", @"class Foo
{
    int field;
}
");
        }

        [Fact]
        public void TestPtrField()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"unsafe class Foo
{
    int* field;
}
");
        }

        [Fact]
        public void TestMemberFieldPositive()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"class Foo
{
    $unsafe$ int field;
}
",@"class Foo
{
int field;
}
");
        }

        [Fact]
        public void TestMemberFieldNegative()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
class Foo
{
    unsafe int *field;
}
");
        }

        [Fact]
        public void TestUnsafeDelegatePositive()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"$unsafe$ class Foo
{
    delegate int del();
}
", @"class Foo
{
    delegate int del();
}
");
        }

        [Fact]
        public void TestUnsafeDelegateNegative()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"unsafe class Foo
{
    delegate int* del();
}
");
        }

        [Fact]
        public void TestUnsafeMethod()
        {
            Analyze<RedundantUnsafeContextAnalyzer>(@"
class Foo
{
    public static $unsafe$ void Main(string[] args)
    {
        System.Console.WriteLine(""Hello World1!"");
        System.Console.WriteLine(""Hello World2!"");
    }
}
", @"
class Foo
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine(""Hello World1!"");
        System.Console.WriteLine(""Hello World2!"");
    }
}
");
        }
    }
}