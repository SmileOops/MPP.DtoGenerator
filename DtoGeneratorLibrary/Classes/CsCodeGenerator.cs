using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DtoGeneratorLibrary.AvailableTypes;
using DtoGeneratorLibrary.Classes.ClassMetadata;
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
        private readonly int _maxTasksNumber;

        private readonly TypesTable _typesTable = new TypesTable();
        private int _activeTasksNumber;
        private readonly string _classesNamespace;
        private readonly ConcurrentQueue<JsonClassInfo> _classesQueue;
        private readonly object _syncObject;
        private readonly List<WriteableClass> _writeableClasses;

        public CsCodeGenerator(string classesNamespace, int maxTasksNumber)
        {
            _maxTasksNumber = maxTasksNumber;
            _classesNamespace = classesNamespace;
            _writeableClasses = new List<WriteableClass>();
            _activeTasksNumber = 0;
            _classesQueue = new ConcurrentQueue<JsonClassInfo>();
            _syncObject = new object();
        }

        private bool IsFilled => _maxTasksNumber == _activeTasksNumber;

        private void EnqueueClassGenerating(JsonClassInfo jsonClass, string classesNamespace,
            CountdownEvent countdownEvent)
        {
            lock (_syncObject)
            {
                if (!IsFilled)
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        GetClassString(jsonClass, classesNamespace);
                        EndCallback(countdownEvent);
                    });
                    _activeTasksNumber++;
                }
                else
                {
                    _classesQueue.Enqueue(jsonClass);
                }
            }
        }

        private void EndCallback(CountdownEvent countdownEvent)
        {
            lock (_syncObject)
            {
                _activeTasksNumber--;
                if (!IsFilled)
                {
                    JsonClassInfo dequeuedClass;
                    if (_classesQueue.TryDequeue(out dequeuedClass))
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            GetClassString(dequeuedClass, _classesNamespace);
                            EndCallback(countdownEvent);
                        });
                    }
                }
                countdownEvent.Signal();
            }
        }

        public List<WriteableClass> GetClassStrings(JsonClassesInfo classesInfo, string classesNamespace)
        {
            //return classesInfo.ClassesInfo.Select(classInfo => new WriteableClass(classInfo.ClassName, GetClassString(classInfo, classesNamespace))).ToList();
            using (var countdownEvent = new CountdownEvent(classesInfo.ClassesInfo.Length))
            {
                foreach (var classInfo in classesInfo.ClassesInfo)
                {
                    EnqueueClassGenerating(classInfo, classesNamespace, countdownEvent);
                }

                countdownEvent.Wait();
            }
            return _writeableClasses;
        }

        private void GetClassString(JsonClassInfo classInfo, string classesNamespace)
        {
            var namespaceDeclaration = GetNameSpaceDeclaration(classesNamespace);
            var classDeclaration = GetClassDeclaration(classInfo.ClassName);

            classDeclaration = classInfo.Properties.Aggregate(classDeclaration,
                (current, property) =>
                    current.AddMembers(
                        GetPropertyDeclaration(
                            _typesTable.GetNetType(new StringDescribedType(property.Type, property.Format)),
                            property.Name)));

            namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);

            _writeableClasses.Add(new WriteableClass(classInfo.ClassName, FormatNode(namespaceDeclaration).ToString()));
        }

        private NamespaceDeclarationSyntax GetNameSpaceDeclaration(string classNamespace)
        {
            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(classNamespace));
        }

        private ClassDeclarationSyntax GetClassDeclaration(string className)
        {
            return SyntaxFactory.ClassDeclaration(className)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.SealedKeyword));
        }

        private PropertyDeclarationSyntax GetPropertyDeclaration(string propertyType, string propertyName)
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