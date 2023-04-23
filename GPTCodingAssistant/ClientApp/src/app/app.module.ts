import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ChatComponent } from './chat/chat.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { SessionComponent } from './session/session.component';
import { LeftNavMenuComponent } from './left-nav-menu/left-nav-menu.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    ChatComponent,
    CounterComponent,
    FetchDataComponent,
    SessionComponent,
    LeftNavMenuComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: SessionComponent, pathMatch: 'full' },
      { path: 'chat/{sessionId}', component: ChatComponent },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
