using System;
using System.Collections.Generic;

public class EventLoop
{
	private enum MessageType
	{
		Async = 0,
		EventHandler = 1,
	}

	private class Message<T> where T:INonBlockingTaskResult
	{
		public MessageType Type;
		public T Result;
		public Func<T> Task;
		public Action<T> Callback;
	}

	private Queue<Message<INonBlockingTaskResult>> MessageQueue = new Queue<Message<INonBlockingTaskResult>>{};

	public void DoWithoutBlocking(Func<INonBlockingTaskResult> task, Action<INonBlockingTaskResult> callback)
	{
		var message = new Message<INonBlockingTaskResult>
		{
			Type = MessageType.Async,
			Result = null,
			Task = task,
			Callback = callback,
		};

		MessageQueue.Enqueue(message);
	}

	public void StartNonBlockingWork()
	{
		while (MessageQueue.Count > 0)
		{
			HandleMessage(MessageQueue.Dequeue());
		}
	}

	private void HandleMessage(Message<INonBlockingTaskResult> message)
	{
		if (message.Type == MessageType.Async)
		{
			message.Callback(message.Task());
		}
		else
		{
			// TODO: Handle event hanlders
		}
	}
}

public interface INonBlockingTaskResult
{ }