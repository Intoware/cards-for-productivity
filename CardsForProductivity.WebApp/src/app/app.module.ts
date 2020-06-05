import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { HostComponent } from './components/host/host.component';
import { JoinComponent } from './components/join/join.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { SidebarItemComponent } from './components/sidebar-item/sidebar-item.component';
import { MatRippleModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { LobbyComponent } from './components/lobby/lobby.component';
import { SessionComponent } from './components/session/session.component';
import { CardItemComponent } from './components/card-item/card-item.component';
import { GithubCornerComponent } from './components/github-corner/github-corner.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    HostComponent,
    JoinComponent,
    SidebarItemComponent,
    LobbyComponent,
    SessionComponent,
    CardItemComponent,
    GithubCornerComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    FormsModule,
    MatRippleModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTableModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
