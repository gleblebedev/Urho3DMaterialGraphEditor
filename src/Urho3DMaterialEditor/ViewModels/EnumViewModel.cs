using System;
using System.Collections.Generic;
using System.Linq;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;

namespace Urho3DMaterialEditor.ViewModels
{
    public class EnumViewModel<T> : EnumViewModel
    {
        public EnumViewModel(ScriptViewModel script, ScriptNode node) : base(script, node, typeof(T))
        {

        }
    }
    public class EnumViewModel : NodeViewModel
    {
        private readonly Type _enumType;
        private string _value;
        private IList<string> _options;

        public EnumViewModel(ScriptViewModel script, ScriptNode node, Type enumType) : base(script, node)
        {
            _enumType = enumType;
            _value = node.Value;
            _options = Enum.GetNames(enumType).ToList();
            _options.RemoveAt(_options.Count-1);
        }
        public IList<string> Options
        {
            get => _options;
        }
    }
}