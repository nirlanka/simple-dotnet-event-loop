using System;

class App
{
	static void Main()
	{
		var eventLoop = new EventLoop();

		Console.WriteLine("1");
		
		eventLoop.DoWithoutBlocking(
			() => 
				new CallbackArgument<int>(2),
			result =>
				Console.WriteLine(((CallbackArgument<int>) result).Value)
			);
		
		Console.WriteLine("3");

		eventLoop.StartNonBlockingWork();
	}
}

public class CallbackArgument<T>: INonBlockingTaskResult
{
	public T Value;

	public CallbackArgument(T value)
	{
		Value = value;
	}
}