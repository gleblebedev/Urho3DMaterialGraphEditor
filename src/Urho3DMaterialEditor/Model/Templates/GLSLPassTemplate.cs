﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Urho3DMaterialEditor.Model.Templates
{
    /// <summary>
    ///     Class to produce the template output
    /// </summary>
#line 1 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"
    [GeneratedCode("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class GLSLPassTemplate : GLSLPassTemplateBase
    {
        /// <summary>
        ///     Create the template output
        /// </summary>
        public virtual string TransformText()
        {
#line 6 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"


            foreach (var define in Graph.Defines)
            {
                WriteLine("#if !defined(" + define + ")");
                WriteLine("#define " + define);
                WriteLine("#endif");
            }

            foreach (var undefine in Graph.Undefines)
            {
                WriteLine("#if defined(" + undefine + ")");
                WriteLine("#undef " + undefine);
                WriteLine("#endif");
            }


#line default
#line hidden
            Write(@"
// ------------------- Vertex Shader ---------------

#if defined(DIRLIGHT) && (!defined(GL_ES) || defined(WEBGL))
    #define NUMCASCADES 4
#else
    #define NUMCASCADES 1
#endif

#ifdef COMPILEVS

// Silence GLSL 150 deprecation warnings
#ifdef GL3
#define attribute in
#define varying out
#endif

");

#line 40 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"


            foreach (var uniform in Graph.VertexShaderAttributes)
            {
                if (uniform.Type == NodeTypes.VertexData)
                    throw new InvalidOperationException();
                var type = GLSLCodeGen.GetType(uniform.OutputPins[0].Type);
                if (uniform.Name == "BlendIndices")
                    type = "vec4";
                uniform.Extra.Define.WriteLineIfDef(this,
                    "attribute " + type + " i" + uniform.Name + GLSLCodeGen.GetArraySize(uniform.OutputPins[0].Type) +
                    ";");
            }

            foreach (var uniform in VertexShaderUniformsAndFunctions.Uniforms)
                uniform.Extra.Define.WriteLineIfDef(this,
                    "uniform " + GLSLCodeGen.GetType(uniform.Type) + " " + uniform.Name +
                    GLSLCodeGen.GetArraySize(uniform.Type) + ";");
            foreach (var uniform in Graph.VertexShaderVaryings)
                uniform.Extra.Define.WriteLineIfDef(this,
                    "varying " + GLSLCodeGen.GetType(GLSLCodeGen.GetVaryingType(uniform.InputPins[0].Type)) + " v" +
                    uniform.Value + GLSLCodeGen.GetArraySize(uniform.InputPins[0].Type) + ";");


#line default
#line hidden
            Write("\r\nattribute float iObjectIndex;\r\n\r\n\r\n");

#line 64 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"

            foreach (var function in VertexShaderUniformsAndFunctions.Functions)
            {
                WriteLine("// --- " + function + " ---");
                WriteLine(VertexShaderGenerator.GetFunction(function));
            }

            WriteLine(null, "");


#line default
#line hidden
            Write("\r\nvoid VS()\r\n{\r\n");

#line 75 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"

            foreach (var varying in Graph.VertexShaderVaryings)
                VertexShaderGenerator.GenerateCode(varying);
            var ret = VertexShaderGenerator.GenerateCode(Graph.OutputPosition);
            WriteLine("vec4 ret =  " + ret + ";");
            WriteLine(null, "");


#line default
#line hidden
            Write(@"
    // While getting the clip coordinate, also automatically set gl_ClipVertex for user clip planes
    #if !defined(GL_ES) && !defined(GL3)
       gl_ClipVertex = ret;
    #elif defined(GL3)
       gl_ClipDistance[0] = dot(cClipPlane, ret);
    #endif
    gl_Position = ret;
}

#else
// ------------------- Pixel Shader ---------------

");

#line 95 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"

            foreach (var uniform in PixelShaderUniformsAndFunctions.Uniforms)
//	WriteIfDef(uniform);
                if (uniform.Type == PinTypes.LightMatrices)
                    uniform.Extra.Define.WriteLineIfDef(this, @"
#if !defined(GL_ES) || defined(WEBGL)
    uniform mat4 cLightMatrices[4];
#else
    uniform highp mat4 cLightMatrices[2];
#endif
");
                else
                    uniform.Extra.Define.WriteLineIfDef(this,
                        "uniform " + GLSLCodeGen.GetType(uniform.Type) + " " + uniform.Name + ";");
//	WriteEndIf(uniform);
            foreach (var uniform in Graph.Samplers)
                if (uniform.Name == SamplerNodeFactory.ShadowMap)
                    uniform.Extra.Define.WriteLineIfDef(this, @"
#ifndef GL_ES
    #ifdef VSM_SHADOW
        uniform sampler2D sShadowMap;
    #else
        uniform sampler2DShadow sShadowMap;
    #endif
#else
    uniform highp sampler2D sShadowMap;
#endif
	");
                else
                    uniform.Extra.Define.WriteLineIfDef(this,
                        "uniform " + GLSLCodeGen.GetType(uniform.OutputPins[0]) + " " +
                        GLSLCodeGen.GetSamplerName(uniform) + ";");
            foreach (var uniform in Graph.PixelShaderVaryings)
                uniform.Extra.Define.WriteLineIfDef(this,
                    "varying " + GLSLCodeGen.GetType(GLSLCodeGen.GetVaryingType(uniform.OutputPins[0].Type)) + " v" +
                    uniform.Value + GLSLCodeGen.GetArraySize(uniform.OutputPins[0].Type) + ";");


#line default
#line hidden
            Write("\r\n");

#line 142 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"

            foreach (var function in PixelShaderUniformsAndFunctions.Functions)
            {
                WriteLine("// --- " + function + " ---");
                WriteLine(PixelShaderGenerator.GetFunction(function));
            }

            WriteLine(null, "");


#line default
#line hidden
            Write("\r\nvoid PS()\r\n{\r\n");

#line 153 "E:\MyWork\Toe.Scripting\src\Urho3DMaterialEditor\Model\Templates\GLSLPassTemplate.tt"

            foreach (var discard in Graph.Discards)
                PixelShaderGenerator.GenerateCode(discard);
            foreach (var rt in Graph.RenderTargets)
                PixelShaderGenerator.GenerateCode(rt);
            WriteLine(null, "");


#line default
#line hidden
            Write("}\r\n#endif");
            return GenerationEnvironment.ToString();
        }
    }

#line default
#line hidden

    #region Base class

    /// <summary>
    ///     Base class for this transformation
    /// </summary>
    [GeneratedCode("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class GLSLPassTemplateBase
    {
        #region Fields

        private StringBuilder generationEnvironmentField;
        private CompilerErrorCollection errorsField;
        private List<int> indentLengthsField;
        private bool endsWithNewline;

        #endregion

        #region Properties

        /// <summary>
        ///     The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected StringBuilder GenerationEnvironment
        {
            get
            {
                if (generationEnvironmentField == null) generationEnvironmentField = new StringBuilder();
                return generationEnvironmentField;
            }
            set => generationEnvironmentField = value;
        }

        /// <summary>
        ///     The error collection for the generation process
        /// </summary>
        public CompilerErrorCollection Errors
        {
            get
            {
                if (errorsField == null) errorsField = new CompilerErrorCollection();
                return errorsField;
            }
        }

        /// <summary>
        ///     A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private List<int> indentLengths
        {
            get
            {
                if (indentLengthsField == null) indentLengthsField = new List<int>();
                return indentLengthsField;
            }
        }

        /// <summary>
        ///     Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent { get; private set; } = "";

        /// <summary>
        ///     Current transformation session
        /// </summary>
        public virtual IDictionary<string, object> Session { get; set; }

        #endregion

        #region Transform-time helpers

        /// <summary>
        ///     Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend)) return;
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (GenerationEnvironment.Length == 0
                || endsWithNewline)
            {
                GenerationEnvironment.Append(CurrentIndent);
                endsWithNewline = false;
            }

            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(Environment.NewLine, StringComparison.CurrentCulture)) endsWithNewline = true;
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if (CurrentIndent.Length == 0)
            {
                GenerationEnvironment.Append(textToAppend);
                return;
            }

            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(Environment.NewLine, Environment.NewLine + CurrentIndent);
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (endsWithNewline)
                GenerationEnvironment.Append(textToAppend, 0, textToAppend.Length - CurrentIndent.Length);
            else
                GenerationEnvironment.Append(textToAppend);
        }

        /// <summary>
        ///     Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            Write(textToAppend);
            GenerationEnvironment.AppendLine();
            endsWithNewline = true;
        }

        /// <summary>
        ///     Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            Write(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Raise an error
        /// </summary>
        public void Error(string message)
        {
            var error = new CompilerError();
            error.ErrorText = message;
            Errors.Add(error);
        }

        /// <summary>
        ///     Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            var error = new CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            Errors.Add(error);
        }

        /// <summary>
        ///     Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if (indent == null) throw new ArgumentNullException("indent");
            CurrentIndent = CurrentIndent + indent;
            indentLengths.Add(indent.Length);
        }

        /// <summary>
        ///     Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            var returnValue = "";
            if (indentLengths.Count > 0)
            {
                var indentLength = indentLengths[indentLengths.Count - 1];
                indentLengths.RemoveAt(indentLengths.Count - 1);
                if (indentLength > 0)
                {
                    returnValue = CurrentIndent.Substring(CurrentIndent.Length - indentLength);
                    CurrentIndent = CurrentIndent.Remove(CurrentIndent.Length - indentLength);
                }
            }

            return returnValue;
        }

        /// <summary>
        ///     Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            indentLengths.Clear();
            CurrentIndent = "";
        }

        #endregion

        #region ToString Helpers

        /// <summary>
        ///     Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private IFormatProvider formatProviderField = CultureInfo.InvariantCulture;

            /// <summary>
            ///     Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public IFormatProvider FormatProvider
            {
                get => formatProviderField;
                set
                {
                    if (value != null) formatProviderField = value;
                }
            }

            /// <summary>
            ///     This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if (objectToConvert == null) throw new ArgumentNullException("objectToConvert");
                var t = objectToConvert.GetType();
                var method = t.GetMethod("ToString", new[]
                {
                    typeof(IFormatProvider)
                });
                if (method == null)
                    return objectToConvert.ToString();
                return (string) method.Invoke(objectToConvert, new object[]
                {
                    formatProviderField
                });
            }
        }

        /// <summary>
        ///     Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper { get; } = new ToStringInstanceHelper();

        #endregion
    }

    #endregion
}