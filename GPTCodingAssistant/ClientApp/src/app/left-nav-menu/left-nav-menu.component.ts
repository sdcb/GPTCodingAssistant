import { Component, OnInit, signal } from '@angular/core';
import { SessionApiService, SessionSimpleResponse } from '../services/session-api.service';
import { ActivatedRoute, NavigationEnd, Route, Router } from '@angular/router';
import { filter } from 'rxjs';

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
         <a class="d-flex flex-column flex-shrink-0 nav-link" [routerLink]="['/chat/' + session.sessionId]" [ngClass]="session.sessionId === activeSessionId() ? 'active' : ''" aria-current="page" (click)="selectSessionId(session.sessionId)">
              <div class="d-flex w-100 justify-content-between">
                <div class="flex-grow-1">{{session.title}}</div>
                <div style="color: red" (click)="deleteSession(session.sessionId); $event.stopPropagation()">×</div>
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
export class LeftNavMenuComponent implements OnInit {
  sessions: SessionSimpleResponse[] = [];
  // activeSessionIndex = -1;
  activeSessionId = signal<number | undefined>(undefined);
  ip: string = 'loading...';

  constructor(private sessionApi: SessionApiService, private router: Router, private activatedRoute: ActivatedRoute) {
    this.reload();
    this.sessionApi.echoIp().then(ip => this.ip = ip);
  }

  ngOnInit(): void {
    this.router.events
    .pipe(filter(event => event instanceof NavigationEnd))
    .subscribe(() => {
      const root = this.activatedRoute.root;
      const chatRoute = findChatRoute(root);

      if (chatRoute) {
        this.activeSessionId.set(parseInt(chatRoute.snapshot.paramMap.get('sessionId')!));
        console.log(this.activeSessionId());
      }
    });

    function findChatRoute(route: ActivatedRoute): ActivatedRoute | null {
      if (route.firstChild) {
        if (route.firstChild.routeConfig && route.firstChild.routeConfig.path === 'chat/:sessionId') {
          return route.firstChild;
        } else {
          return findChatRoute(route.firstChild);
        }
      }
      return null;
    }
  }

  selectSessionId(sessionId: number) {
    this.activeSessionId.set(sessionId);
  }

  async deleteSession(sessionId: number) {
    const toDelete = this.sessions.filter(x => x.sessionId === sessionId)[0];

    if (confirm(`确定要删除会话“${toDelete.title}”?`)) {
      await this.sessionApi.deleteSession(toDelete.sessionId);
      await this.reload();
      console.log(`trigger reload, set activeSessionId: ${this.sessions.length > 0 ? this.sessions[0].sessionId : undefined}`);
      this.activeSessionId.set(this.sessions.length > 0 ? this.sessions[0].sessionId : undefined);
    }
  }

  private async reload() {
    this.sessions = await this.sessionApi.getSessions();
  }
}
