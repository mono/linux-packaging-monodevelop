//
// FormatStringHelper.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials
{
	static class FormatStringHelper
	{
		static readonly string[] parameterNames = { "format", "frmt", "fmt" };

		public static bool TryGetFormattingParameters(
			IInvocationOperation invocationExpression,
			out IArgumentOperation formatArgument, out IList<IArgumentOperation> arguments,
			Func<IParameterSymbol, IOperation, bool> argumentFilter,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			if (invocationExpression == null)
				throw new ArgumentNullException(nameof(invocationExpression));

			if (argumentFilter == null)
				argumentFilter = (p, e) => true;

			formatArgument = null;
			arguments = new List<IArgumentOperation>();
			var method = invocationExpression.TargetMethod;
			if (method == null || method.Kind != SymbolKind.Method)
				return false;

			// Search for method of type: void Name(string format, params object[] args);
			var methods = method.ContainingType.GetMembers(method.Name).OfType<IMethodSymbol>();
			if (!methods.Any(m => m.Parameters.Length == 2 &&
							 m.Parameters[0].Type.SpecialType == SpecialType.System_String && parameterNames.Contains(m.Parameters[0].Name) &&
							 m.Parameters[1].IsParams))
				return false;

			// TODO: Handle argument -> parameter mapping.
			//var argumentToParameterMap = invocationResolveResult.GetArgumentToParameterMap();
			//var resolvedParameters = invocationResolveResult.Member.Parameters;
			int i = 0;
			foreach (var argument in invocationExpression.Arguments)
			{
				var parameterIndex = i++; //argumentToParameterMap[i];
				if (parameterIndex < 0 || parameterIndex >= method.Parameters.Length)
				{
					// No valid mapping for this argument, skip it
					continue;
				}
				var parameter = argument.Parameter;
				if (parameterIndex == 0 && parameter.Type.SpecialType == SpecialType.System_String && parameterNames.Contains(parameter.Name))
				{
					formatArgument = argument;
				}
				else if (formatArgument != null && parameter.IsParams /*&& !invocationResolveResult.IsExpandedForm*/)
				{
					if (argument.Value is IArrayCreationOperation ace)
					{
						foreach (var element in ace.Initializer?.ElementValues)
						{
							if (argumentFilter(parameter, element))
								arguments.Add(argument);
						}
					}
					else
						return false;
				}
				else if (formatArgument != null && argumentFilter(parameter, argument.Value))
				{
					arguments.Add(argument);
				}
			}
			return formatArgument != null;
		}
	}
}