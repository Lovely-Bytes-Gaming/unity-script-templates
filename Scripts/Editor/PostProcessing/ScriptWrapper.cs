using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace ScriptTemplates.PostProcessing 
{
	internal class ScriptWrapper 
	{
        public int Index { get; private set; }
        public string Value { get; private set;}

        public ScriptWrapper(string sourceString) 
        {
            Index = 0;
            
            Value = sourceString 
                ?? throw new NullReferenceException("Tried to instantiate ScriptWrapper with NULL string!");
        }

        public void AddNameSpace(string nameSpace) 
        {
            GotoStart();
            GotoLast("using");

            if(IsOutOfBounds(Index)) 
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
            RemoveConsecutive('\n');

            while(!IsOutOfBounds(Index)) 
            {
                Insert("\t");
                GotoNextLine();
            }

            Insert("\n}");
        }

        public void ChangeNamespace(string newNamespace) 
        {
            GotoStart();
            GotoLast("using");

            if(IsOutOfBounds(Index))
                GotoStart();
            else
                GotoNextLine();

            string namespaceLine = "namespace " + newNamespace;

            while(true) 
            {
                GotoNext("namespace ");

                if(IsOutOfBounds(Index))
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

        public void GotoFirst(string substring) 
        {
            Index = IsOutOfBounds(Index) 
                ? Index
                : Value.IndexOf(substring);
        }

        public void GotoNext(string substring) 
        {
            Index = IsOutOfBounds(Index)
                ? Index
                : Value.IndexOf(substring, Index);
        }

        public void GotoLast(string substring) 
        {
            Index = IsOutOfBounds(Index)
                ? Index 
                : Value.LastIndexOf(substring);
        }

        public void GotoNextLine() 
        {
            Index = IsOutOfBounds(Index)
                ? Index
                : Value.IndexOf('\n', Index);
            
            if(!IsOutOfBounds(Index))
                Index += 1;
        }

        public void RemoveUntil(int endIndex) 
        {
            if(IsOutOfBounds(Index))
                return;

            if(endIndex < 0)
                Value = Value.Remove(Index);
            else
                Value = Value.Remove(Index, endIndex - Index);
        }

        public void Insert(string substring) 
        {
            if(IsOutOfBounds(Index)) 
            {
                Index = Value.Length;
                Value += substring;
            }
            else
                Value = Value.Insert(Index, substring);
        }

        public void InsertAndSkip(string substring) 
        {
            Insert(substring);
            Index += substring.Length;
        }

        public void RemoveConsecutive(char character) 
        {
            int endIdx = Index;

            while(!IsOutOfBounds(endIdx) && Value[endIdx] == character)
                ++endIdx;

            RemoveUntil(endIdx);
        }

        public bool IndexOutOfBounds
            => IsOutOfBounds(Index);

        public bool IsOutOfBounds(int index)
            => index < 0 || index >= Value.Length;
    }
}