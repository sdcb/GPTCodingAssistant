import { HubConnection, HubConnectionBuilder, IStreamSubscriber } from '@microsoft/signalr';


export class RealApi {
  connection: HubConnection;

  constructor(baseUrl: string) {
    this.connection = new HubConnectionBuilder()
      .withUrl(`/chatHub`)
      .build();
    this.connection.start();
  }

  async getList() {
    const resp = await fetch('/chat');
    return <ChatMessage[]>(await resp.json());
  }

  async *chat(input: string): AsyncGenerator<string, any, unknown> {


    let pushValue: (value: string) => void;
    let pushError: (error: any) => void;
    let pushEnd: () => void;

    let streamPromise = new Promise<string>((resolve, reject) => {
      pushValue = resolve;
      pushError = reject;
    });
    const endPromise = new Promise<void>((resolve) => {
      pushEnd = resolve;
    });

    const observer: IStreamSubscriber<string> = {
      next(value: string) {
        pushValue(value);
      },
      error(err: any) {
        pushError(err);
      },
      complete() {
        pushEnd();
      },
    };

    const subscription = this.connection.stream('Chat', input).subscribe(observer);

    try {
      while (true) {
        const nextPromise = streamPromise.then(
          (value) => ({ value, done: false }),
          (err) => { throw err; }
        );
        const { value, done } = await Promise.race([nextPromise, endPromise.then(() => ({ value: '', done: true }))]);

        if (done) {
          break;
        }

        yield value;

        streamPromise = new Promise<string>((resolve, reject) => {
          pushValue = resolve;
          pushError = reject;
        });
      }
    } finally {
      subscription.dispose();
    }
  }
}

export class ChatMessage {
  role: 'user' | 'assistant' | 'system';
  content: string;

  constructor(role: 'user' | 'assistant' | 'system', content: string) {
    this.role = role;
    this.content = content;
  }
}