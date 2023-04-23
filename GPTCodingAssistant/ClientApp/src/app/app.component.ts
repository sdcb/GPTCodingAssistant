import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
<body>
<div class="d-flex flex-row">
    <div class="flex-shrink-0 bg-light" style="width: 280px;">
      <app-left-nav-menu></app-left-nav-menu>
    </div>
    <main class="flex-grow-1 p-3">
      <router-outlet></router-outlet>
    </main>
  </div>
</body>`
})
export class AppComponent {
  title = 'app';
}
