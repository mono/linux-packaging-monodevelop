using System.Collections.Generic;
using RefactoringEssentials.CSharp.CodeRefactorings;
using Xunit;

namespace RefactoringEssentials.Tests.CSharp.CodeRefactorings
{
    public class ReplaceAssignmentWithPostfixExpressionTests : CSharpCodeRefactoringTestBase
    {
        [Fact]
        public void TestAdd()
        {
            Test<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"
class Test
{
	void Foo (int i)
	{
		i $+= 1;
	}
}", @"
class Test
{
	void Foo (int i)
	{
        i++;
	}
}");
        }

        [Fact]
        public void TestAddWithComment()
        {
            Test<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"
class Test
{
	void Foo (int i)
	{
        // Some comment
		i $+= 1;
	}
}", @"
class Test
{
	void Foo (int i)
	{
        // Some comment
        i++;
	}
}");
        }

        [Fact]
        public void TestSub()
        {
            Test<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"
class Test
{
	void Foo (int i)
	{
		i $-= 1;
	}
}", @"
class Test
{
	void Foo (int i)
	{
        i--;
	}
}");
        }


        [Fact]
        public void TestAddCase2()
        {
            Test<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"
class Test
{
	void Foo (int i)
	{
		i $= i + 1;
	}
}", @"
class Test
{
	void Foo (int i)
	{
        i++;
	}
}");
        }

        static readonly List<object[]> integralTypes = new List<object[]>
        {
            new object[] { "sbyte" } ,
            new object[] { "byte" } ,
            new object[] { "char" } ,
            new object[] { "short" } ,
            new object[] { "ushort" } ,
            new object[] { "int" } ,
            new object[] { "uint" } ,
            new object[] { "long" } ,
            new object[] { "ulong" } ,
            new object[] { "decimal" } ,
        };

        public static IEnumerable<object[]> IntegralTypes => integralTypes;

        [Theory]
        [MemberData(nameof(IntegralTypes))]
        public void TestIncrement(string type)
        {
            string test = @"class TestClass {
    public void Increment()
    {
        " + type + @" x = 0;
        x $+= 1;
    }
}";
            string result = @"class TestClass {
    public void Increment()
    {
        " + type + @" x = 0;
        x++;
    }
}";

            Test<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(test, result);
        }

        [Theory]
        [MemberData(nameof(IntegralTypes))]
        public void TestDecrement(string type)
        {
            string test = @"class TestClass {
    public void Decrement()
    {
        " + type + @" x = 0;
        x $-= 1;
    }
}";
            string result = @"class TestClass {
    public void Decrement()
    {
        " + type + @" x = 0;
        x--;
    }
}";
            string a = "asdf";
            a += 1;
            a += "1";
            char c = '0';
            c++;

            Test<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(test, result);
        }

        [Fact]
        public void StringIsNotACandidate()
        {
            TestWrongContext<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"class TestClass {
    public void Decrement()
    {
        string x = 0;
        x $+= 1;
    }
}");
        }

        [Fact]
        public void NoExceptionWhenAddingNonIntegralType()
        {
            TestWrongContext<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"class TestClass {
    public void Decrement()
    {
        string x = 0;
        x $+= ""1"";
    }
}");
        }

        [Fact]
        public void NoRefactoringIfNoOperatorPlusPlusDefined()
        {
            TestWrongContext<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"class Operand {
    public static Operand operator +(Operand a, int b)
    {
        return new Operand();
    }
}

class TestClass {
    public void Increment()
    {
        Operand x = new Operand ();
        x $+= 1;
    }
}");
        }

        [Fact]
        public void RefactoringOfferedForOverloadedOperators()
        {
            Test<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"class Operand {
    public static Operand operator +(Operand a, int b)
    {
        return new Operand();
    }

    public static Operand operator ++(Operand a)
    {
        return new Operand();
    }
}

class TestClass {
    public void Increment()
    {
        Operand x = new Operand ();
        x $+= 1;
    }
}", @"class Operand {
    public static Operand operator +(Operand a, int b)
    {
        return new Operand();
    }

    public static Operand operator ++(Operand a)
    {
        return new Operand();
    }
}

class TestClass {
    public void Increment()
    {
        Operand x = new Operand ();
        x++;
    }
}");
        }

        [Fact]
        public void RefactoringNotOfferedForOverloadedOperatorsNotOne()
        {
            TestWrongContext<ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider>(@"class Operand {
    public static Operand operator +(Operand a, int b)
    {
        return new Operand();
    }

    public static Operand operator +(Operand a, Operand b)
    {
        return new Operand();
    }

    public static Operand operator ++(Operand a)
    {
        return new Operand();
    }
}

class TestClass {
    public void Increment()
    {
        Operand x = new Operand ();
        x $+= new Operand();
    }
}");
        }

    }
}

