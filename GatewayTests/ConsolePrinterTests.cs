using System.Globalization;
using Domain;
using Gateway.ConsolePrinter;
using Xunit;

namespace GatewayTests;

public class ConsolePrinterTests
{
    [Fact]
    public void NotifyMessage_WithString_PrintsToConsole()
    {
        var consoleOutput = new ConsoleOutput();
        var messageToSend = "TestMessage";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        consoleOutput.NotifyMessage(messageToSend);

        var output = stringWriter.ToString();
        Assert.Equal($"{messageToSend}{Environment.NewLine}", output);
    }
    
    
    [Fact]
    public void NotifyError_WithStringAndException_PrintsToConsole()
    {
        var consoleOutput = new ConsoleOutput();
        var messageToSend = "TestMessage";
        var innerExceptionMessage = "Inner message";
        var exception = new Exception(innerExceptionMessage);
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        consoleOutput.NotifyError(messageToSend, exception);

        var output = stringWriter.ToString();
        Assert.StartsWith(messageToSend, output);
        Assert.Contains(exception.ToString(), output);
        Assert.Contains(exception.GetType().FullName!, output);
    }
    
    [Fact]
    public void NotifyViolations_WithEventAndViolations_PrintsToConsole()
    {
        var consoleOutput = new ConsoleOutput();
        var violation1 = "violation1";
        var violation2 = "violation2";
        var pushEvent = new Event { EventName = "eventName" };
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        consoleOutput.NotifyEventViolations(pushEvent, new []{ violation1, violation2});

        var output = stringWriter.ToString();
        Assert.Contains(violation1, output);
        Assert.Contains(violation2, output);
        Assert.Contains(pushEvent.EventName, output);
    }
}