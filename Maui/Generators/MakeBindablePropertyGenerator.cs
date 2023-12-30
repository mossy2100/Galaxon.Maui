using System.Text;
using Galaxon.Core.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Galaxon.Maui.Generators;

[Generator]
public class MakeBindablePropertyGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context) { }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        // Logic to find fields marked with [Bindable] and generate properties
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var root = syntaxTree.GetRoot();
            var fields = root.DescendantNodes().OfType<FieldDeclarationSyntax>();

            foreach (var field in fields)
            {
                var hasBindableAttribute = field.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => a.Name.ToString() == "MakeBindableProperty");

                if (hasBindableAttribute)
                {
                    var fieldName = field.Declaration.Variables.First().Identifier.ValueText;
                    var fieldType = field.Declaration.Type.ToString();
                    var propertyCode = GeneratePropertyCode(fieldName, fieldType);
                    context.AddSource($"{fieldName}_property.cs",
                        SourceText.From(propertyCode, Encoding.UTF8));
                }
            }
        }
    }

    private string PropertyNameFromFieldName(string fieldName)
    {
        if (char.IsUpper(fieldName[0]))
        {
            throw new ArgumentFormatException(nameof(fieldName),
                "Field names cannot begin with an upper-case letter.");
        }

        fieldName = fieldName.Trim('_');
        return fieldName[0].ToString().ToUpper() + fieldName[1..];
    }

    private string GeneratePropertyCode(string fieldName, string fieldType)
    {
        var propertyName = PropertyNameFromFieldName(fieldName);
        return $@"
private {fieldType} {fieldName};

public {fieldType} {propertyName}
{{
    get => {fieldName};
    set => SetProperty(ref {fieldName}, value);
}}";
    }
}
