using System.Linq;
using DtoGeneratorLibrary.AvailableTypes;
using DtoGeneratorLibrary.ClassMetadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace DtoGeneratorLibrary
{
    public sealed class CsCodeGenerator
    {
        private readonly TypesTable _typesTable = new TypesTable();

        public string GetClassDeclaration(JsonClassInfo classInfo)
        {
            var classDeclaration = OpenClassDeclaration(classInfo.ClassName);

            classDeclaration = classInfo.Properties.Aggregate(classDeclaration,
                (current, property) =>
                    current.AddMembers(
                        DeclareProperty(
                            _typesTable.GetNetType(new StringDescribedType(property.Type, property.Format)),
                            property.Name)));

            return FormatNode(classDeclaration).ToString();
        }

        private ClassDeclarationSyntax OpenClassDeclaration(string className)
        {
            return SyntaxFactory.ClassDeclaration(className)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.SealedKeyword));
        }

        private PropertyDeclarationSyntax DeclareProperty(string propertyType, string propertyName)
        {
            return SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.ParseTypeName(propertyType),
                SyntaxFactory.Identifier(propertyName))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .WithAccessorList(
                    SyntaxFactory.AccessorList(
                        SyntaxFactory.List(
                            new[]
                            {
                                SyntaxFactory.AccessorDeclaration(
                                    SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(
                                        SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                SyntaxFactory.AccessorDeclaration(
                                    SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(
                                        SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                            }))
                        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken)));
        }

        private SyntaxNode FormatNode(SyntaxNode compilationUnit)
        {
            var workspace = new AdhocWorkspace();
            var options = workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.SpaceBeforeOpenSquareBracket, false);

            var formattedNode = Formatter.Format(compilationUnit, workspace, options);
            return formattedNode;
        }
    }
}