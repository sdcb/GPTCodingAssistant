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
      <li class="nav-item" *ngFor="let session of sessions; let i = index">
         <a class="d-flex flex-column flex-shrink-0 nav-link" href="javascript:void(0)" [ngClass]="i === activeSessionId ? 'active' : ''" aria-current="page" (click)="selectSessionId(i)">
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
            <strong>欢迎!</strong>
        </a>
    </div>
</div>
  `,
  styleUrls: ['./left-nav-menu.component.css']
})
export class LeftNavMenuComponent {
  sessions: SessionSimpleResponse[] = [];
  activeSessionId = 0;

  constructor(private sessionApi: SessionApiService) {
    this.sessionApi.getSessions().then(data => this.sessions = data);
  }

  selectSessionId(sessionId: number) {
    this.activeSessionId = sessionId;
  }

  deleteSession(sessionId: number) {
    alert('delete session: ' + sessionId);
  }
}
