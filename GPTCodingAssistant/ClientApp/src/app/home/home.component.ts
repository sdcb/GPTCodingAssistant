import { Component, ElementRef, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { ChatMessage, RealApi } from './RealApi';

@Component({
  selector: 'app-home',
  styles: [`
.chat {
  max-height: calc(100vh - 150px); /* 你可以根据需要自定义高度 */
  overflow-y: auto;
  margin-bottom: 10px; /* 为了避免与输入区域重叠，你可以添加一些底部外边距 */
}
.chat .resp {
  white-space: pre-wrap;
}

.chat li {
  list-style: none;
}

.chat li.user {
  text-align: right;
  background-color: #e6f3ff;
}

.chat li.assistant {
  background-color: #f0f0f0;
}
`],
  template: `
<ul class="chat" #uiChatList>
  <li *ngFor="let chatItem of chatHistory" [ngClass]="chatItem.role">
    <strong>{{chatItem.role}}</strong>: <pre class="resp">{{chatItem.content}}</pre>
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
export class HomeComponent implements OnInit {
  userInput = '';
  inputPlaceholder = '输入您的消息, 可按Enter发送, Shift+Enter换行…';
  chatHistory = <ChatMessage[]>[];
  api: RealApi;;

  constructor(@Inject('BASE_URL') baseUrl: string) {
    console.log(baseUrl);
    this.api = new RealApi(baseUrl);
  }
  ngOnInit(): void {
    this.api.getList().then(list => this.chatHistory = list);
  }

  @ViewChild('uiChatList', { static: true }) uiChatList!: ElementRef<HTMLUListElement>;

  @HostListener('keydown.enter', ['$event'])
  onCtrlEnter(event: KeyboardEvent) {
    this.send();
  }

  async send() {
    this.chatHistory = [...this.chatHistory, new ChatMessage('user', this.userInput)];
    const resp = new ChatMessage('assistant', '');
    this.chatHistory = [...this.chatHistory, resp];

    this.delayCleanUserInput();
    this.scrollToBottom();
    for await (const c of this.api.chat(this.userInput)) {
      resp.content += c;
      if (c == '\n' || c == '\r') {
        this.scrollToBottom();
      }
    }
  }

  delayCleanUserInput() {
    setTimeout(() => {
      this.userInput = '';
    }, 1);
  }

  scrollToBottom() {
    setTimeout(() => {
      this.uiChatList.nativeElement.scrollTop = this.uiChatList.nativeElement.scrollHeight;
    }, 1);
  }
}

