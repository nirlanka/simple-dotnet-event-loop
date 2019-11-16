using System;
using System.Collections.Generic;
using System.Linq;

public class EventLoop
{
	private enum MessageType
	{
		Async = 0,
		EventHandler = 1,
	}

	private enum EventMessageType
	{
		Event = 0,
		Timeout = 1,
	}

	private class Message<T> where T:INonBlockingTaskResult
	{
		public MessageType Type;
		
		public T Result;
		public Func<T> Task;
		public Action<T> Callback;

		public string EventName;
	}

	private class EventMessage<T> where T:INonBlockingTaskResult
	{
		public EventMessageType Type;
		public string Name;
		public Func<T> GetResult;
	}

	private Queue<Message<INonBlockingTaskResult>> MessageQueue = new Queue<Message<INonBlockingTaskResult>>{};

	private List<EventMessage<INonBlockingTaskResult>> EventList = new List<EventMessage<INonBlockingTaskResult>>{};

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

	public void SetEventDone(string eventName, Func<INonBlockingTaskResult> getResultFn)
	{
		EventList.Add(new EventMessage<INonBlockingTaskResult>
		{ 
			Type = EventMessageType.Event,
			Name = eventName,
			GetResult = getResultFn,
		});
	}

	public void SetEventHandler(string eventName, Action<INonBlockingTaskResult> callback)
	{
		MessageQueue.Enqueue(new Message<INonBlockingTaskResult>
			{
				EventName = eventName,
				Callback = callback,
			});
	}

	// public void SetTimeoutTask()
	// {
	// 	EventList.Add(new EventMessage<INonBlockingTaskResult>
	// 	{
	// 		Type = EventMessageType.Timeout,
			
	// 	});
	// }

	private void HandleMessage(Message<INonBlockingTaskResult> message)
	{
		if (message.Type == MessageType.Async)
		{
			message.Callback(message.Task());
		}
		else // EventHandler
		{
			var messageIndex = EventList.FindIndex(e => e.Name == message.EventName);
			
			if (messageIndex > -1)
			{
				var eventMessage = EventList.ElementAt(messageIndex);
				message.Callback(eventMessage.GetResult());
				EventList.RemoveAt(messageIndex);
			}
			else
			{
				MessageQueue.Enqueue(message);
			}
		}
	}
}

public interface INonBlockingTaskResult
{ }