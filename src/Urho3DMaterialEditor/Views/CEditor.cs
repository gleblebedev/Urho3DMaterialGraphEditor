using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Urho3DMaterialEditor.Views {
    public class MvvmTextEditor : TextEditor, INotifyPropertyChanged {
        public static DependencyProperty DocumentTextProperty =
            DependencyProperty.Register("DocumentText", typeof(string), typeof(MvvmTextEditor),
            new PropertyMetadata((obj, args) => {
                MvvmTextEditor target = (MvvmTextEditor)obj;
                target.DocumentText = (string)args.NewValue;
            })
        );
        private FoldingManager foldingManager;
        private BraceFoldingStrategy foldingStrategy;

        public string DocumentText {
            get { return base.Text; }
            set {

                if (value != null) {
                    if (value.Contains("iChannel")) value = value.Replace("iChannel0", "sDiffMap").Replace("iChannel1", "sSpecMap").Replace("iChannel2", "sNormalMap");

                    var od = base.SelectionStart;
                    base.Document.Text = value;
                    base.SelectionStart = od>600?600:od;
                }
                
    
            }
        }

     
        public override void BeginInit() {           

            base.BeginInit();

            base.Document.UndoStack.SizeLimit = 1600;

            this.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C++");
            this.ShowLineNumbers = true;
            this.FontFamily = new FontFamily("Consolas");
            this.FontSize = 13.3; // 10pt
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);

            this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(this.Options);
            foldingManager = FoldingManager.Install(this.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, this.Document);

            // var search = new SearchInputHandler(this.TextArea);
            //this.TextArea.DefaultInputHandler.NestedInputHandlers.Add(search);
            SearchPanel.Install(this.TextArea);

        }

        public void searchShow() {
           // this... = true;
        }

        protected override void OnTextChanged(EventArgs e) {
            //RaisePropertyChanged("DocumentText");
            //base.OnTextChanged(e);
            SetCurrentValue(DocumentTextProperty, base.Text);
            base.OnTextChanged(e);

            foldingStrategy?.UpdateFoldings(foldingManager, this.Document);
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string info) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        internal void ClickMenu(string header) {
            switch (header) {
                case "Undo":
                    if (CanUndo) Undo();
                    break;
                case "Redo":
                    if (CanRedo) Redo();
                    break;
                case "Cut":
                    Cut();
                    break;
                case "Copy":
                    Copy();
                    break;
                case "Paste":
                    Paste();
                    break;
                case "Delete":
                    Delete();
                    break;

                default:
                    break;
            }
        }
    }

    public class BraceFoldingStrategy {
        /// <summary>
        /// Gets/Sets the opening brace. The default value is '{'.
        /// </summary>
        public char OpeningBrace { get; set; }

        /// <summary>
        /// Gets/Sets the closing brace. The default value is '}'.
        /// </summary>
        public char ClosingBrace { get; set; }

        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public BraceFoldingStrategy() {
            this.OpeningBrace = '{';
            this.ClosingBrace = '}';
        }

        public void UpdateFoldings(FoldingManager manager, TextDocument document) {
            int firstErrorOffset;
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset) {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document) {
            List<NewFolding> newFoldings = new List<NewFolding>();

            Stack<int> startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;
            char openingBrace = this.OpeningBrace;
            char closingBrace = this.ClosingBrace;
            for (int i = 0; i < document.TextLength; i++) {
                char c = document.GetCharAt(i);
                if (c == openingBrace) {
                    startOffsets.Push(i);
                } else if (c == closingBrace && startOffsets.Count > 0) {
                    int startOffset = startOffsets.Pop();
                    // don't fold if opening and closing brace are on the same line
                    if (startOffset < lastNewLineOffset) {
                        newFoldings.Add(new NewFolding(startOffset, i + 1));
                    }
                } else if (c == '\n' || c == '\r') {
                    lastNewLineOffset = i + 1;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}