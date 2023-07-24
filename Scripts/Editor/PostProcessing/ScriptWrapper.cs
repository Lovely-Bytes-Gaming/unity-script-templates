namespace ScriptTemplates.PostProcessing 
{
	internal struct ScriptWrapper 
	{
        private int index;
        public string Value { get;  private set;}

        public ScriptWrapper(string sourceString) 
        {
            index = 0;
            Value = sourceString;
        }

        public void AddNameSpace(string nameSpace) 
        {
            GotoStart();
            GotoLast("using");
            GotoNextLine();
            
            InsertAndSkip("\n");
            InsertAndSkip("\nnamespace " + nameSpace + "\n");
            InsertAndSkip("{\n");

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
            PlaceIndexAfterLastOccurence("using");

            string namespaceLine = "namespace " + newNamespace;

            while(true) 
            {
                GotoNext("namespace ");

                if(IndexAtEOF)
                    break;

                RemoveUntil(Value.IndexOf('\n', index));
                Insert(namespaceLine);
                GotoNextLine();
            }
        } 

        public void GotoStart() 
        {
            index = 0;
        }

        public void PlaceIndexAfterLastOccurence(string substring) 
        {
            index = Value.LastIndexOf(substring);

            if(IndexAtEOF)
                GotoStart();

            else
                GotoNextLine();
        }

        public void GotoFirst(string substring) 
        {
            GotoStart();
            index = Value.IndexOf(substring, index);
        }

        public void GotoNext(string substring, bool skip = false) 
        {
            index = Value.IndexOf(substring, index);
        }

        public void GotoLast(string substring, bool skip = false) 
        {
            index = Value.LastIndexOf(substring, index);
        }

        public void GotoNextLine() 
        {
            index = Value.IndexOf('\n', index) + 1;
        }

        public void RemoveUntil(int endIndex) 
        {
            if(endIndex < 0)
                Value = Value.Remove(index);

            Value = Value.Remove(index, endIndex - index);
        }

        public void Insert(string substring) 
        {
            if(!IndexAtEOF)
                Value = Value.Insert(index, substring);
            else
                Value += substring;
        }

        public void InsertAndSkip(string substring) 
        {
            Insert(substring);
            index += substring.Length;
        }


        private bool IndexAtEOF
            => index < 0 || index >= Value.Length;
    }
}