import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';

@Component({
  selector: 'app-home',
  styles: [`
.scrollable-chat {
  max-height: calc(100vh - 150px); /* 你可以根据需要自定义高度 */
  overflow-y: auto;
  margin-bottom: 10px; /* 为了避免与输入区域重叠，你可以添加一些底部外边距 */
}
`],
  template: `
<ul class="scrollable-chat" #uiChatList>
  <li *ngFor="let chatItem of chatHistory">
    <strong>{{chatItem.role}}</strong>: <pre>{{chatItem.content}}</pre>
  </li>
</ul>
<div class="container-fluid fixed-bottom input-group mb-3">
  <textarea type="text" class="form-control" [placeholder]="inputPlaceholder" [ariaLabel]="inputPlaceholder" aria-describedby="basic-addon2" [(ngModel)]="userInput"></textarea>
  <button (click)="send()" class="btn btn-outline-secondary" type="button">
    <svg stroke="currentColor" fill="none" stroke-width="2" viewBox="0 0 24 24" stroke-linecap="round" stroke-linejoin="round" class="h-4 w-4 mr-1" height="1em" width="1em" xmlns="http://www.w3.org/2000/svg">
      <line x1="22" y1="2" x2="11" y2="13"></line>
      <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
    </svg>
  </button>
</div>
  `
})
export class HomeComponent {
  userInput = '';
  inputPlaceholder = '输入您的消息, 可按Enter发送, Shift+Enter换行…';
  chatHistory = <ChatMessage[]>[1,2,3,4,5,6,7,8,9,10,11,12,13,14].map(x => new ChatMessage('system', `这是系统消息 ${x}`));
  api = new Api();

  @ViewChild('uiChatList', { static: true }) uiChatList!: ElementRef<HTMLUListElement>;

  @HostListener('keydown.enter', ['$event'])
  onCtrlEnter(event: KeyboardEvent) {
    this.send();
  }

  async send() {
    this.chatHistory = [...this.chatHistory, new ChatMessage('user', this.userInput)];
    const resp = new ChatMessage('assistant', '');
    this.chatHistory = [...this.chatHistory, resp];
    setTimeout(() => {
      this.uiChatList.nativeElement.scrollTop = this.uiChatList.nativeElement.scrollHeight;
    }, 0);
    
    for await (const c of this.api.chat(this.userInput)) {
      resp.content += c;
    }
    this.userInput = '';
  }
}

export class Api {
  public async* chat(input: string) {
    const response = `Hello, ${input}!`;
    for (const c of response) {
      yield c;
      await delay(10);
    }
  }
}

function delay(ms: number): Promise<void> {
  return new Promise<void>(resolve => setTimeout(resolve, ms));
}

export class ChatMessage {
  role: 'user' | 'assistant' | 'system';
  content: string;

  constructor(role: 'user' | 'assistant' | 'system', content: string) {
    this.role = role;
    this.content = content;
  }
}