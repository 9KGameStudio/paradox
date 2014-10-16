﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;
using System.Linq;

using SiliconStudio.Paradox.Shaders.Parser.Ast;
using SiliconStudio.Shaders.Ast;
using SiliconStudio.Shaders.Ast.Hlsl;
using SiliconStudio.Shaders.Visitor;
using SiliconStudio.Shaders.Writer;

namespace SiliconStudio.Paradox.Shaders.Parser.Mixins
{
    public class ShaderKeyGeneratorBase : ShaderWriter
    {
        /// <summary>
        /// A flag stating if the currently visited variable is a Color.
        /// </summary>
        protected bool IsColorStatus = false;

        /// <summary>
        /// A flag stating if the currently visited variable is an array.
        /// </summary>
        protected bool IsArrayStatus = false;

        /// <summary>
        /// A flag stating if the initial value of the variable should be processed.
        /// </summary>
        protected bool ProcessInitialValueStatus = false;

        /// <summary>
        /// Runs the code generation. Results is accessible from <see cref="ShaderWriter.Text"/> property.
        /// </summary>
        public virtual bool Run()
        {
            return true;
        }

        /// <inheritdoc />
        [Visit]
        public override void Visit(Variable variable)
        {
            if (variable.Qualifiers.Contains(SiliconStudio.Shaders.Ast.Hlsl.StorageQualifier.Extern)
                || variable.Qualifiers.Contains(SiliconStudio.Shaders.Ast.Hlsl.StorageQualifier.Const)
                || variable.Qualifiers.Contains(ParadoxStorageQualifier.Stream))
                return;

            if (variable.Attributes.OfType<AttributeDeclaration>().Any(x => x.Name == "RenameLink"))
                return;

            var variableType = variable.Attributes.OfType<AttributeDeclaration>().Where(x => x.Name == "Type").Select(x => (string)x.Parameters[0].Value).FirstOrDefault();
            var variableMap = variable.Attributes.OfType<AttributeDeclaration>().Where(x => x.Name == "Map").Select(x => (string)x.Parameters[0].Value).FirstOrDefault();
            IsColorStatus = variable.Attributes.OfType<AttributeDeclaration>().Any(x => x.Name == "Color");

            ProcessInitialValueStatus = false;
            IsArrayStatus = false;

            var type = variable.Type.ResolveType();
            bool isArray = type is ArrayType;
            if (isArray)
            {
                type = ((ArrayType)type).Type.ResolveType();
            }
            if (!IsKeyType(type))
                return;

            Write("public static readonly ParameterKey<");
            if (variableType == null)
                VisitDynamic(variable.Type);
            else
                Write(variableType);
            Write("> ");
            Write(variable.Name);
            Write(" = ");
            if (variableMap == null)
            {
                Write("ParameterKeys.New<");
                if (variableType == null)
                    VisitDynamic(variable.Type);
                else
                    Write(variableType);
                Write(">(");
                if (ProcessInitialValueStatus && variable.InitialValue != null)
                {
                    var initialValueString = variable.InitialValue.ToString();

                    if (initialValueString != "null")
                    {
                        if (IsArrayStatus)
                        {
                            initialValueString = variable.Type.ToString() + initialValueString;
                        }

                        // Rename float2/3/4 to Vector2/3/4
                        if (initialValueString.StartsWith("float2")
                            || initialValueString.StartsWith("float3")
                            || initialValueString.StartsWith("float4"))
                            initialValueString = initialValueString.Replace("float", "new Vector");
                        else if (IsArrayStatus)
                        {
                            initialValueString = "new " + initialValueString;
                        }

                        if (IsColorStatus)
                        {
                            initialValueString = initialValueString.Replace("Vector3", "Color3");
                            initialValueString = initialValueString.Replace("Vector4", "Color4");
                        }
                    }
                    Write(initialValueString);
                }
                Write(")");
            }
            else
            {
                Write(variableMap);
            }
            WriteLine(";");

            IsColorStatus = false;
            IsArrayStatus = false;
            ProcessInitialValueStatus = false;
        }

        /// <summary>
        /// Check if a key should be generated.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if a key should be generated, false otherwise.</returns>
        protected virtual bool IsKeyType(TypeBase type)
        {
            return (type is ScalarType)
                   || (type is VectorType)
                   || (type is MatrixType)
                   || (type is TextureType || IsStringInList(type.Name, "Texture1D", "RWTexture1D", "Texture2D", "RWTexture2D", "Texture3D", "RWTexture3D"))
                   || (type is StateType && type.Name == "SamplerState")
                   || (type is TypeName && type.Name.Text == "ShaderMixinSource");
        }

        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="typeName">the type.</param>
        [Visit]
        public override void Visit(TypeName typeName)
        {
            var type = typeName.ResolveType();
            if (ReferenceEquals(typeName, type))
            {
                base.Visit(typeName);
                ProcessInitialValueStatus = true;
            }
            else
            {
                VisitDynamic(type);
            }
        }

        /// <inheritdoc />
        [Visit]
        public override void Visit(ScalarType scalarType)
        {
            base.Visit(scalarType);
            ProcessInitialValueStatus = true;
        }

        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="type">the type.</param>
        [Visit]
        protected virtual void Visit(VectorType type)
        {
            var finalTypeName = "Vector" + type.Dimension;
            if (IsColorStatus)
            {
                if (type.Dimension == 3)
                    finalTypeName = "Color3";
                else if (type.Dimension == 4)
                    finalTypeName = "Color4";
                else
                    throw new NotSupportedException("Color attribute is only valid for float3/float4.");
            }
            Write(finalTypeName);
            ProcessInitialValueStatus = true;
        }

        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="type">the type.</param>
        [Visit]
        protected virtual void Visit(MatrixType type)
        {
            Write("Matrix");
            ProcessInitialValueStatus = true;
        }

        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="type">the type.</param>
        [Visit]
        protected virtual void Visit(TextureType type)
        {
            Write("Texture");
        }

        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="type">the type.</param>
        [Visit]
        protected virtual void Visit(StateType type)
        {
            Write("SamplerState");
        }

        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="type">the type.</param>
        [Visit]
        public override void Visit(ArrayType type)
        {
            var dimensions = type.Dimensions;
            if (dimensions.Count != 1)
                throw new NotSupportedException();
            /*
            var expressionEvaluator = new ExpressionEvaluator();
            if (dimensions.All(x => !(x is EmptyExpression)))
            {
                var expressionResult = expressionEvaluator.Evaluate(dimensions[0]);
                if (expressionResult.HasErrors)
                    throw new InvalidOperationException();
                Write(expressionResult.Value.ToString());
            }
            */
            VisitDynamic(type.Type);
            Write("[]");
            ProcessInitialValueStatus = true;
            IsArrayStatus = true;
        }

        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="type">the type.</param>
        [Visit]
        protected virtual void Visit(TypeBase type)
        {
            Write(type.Name);
            ProcessInitialValueStatus = true;
        }

        protected static bool IsStringInList(string value, params string[] list)
        {
            return list.Any(str => string.Compare(value, str, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}