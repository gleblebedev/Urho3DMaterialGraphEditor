using System;
using System.Collections.Generic;
using Toe.Scripting.Helpers;
using Urho3DMaterialEditor.Model.Templates;
using static Urho3DMaterialEditor.Model.TranslatedMaterialGraph;

namespace Urho3DMaterialEditor.Model
{
    public class HLSLCodeGen : ICodeGen
    {
        private readonly IT4Writer _writer;
        public Dictionary<NodeHelper<NodeInfo>, string> _variables;

        public HLSLCodeGen(IT4Writer writer)
        {
            _writer = writer;
            _variables = new Dictionary<NodeHelper<NodeInfo>, string>();
        }

        public IEnumerable<RequiredFunction> GetRequiredFunctions(RequiredFunction requiredFunction)
        {
            yield break;
        }

        public IEnumerable<RequiredFunction> GetRequiredFunctions(NodeHelper<NodeInfo> node)
        {
            yield break;
        }

        public IEnumerable<NodeHelper<NodeInfo>> GetRequiredUniforms(RequiredFunction requiredFunction)
        {
            yield break;
        }

        public IEnumerable<NodeHelper<NodeInfo>> GetRequiredUniforms(NodeHelper<NodeInfo> node)
        {
            yield break;
        }

        public string GetFunction(RequiredFunction function)
        {
            switch (function)
            {
                default:
                    throw new NotImplementedException();
            }
        }
    }
}