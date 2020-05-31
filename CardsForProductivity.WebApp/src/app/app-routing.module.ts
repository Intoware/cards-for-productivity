import { HostComponent } from './components/host/host.component';
import { HomeComponent } from './components/home/home.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { JoinComponent } from './components/join/join.component';
import { LobbyComponent } from './components/lobby/lobby.component';
import { SessionComponent } from './components/session/session.component';

const routes: Routes = [
  { path: 'host', component: HostComponent },
  { path: 'join', component: JoinComponent },
  { path: 'lobby', component: LobbyComponent },
  { path: 'session', component: SessionComponent },
  { path: '**', component: HomeComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
