using System.Collections.Generic;
using System.Linq;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class FunctionNodeFactory : AbstractNodeFactory
    {
        private readonly Pin[] _inputs;
        private readonly List<string> _inputTypes;
        private readonly Pin[] _outputs;
        private readonly List<string> _outputTypes;

        public FunctionNodeFactory(string type, string name, string[] category, Pin[] inputs,
            Pin[] outputs) : base(type, name, category)
        {
            _inputs = inputs;
            _outputs = outputs;
            _inputTypes = _inputs.Select(_ => _.Type).Distinct().ToList();
            _outputTypes = _outputs.Select(_ => _.Type).Distinct().ToList();
        }

        public override IEnumerable<string> InputTypes => _inputTypes;

        public override IEnumerable<string> OutputTypes => _outputTypes;

        public static FunctionNodeFactory Function(string type, string name, string category, Pin[] inputs,
            Pin[] outputs)
        {
            return new FunctionNodeFactory(type, name, new[] {NodeTypes.Categories.Functions, category}, inputs,
                outputs);
        }

        public static FunctionNodeFactory Function(string type, string name, string category, Pin[] inputs,
            string outputPinType)
        {
            return new FunctionNodeFactory(type, name, new[] {NodeTypes.Categories.Functions, category}, inputs,
                new[] {new Pin("", outputPinType)});
        }

        public static FunctionNodeFactory Function(string type, string name, string category, string subCategory,
            Pin[] inputs,
            Pin[] outputs)
        {
            return new FunctionNodeFactory(type, name, new[] {NodeTypes.Categories.Functions, category, subCategory},
                inputs, outputs);
        }

        public static FunctionNodeFactory Function(string type, string name, string category, string subCategory,
            Pin[] inputs,
            string outputPinType)
        {
            return new FunctionNodeFactory(type, name, new[] {NodeTypes.Categories.Functions, category, subCategory},
                inputs, new[] {new Pin("", outputPinType)});
        }


        public override ScriptNode Build()
        {
            var node = new ScriptNode();
            node.Type = Type;
            node.Name = Name;
            node.Category = NodeCategory.Function;
            foreach (var output in _outputs) node.OutputPins.Add(output.Clone());
            foreach (var input in _inputs) node.InputPins.Add(input.AsPinWithConnection());
            return node;
        }
    }
}