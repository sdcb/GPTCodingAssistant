import { Component } from '@angular/core';
import { SessionApiService, SessionSimpleResponse } from '../services/session-api.service';

@Component({
  selector: 'app-left-nav-menu',
  template: `
<div class="d-flex flex-column flex-shrink-0 p-3 wrapper">
    <a href="/" class="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-dark text-decoration-none">
        <span class="fs-6">GPT编程助手</span>
    </a>
    <hr>
    <ul class="nav nav-pills flex-fill flex-column mb-auto">
      <li class="nav-item" *ngFor="let session of sessions; let i = index" 
          [routerLinkActive]="['link-active']"
          [routerLinkActiveOptions]="{ exact: true }">
         <a class="d-flex flex-column flex-shrink-0 nav-link" [routerLink]="['/chat/' + session.sessionId]" [ngClass]="i === activeSessionIndex ? 'active' : ''" aria-current="page" (click)="selectSessionIndex(i)">
              <div class="d-flex w-100 justify-content-between">
                <div class="flex-grow-1">{{session.title}}</div>
                <div style="color: red" (click)="deleteSession(i); $event.stopPropagation()">×</div>
              </div>
          </a>
      </li>
    </ul>
    <hr>
    <div class="footer">
        <a class="d-flex align-items-center link-dark text-decoration-none">
            <strong>欢迎 {{ip}}</strong>
        </a>
    </div>
</div>
  `,
  styleUrls: ['./left-nav-menu.component.css']
})
export class LeftNavMenuComponent {
  sessions: SessionSimpleResponse[] = [];
  activeSessionIndex = -1;
  ip: string = 'loading...';

  constructor(private sessionApi: SessionApiService) {
    this.reload();
    this.sessionApi.echoIp().then(ip => this.ip = ip);
  }

  selectSessionIndex(sessionIndex: number) {
    this.activeSessionIndex = sessionIndex;
  }

  async deleteSession(sessionIndex: number) {
    const toDelete = this.sessions[sessionIndex];

    if (confirm(`确定要删除会话“${toDelete.title}”?`)) {
      await this.sessionApi.deleteSession(toDelete.sessionId);
      this.sessions = await this.sessionApi.getSessions();
      if (this.activeSessionIndex === this.sessions.length) {
        this.activeSessionIndex -= 1;
      }
    }
  }

  private reload() {
    this.sessionApi.getSessions().then(data => this.sessions = data);
  }
}
