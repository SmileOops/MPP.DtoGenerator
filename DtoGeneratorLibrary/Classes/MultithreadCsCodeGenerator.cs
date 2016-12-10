using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DtoGeneratorLibrary.AvailableTypes;
using DtoGeneratorLibrary.ClassMetadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace DtoGeneratorLibrary
{
    public sealed class MultithreadCsCodeGenerator
    {
        private readonly string _classesNamespace;
        private readonly Queue<JsonClassInfo> _classesQueue;
        private readonly int _maxTasksNumber;
        private readonly object _syncObject;
        private readonly TypesTable _typesTable;
        private readonly List<WriteableClass> _writeableClasses;
        private int _activeTasksNumber;

        public MultithreadCsCodeGenerator(string classesNamespace, int maxTasksNumber)
        {
            _activeTasksNumber = 0;
            _maxTasksNumber = maxTasksNumber;
            _classesNamespace = classesNamespace;
            _typesTable = new TypesTable();
            _writeableClasses = new List<WriteableClass>();
            _classesQueue = new Queue<JsonClassInfo>();
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
                        PutClassStringInList(jsonClass, classesNamespace);
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
                    if (_classesQueue.Count != 0)
                    {
                        var dequeuedClass = _classesQueue.Dequeue();

                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            PutClassStringInList(dequeuedClass, _classesNamespace);
                            EndCallback(countdownEvent);
                        });
                    }
                }
                countdownEvent.Signal();
            }
        }

        public List<WriteableClass> GetWriteableClasses(JsonClassesInfo classesInfo, string classesNamespace)
        {
            using (var countdownEvent = new CountdownEvent(classesInfo.ClassesInfo.Count))
            {
                foreach (var classInfo in classesInfo.ClassesInfo)
                {
                    EnqueueClassGenerating(classInfo, classesNamespace, countdownEvent);
                }

                countdownEvent.Wait();
            }
            return _writeableClasses;
        }

        private void PutClassStringInList(JsonClassInfo classInfo, string classesNamespace)
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