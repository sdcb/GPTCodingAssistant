import { HubConnection, HubConnectionBuilder, IStreamResult, IStreamSubscriber } from '@microsoft/signalr';


export class RealApi {
  connection: HubConnection;

  constructor(baseUrl: string) {
    this.connection = new HubConnectionBuilder()
      .withUrl(`/chatHub`)
      .build();
    this.connection.start();
  }

  append(sessionId: number, input: string): AsyncGenerator<string, any, unknown> {
    return streamResultToAsyncGenerator(this.connection.stream('Append', sessionId, input));
  }

  regenerateFor(sessionId: number, assistantMessageId: number): AsyncGenerator<string, any, unknown> {
    return streamResultToAsyncGenerator(this.connection.stream('RegenerateFor', sessionId, assistantMessageId));
  }

  edit(sessionId: number, userChatMessageId: number, input: string): AsyncGenerator<string, any, unknown> {``
    return streamResultToAsyncGenerator(this.connection.stream('Edit', sessionId, userChatMessageId, input));
  }
}

async function* streamResultToAsyncGenerator<T>(streamResult: IStreamResult<T>): AsyncGenerator<T> {
  const queue: T[] = [];
  let resolveNext: () => void;
  let rejectNext: (error: any) => void;
  let isDone = false;

  const subscriber: IStreamSubscriber<T> = {
    next(value: T) {
      queue.push(value);
      resolveNext();
    },
    error(err: any) {
      isDone = true;
      rejectNext(err);
    },
    complete() {
      isDone = true;
      resolveNext();
    },
  };

  streamResult.subscribe(subscriber);

  try {
    while (!isDone) {
      await new Promise<void>((resolve, reject) => {
        if (queue.length > 0) {
          resolve();
        } else {
          resolveNext = resolve;
          rejectNext = reject;
        }
      });

      if (queue.length > 0) {
        yield queue.shift()!;
      }
    }
  } finally {
    if (subscriber.closed === undefined || !subscriber.closed) {
      subscriber.complete();
    }
  }
}