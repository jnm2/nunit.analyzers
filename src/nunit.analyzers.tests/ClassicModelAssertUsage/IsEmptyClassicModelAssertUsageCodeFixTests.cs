using System.Collections.Immutable;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Analyzers.ClassicModelAssertUsage;
using NUnit.Analyzers.Constants;
using NUnit.Framework;

namespace NUnit.Analyzers.Tests.ClassicModelAssertUsage
{
    [TestFixture]
    public sealed class IsEmptyClassicModelAssertUsageCodeFixTests
    {
        private static readonly DiagnosticAnalyzer analyzer = new ClassicModelAssertUsageAnalyzer();
        private static readonly CodeFixProvider fix = new IsEmptyClassicModelAssertUsageCodeFix();
        private static readonly ExpectedDiagnostic expectedDiagnostic =
            ExpectedDiagnostic.Create(AnalyzerIdentifiers.IsEmptyUsage);

        [Test]
        public void VerifyGetFixableDiagnosticIds()
        {
            var fix = new IsEmptyClassicModelAssertUsageCodeFix();
            var ids = fix.FixableDiagnosticIds.ToImmutableArray();

            Assert.That(ids, Is.EquivalentTo(new[] { AnalyzerIdentifiers.IsEmptyUsage }));
        }

        [Test]
        public void VerifyIsEmptyFix()
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var collection = Array.Empty<object>();

            ↓Assert.IsEmpty(collection);
        }}");
            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void TestMethod()
        {
            var collection = Array.Empty<object>();

            Assert.That(collection, Is.Empty);
        }");
            AnalyzerAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, fixTitle: CodeFixConstants.TransformToConstraintModelDescription);
        }

        [Test]
        public void VerifyIsEmptyFixWithMessage()
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var collection = Array.Empty<object>();

            ↓Assert.IsEmpty(collection, ""message"");
        }}");
            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void TestMethod()
        {
            var collection = Array.Empty<object>();

            Assert.That(collection, Is.Empty, ""message"");
        }");
            AnalyzerAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, fixTitle: CodeFixConstants.TransformToConstraintModelDescription);
        }

        [Test]
        public void VerifyIsEmptyFixWithMessageAndParams()
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var collection = Array.Empty<object>();

            ↓Assert.IsEmpty(collection, ""message"", Guid.NewGuid());
        }}");
            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void TestMethod()
        {
            var collection = Array.Empty<object>();

            Assert.That(collection, Is.Empty, ""message"", Guid.NewGuid());
        }");
            AnalyzerAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, fixTitle: CodeFixConstants.TransformToConstraintModelDescription);
        }
    }
}
