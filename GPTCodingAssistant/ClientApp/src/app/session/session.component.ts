import { Component, OnInit } from '@angular/core';
import { SessionApiService } from '../services/session-api.service';

@Component({
  selector: 'app-session',
  template: `
    <p>
      session works!
    </p>
  `,
  styles: [
  ]
})
export class SessionComponent implements OnInit {
  constructor(private sessionApi: SessionApiService) { }

  ngOnInit(): void {
    this.sessionApi.getSessions().then((data) => console.log(data));
  }
}
