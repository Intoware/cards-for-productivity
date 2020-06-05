import { HubListener } from './../../models/HubListener';
import { ClientRequestDetails } from './../../models/ClientRequestDetails';
import { SessionStateResponse } from './../../models/SessionStateResponse';
import { UserModel } from './../../models/UserModel';
import { SessionService } from 'src/app/services/session.service';
import { HostedSession } from './../../models/HostedSession';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { JoinedSession } from 'src/app/models/JoinedSession';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit {

  hostedSession: HostedSession;
  joinedSession: JoinedSession;
  currentSession: SessionStateResponse;

  sessionCode: string;
  sessionId: string;

  users: UserModel[] = [];
  selectedUser: UserModel;

  isLoading: boolean;

  sessionHubConnection: signalR.HubConnection;
  hubListeners: HubListener[] = [];

  constructor(private router: Router,
              private sessionService: SessionService,
              private dialog: MatDialog) { }

  ngOnInit(): void {
    this.hostedSession = this.sessionService.getHostVariables();
    this.joinedSession = this.sessionService.getJoinVariables();

    if (this.hostedSession) {
      this.users[0] = {
        isHost: true,
        nickname: this.hostedSession.nickname,
        userId: this.hostedSession.userId,
        sessionId: this.hostedSession.sessionId
      };
      this.sessionService.setCurrentSessionUsers(this.users);

      this.setSessionInformation(this.hostedSession.sessionId, this.hostedSession.sessionCode);
    } else if (this.joinedSession) {
      this.users = this.joinedSession.users;
      this.setSessionInformation(this.joinedSession.sessionId, this.joinedSession.sessionCode);
    } else {
      this.navigateTo('');
    }

    this.setupHubConnection();
  }

  startSessionClicked() {
    this.sessionHubConnection.invoke('StartSession', this.sessionId, this.hostedSession.hostCode).then(null, err => {
      console.error(`[SessionHub] Error starting session: ${err}`);
    });
  }

  kickUser(user: UserModel) {
    this.sessionHubConnection.invoke('KickUser', this.sessionId, this.hostedSession.hostCode, user.userId).then(null, err => {
      console.error(`[SessionHub] Error kicking user: ${err}`);
    });
  }

  displayDialog(template: TemplateRef<any>) {
    this.dialog.open(template);
  }

  private setSessionInformation(sessionId: string, sessionCode: string) {
    this.sessionId = sessionId;
    this.sessionCode = sessionCode;
  }

  private setupHubConnection() {
    this.sessionHubConnection = this.sessionService.getConnection();
    this.registerListeners();

    this.sessionHubConnection.start().then(() => {
      console.log('[SessionHub] Connected to SessionHub');

      this.sessionHubConnection.invoke('Subscribe', this.sessionService.getClientRequestDetails()).then(() => {
        console.log('[SessionHub] Subscribed to SessionHub');
      }, err => {
        console.error(`[SessionHub] Error subscribing to SessionHub: ${err}`);
      });
    }, err => {
      console.error(`[SessionHub] Error connecting to SessionHub: ${err}`);
    });
  }

  private userList(users: UserModel[]) {
    console.log(`[SessionHub] UserList: ${users.length} users`);
    this.users = users;
    this.sessionService.setCurrentSessionUsers(this.users);
  }

  private userConnected(user: UserModel) {
    console.log(`[SessionHub] UserConnected: ${user.nickname}`);

    const userFilter = this.users.filter((u) => u.userId === user.userId);

    if (userFilter.length === 0) {
      this.users[this.users.length] = user;
    } else {
      this.users.find(u => u.userId === user.userId).isOnline = true;
    }

    this.sessionService.setCurrentSessionUsers(this.users);
  }

  private userLeft(user: UserModel) {
    console.log(`[SessionHub] UserLeft: ${user.nickname}`);
    const removalIndex = this.users.indexOf(user);
    this.users.splice(removalIndex, 1);
    this.sessionService.setCurrentSessionUsers(this.users);
  }

  private userDisconnected(user: UserModel) {
    console.log(`[SessionHub] UserDisconnected: ${user.nickname}`);
    this.users.find(u => u.userId === user.userId).isOnline = false;
    this.sessionService.setCurrentSessionUsers(this.users);
  }

  private startSession() {
    this.navigateTo('session');
  }

  private userKicked(user: UserModel) {
    this.userLeft(user);

    if (!this.hostedSession && user.userId === this.joinedSession.userId) {
      this.sessionService.resetSession();
      this.sessionHubConnection.stop();
      this.navigateTo('');
    }
  }

  private navigateTo(path: string) {
    this.unregisterListeners();
    this.router.navigate([path]);
  }

  private registerListeners() {
    this.hubListeners.push({ name: 'StartSession', newMethod: () => { this.startSession(); } });
    this.hubListeners.push({ name: 'UserList', newMethod: (users) => { this.userList(users); } });
    this.hubListeners.push({ name: 'UserConnected', newMethod: (user) => { this.userConnected(user); } });
    this.hubListeners.push({ name: 'UserLeft', newMethod: (user) => { this.userLeft(user); } });
    this.hubListeners.push({ name: 'UserDisconnected', newMethod: (user) => { this.userDisconnected(user); } });
    this.hubListeners.push({ name: 'UserKicked', newMethod: (user) => { this.userKicked(user); } });

    for (const listener of this.hubListeners) {
      this.sessionHubConnection.on(listener.name, listener.newMethod);
    }
  }

  private unregisterListeners() {
    for (const listener of this.hubListeners) {
      this.sessionHubConnection.off(listener.name, listener.newMethod);
    }
  }

}
