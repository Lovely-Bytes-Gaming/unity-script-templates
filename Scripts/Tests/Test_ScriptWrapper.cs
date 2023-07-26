using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ScriptTemplates.PostProcessing;
using System;

public class Test_ScriptWrapper
{
    [Test]
    public void Should_IndexIsZero_After_Construction()
    {
        var sw = new ScriptWrapper("foo");
        Assert.Zero(sw.Index);
    }

    [Test]
    public void Should_ThrowNullreferenceException_When_PassNullArgumentToConstructor() 
    {
        try 
        {
            var sw = new ScriptWrapper(null);
        }
        catch(NullReferenceException) 
        {
        }
        catch(Exception) 
        {
            Assert.Fail();
        }
    }

    [Test]
    public void Should_AddNamespaceAtBeginning_When_NoUsingDirective()
    {
        var sourceStr = "Foo";

        var expectedStr =
            "namespace Bar\n" +
            "{\n" +
            "\tFoo\n" +
            "}";

        var sw = new ScriptWrapper(sourceStr);
        sw.AddNameSpace("Bar");

        Assert.AreEqual(sw.Value, expectedStr);
    }

    [Test]
    public void Should_AddNamespaceAfterLastUsing_When_ContainsUsingDirective()
    {
        var sourceStr = 
            "using Spoon\n" +
            "using Fork\n" +
            "Foo";

        var expectedStr =
            "using Spoon\n" +
            "using Fork\n" +
            "\n" +
            "namespace Bar\n" +
            "{\n" +
            "\tFoo\n" +
            "}";

        var sw = new ScriptWrapper(sourceStr);
        sw.AddNameSpace("Bar");

        Assert.AreEqual(expectedStr, sw.Value);
    }

    [Test]
    public void Should_AllNamespaceOccurencesChanged_After_ChangeNamespace()
    {
        var sourceStr =
            "namespace Foo\n" +
            "namespace Bar";

        var expectedStr =
            "namespace Batman\n" +
            "namespace Batman";

        var sw = new ScriptWrapper(sourceStr);
        sw.ChangeNamespace("Batman");

        Assert.AreEqual(expectedStr, sw.Value);
    }

    [Test]
    public void Should_IndexIsZero_After_ResetIndex() 
    {
        var sw = new ScriptWrapper("foo");
        sw.GotoLast("o");
        sw.GotoStart();

        Assert.Zero(sw.Index);
    }

    [Test]
    public void Should_IndexAtFirstOccurence_After_GotoFirstOccurence() 
    {
        var str = "bar foo foo";
        var sw = new ScriptWrapper(str);
        sw.GotoFirst("foo");

        Assert.AreEqual(str.IndexOf("foo"), sw.Index);    
    }

    [Test]
    public void Should_IndexOutOfBounds_After_GotoFirstOccurence_When_NoOccurence() 
    {
        var str = "bar foo foo";
        var sw = new ScriptWrapper(str);
        sw.GotoFirst("car");
        Assert.IsTrue(sw.IndexOutOfBounds);
    }

    [Test]
    public void Should_IndexAtNextOccurence_After_GotoNextOccurence() 
    {
        var str = "foo bar foo";
        var sw = new ScriptWrapper(str);
        
        sw.GotoFirst("bar");
        sw.GotoNext("foo");

        Assert.AreEqual(str.LastIndexOf("foo"), sw.Index);
    }

    [Test]
    public void Should_IndexOutOfBounds_After_GotoNextOccurence_When_NoMoreOccurence() 
    {
        var str = "bar foo foo";
        var sw = new ScriptWrapper(str);
        sw.GotoFirst("foo");
        sw.GotoNext("bar");
        Assert.IsTrue(sw.IndexOutOfBounds);    
    }

    [Test]
    public void Should_IndexAtLastOccurence_After_GotoLastOccurence() 
    {
        var str = "foo bar foo";
        var sw = new ScriptWrapper(str);
        
        sw.GotoLast("foo");

        Assert.AreEqual(str.LastIndexOf("foo"), sw.Index);
    }

    [Test]
    public void Should_IndexOutOfBounds_After_GotoLastOccurence_When_NoOccurence() 
    {
        var str = "bar foo foo";
        var sw = new ScriptWrapper(str);
        sw.GotoLast("car");
        Assert.IsTrue(sw.IndexOutOfBounds);    
    }

    [Test]
    public void Should_IndexAfterNextNewline_After_GotoNextLine() 
    {
        var str = "foo\nbar";
        var sw = new ScriptWrapper(str);
        sw.GotoNextLine();
        Assert.AreEqual(str.IndexOf("bar"), sw.Index);
    }

    [Test]
    public void Should_IndexOutOfBounds_After_GotoNextLine_When_NoMoreNewlines() 
    {
        var str = "foo\nbar";
        var sw = new ScriptWrapper(str);
        sw.GotoNextLine();
        sw.GotoNextLine();
        Assert.IsTrue(sw.IndexOutOfBounds);
    }

    [Test]
    public void Should_CharsRemovedFromStartToEndIndex_After_RemoveUntil() 
    {
        var sw = new ScriptWrapper("fork");
        var referenceStr = "rk";
        
        sw.RemoveUntil(2);
        Assert.AreEqual(referenceStr, sw.Value);
    }

    [Test]
    public void Should_CharsRemovedFromIndexToEndIndex_After_RemoveUntil() 
    {
        var sw = new ScriptWrapper("fork");
        var referenceStr = "fok";
        
        sw.GotoFirst("r");
        sw.RemoveUntil(sw.Index + 1);
        Assert.AreEqual(referenceStr, sw.Value);
    }

    [Test]
    public void Should_CharsRemovedFromIndexToEnd_After_RemoveUntil_When_EndIndexOutOfBounds() 
    {
        var sw = new ScriptWrapper("fork");
        var referenceStr = "fo";
        
        sw.GotoFirst("rk");
        sw.RemoveUntil(-1);
        Assert.AreEqual(referenceStr, sw.Value);
    }

    [Test]
    public void Should_RemoveConsecutiveChars() 
    {
        var sw = new ScriptWrapper("ffffork");
        var referenceStr = "ork";

        sw.RemoveConsecutive('f');
        Assert.AreEqual(referenceStr, sw.Value);
    }

    [Test]
    public void Should_IndexOutOfBounds_When_NegativeIndex() 
    {
        var sw = new ScriptWrapper("foo");
        Assert.IsTrue(sw.IsOutOfBounds(-1));
    }

    [Test]
    public void Should_IndexOutOfBounds_When_IndexGreaterEqualsValueLength() 
    {
        var str = "foo";
        var sw = new ScriptWrapper(str);
        Assert.IsTrue(sw.IsOutOfBounds(str.Length));
    }
}
