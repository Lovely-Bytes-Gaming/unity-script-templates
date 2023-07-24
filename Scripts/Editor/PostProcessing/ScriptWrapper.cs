namespace ScriptTemplates.PostProcessing 
{
	internal struct ScriptWrapper 
	{
        public int Index { get; private set; }
        public string Value { get;  private set;}

        public ScriptWrapper(string sourceString) 
        {
            Index = 0;
            Value = sourceString;
        }

        public void AddNameSpace(string nameSpace) 
        {
            GotoStart();
            GotoLast("using");

            if(IndexAtEOF) 
            {
                // no using directives, so we can put the namespace in the first line
                GotoStart();
            }
            else 
            {
                // place the namespace after the last using directive
                GotoNextLine();
                InsertAndSkip("\n");
            }
            
            InsertAndSkip("namespace " + nameSpace + "\n");
            InsertAndSkip("{\n");
            ConsumeAll('\n');

            while(!IndexAtEOF) 
            {
                Insert("\t");
                GotoNextLine();
            }

            Insert("\n}");
        }

        public void ChangeNamespace(string newNamespace) 
        {
            GotoStart();
            PlaceIndexAfterLast("using");

            string namespaceLine = "namespace " + newNamespace;

            while(true) 
            {
                GotoNext("namespace ");

                if(IndexAtEOF)
                    break;

                RemoveUntil(Value.IndexOf('\n', Index));
                Insert(namespaceLine);
                GotoNextLine();
            }
        } 

        public void GotoStart() 
        {
            Index = 0;
        }

        public void PlaceIndexAfterLast(string substring) 
        {
            Index = Value.LastIndexOf(substring);

            if(IndexAtEOF)
                GotoStart();

            else
                GotoNextLine();
        }

        public void GotoFirst(string substring) 
        {
            Index = Value.IndexOf(substring);
        }

        public void GotoNext(string substring) 
        {
            Index = Value.IndexOf(substring, Index);
        }

        public void GotoLast(string substring) 
        {
            Index = Value.LastIndexOf(substring);
        }

        public void GotoNextLine() 
        {
            Index = Value.IndexOf('\n', Index);
            
            if(!IndexAtEOF)
                Index += 1;
        }

        public void RemoveUntil(int endIndex) 
        {
            if(endIndex < 0)
                Value = Value.Remove(Index);

            Value = Value.Remove(Index, endIndex - Index);
        }

        public void Insert(string substring) 
        {
            if(!IndexAtEOF)
                Value = Value.Insert(Index, substring);
            else
                Value += substring;
        }

        public void InsertAndSkip(string substring) 
        {
            Insert(substring);
            Index += substring.Length;
        }

        public void ConsumeAll(char character) 
        {
            int endIdx = Index;

            while(!IsEOF(endIdx) && Value[endIdx] == character)
                ++endIdx;

            RemoveUntil(endIdx);
        }


        private bool IndexAtEOF
            => IsEOF(Index);

        private bool IsEOF(int index)
            => index < 0 || index >= Value.Length;
    }
}