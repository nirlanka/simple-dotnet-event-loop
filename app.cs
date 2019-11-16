using System;

class App
{
	static void Main()
	{
		var eventLoop = new EventLoop();

		Console.WriteLine("1");

		eventLoop.SetTimeoutTask(
			() => 
				{ Console.WriteLine(4); }, 
			1000
		);
		
		eventLoop.DoWithoutBlocking(
			() => 
				new CallbackArgument<int>(3),
			result =>
				Console.WriteLine(((CallbackArgument<int>) result).Value)
			);
		
		Console.WriteLine("4");

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