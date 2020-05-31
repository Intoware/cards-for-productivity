import { HostedSession } from './../../models/HostedSession';
import { StoryModel } from './../../models/StoryModel';
import { SessionStateResponse } from './../../models/SessionStateResponse';
import { SessionService } from 'src/app/services/session.service';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnectionState } from '@microsoft/signalr';
import { UserModel } from 'src/app/models/UserModel';
import { HubListener } from 'src/app/models/HubListener';
import { StorySummaryModel } from 'src/app/models/StorySummaryModel';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.scss']
})
export class SessionComponent implements OnInit {

  currentSession: SessionStateResponse;
  hostedSession: HostedSession;

  sessionHubConnection: signalR.HubConnection;
  hubListners: HubListener[] = [];

  cardColors = [
    '#00CEAD', '#00D254', '#0097E7',
    '#A350C1', '#00A381', '#0080C5',
    '#983ABA', '#F7C400', '#F79417',
    '#FA6756', '#E24A00', '#D02911'
  ];

  loadedSession: boolean;

  pendingIcon = '<i class="far fa-clock"></i>';
  tickIcon = '<i class="far fa-check-circle"></i>';

  pointChoiceCardWidth = 70;
  pointChoiceCardHeight = 100;

  confirmedPointsForCurrentStory: boolean;
  hasSelectedPointsForCurrentStory: boolean;
  selectedPointsForCurrentStory: string;

  currentStory: StoryModel;

  constructor(private router: Router,
              private sessionService: SessionService) {
    this.currentSession = this.sessionService.getCurrentSession();

    if (!this.currentSession) {
      this.navigateTo('');
    }
  }

  ngOnInit(): void {
    this.initializeSession();
  }

  pointsSelected(points: string) {
    this.hasSelectedPointsForCurrentStory = true;
    this.selectedPointsForCurrentStory = points;
  }

  confirmSelection() {
    this.confirmedPointsForCurrentStory = true;

    this.sessionHubConnection.invoke('MakePointSelection', this.sessionService.getClientRequestDetails(),
      this.selectedPointsForCurrentStory).then(null, err => {
      console.error(`[SessionHub] Error making point selection: ${err}`);
    });
  }

  leaveSession() {
    this.sessionHubConnection.stop();
    this.sessionService.resetSession();
    this.unregisterListeners();
    this.navigateTo('');
  }

  getSelectionForUser(userId: string): string {
    const story = this.currentStory;
    const selection = story.userPoints[userId];
    return selection;
  }

  revealCurrentStoryClicked() {
    this.sessionHubConnection.invoke('RevealCurrentStory', this.currentSession.sessionId, this.hostedSession.hostCode).then(null, err => {
      console.error(`[SessionHub] Error revealing current story: ${err}`);
    });
  }

  nextStory() {
    const currentStoryIndex = this.currentSession.stories.indexOf(this.currentStory);
    const nextStoryIndex = currentStoryIndex + 1;
    const nextStory = this.currentSession.stories[nextStoryIndex];

    if (nextStory) {
      this.setCurrentStory(nextStory.storyId);
    }
  }

  previousStory() {
    const currentStoryIndex = this.currentSession.stories.indexOf(this.currentStory);
    const previousStoryIndex = currentStoryIndex - 1;
    const previousStory = this.currentSession.stories[previousStoryIndex];

    if (previousStory) {
      this.setCurrentStory(previousStory.storyId);
    }
  }

  getSummary(): StorySummaryModel {
    return {
      minimum: this.getMinimumSelection(),
      average: this.getAverageSelection(),
      maximum: this.getMaximumSelection()
    };
  }

  private navigateTo(path: string) {
    this.router.navigate([path]);
  }

  private initializeSession() {
    this.sessionHubConnection = this.sessionService.getConnection();
    this.sessionHubConnection.start().then(() => {
      this.setupSession();
    }, err => {
      switch (this.sessionHubConnection.state) {
        case HubConnectionState.Connected:
          this.setupSession();
          break;
        default:
          console.log(err);
          break;
      }
    });
  }

  private setupSession() {
    this.registerListeners();

    this.sessionHubConnection.invoke('Subscribe', this.sessionService.getClientRequestDetails()).then(() => {
      this.sessionHubConnection.invoke('GetSessionState', this.sessionService.getClientRequestDetails()).then(null, err => {
        console.error(`[SessionHub] Error getting session state: ${err}`);
      });
    }, err => {
      console.error(`[SessionHub] Error subscribing to SessionHub: ${err}`);
    });
  }

  private userList(users: UserModel[]) {
    console.log(`[SessionHub] UserList: ${users.length} users`);
    this.currentSession.users = users;
    this.sessionService.setCurrentSessionUsers(this.currentSession.users);
  }

  private userConnected(user: UserModel) {
    console.log(`[SessionHub] UserConnected: ${user.nickname}`);

    const userFilter = this.currentSession.users.filter((u) => u.userId === user.userId);

    if (userFilter.length === 0) {
      this.currentSession.users[this.currentSession.users.length] = user;
    } else {
      this.currentSession.users.find(u => u.userId === user.userId).isOnline = true;
    }

    this.sessionService.setCurrentSessionUsers(this.currentSession.users);
  }

  private userLeft(user: UserModel) {
    console.log(`[SessionHub] UserLeft: ${user.nickname}`);
    const removalIndex = this.currentSession.users.indexOf(user);
    this.currentSession.users.splice(removalIndex, 1);
    this.sessionService.setCurrentSessionUsers(this.currentSession.users);
  }

  private userDisconnected(user: UserModel) {
    console.log(`[SessionHub] UserDisconnected: ${user.nickname}`);
    this.currentSession.users.find(u => u.userId === user.userId).isOnline = false;
    this.sessionService.setCurrentSessionUsers(this.currentSession.users);
  }

  private getSessionState(state: SessionStateResponse) {
    console.log(`[SessionHub] GetSessionState: ${state.sessionId}`);
    this.currentSession = state;
    this.sessionService.setCurrentSession(state);

    this.currentStory = this.currentSession.stories.filter((s) => s.storyId === this.currentSession.currentStoryId)[0];

    if (this.getSelectionForUser(this.currentSession.userId)) {
      this.confirmedPointsForCurrentStory = true;
    }

    if (this.currentSession.isHost) {
      this.hostedSession = this.sessionService.getHostVariables();
    }

    this.loadedSession = true;
  }

  private makePointSelection(story: StoryModel) {
    console.log(`[SessionHub] MakePointSelection: ${story.storyId}`);
    this.currentSession.stories.find((s) => s.storyId === story.storyId).userPoints = story.userPoints;
  }

  private revealCurrentStory() {
    console.log('[SessionHub] RevealCurrentStory');
    this.currentSession.stories.filter((s) => s.storyId === this.currentSession.currentStoryId)[0].revealed = true;
  }

  private currentStoryChanged(storyId: string) {
    console.log(`[SessionHub] CurrentStoryChanged: ${storyId}`);
    this.currentSession.currentStoryId = storyId;

    this.confirmedPointsForCurrentStory = false;
    this.hasSelectedPointsForCurrentStory = false;
    this.selectedPointsForCurrentStory = undefined;

    this.currentStory = this.currentSession.stories.filter((s) => s.storyId === this.currentSession.currentStoryId)[0];
  }

  private registerListeners() {
    this.hubListners.push({ name: 'GetSessionState', newMethod: (state) => { this.getSessionState(state); } });
    this.hubListners.push({ name: 'UserList', newMethod: (users) => { this.userList(users); } });
    this.hubListners.push({ name: 'UserConnected', newMethod: (user) => { this.userConnected(user); } });
    this.hubListners.push({ name: 'UserLeft', newMethod: (user) => { this.userLeft(user); } });
    this.hubListners.push({ name: 'UserDisconnected', newMethod: (user) => { this.userDisconnected(user); } });
    this.hubListners.push({ name: 'MakePointSelection', newMethod: (story) => { this.makePointSelection(story); } });
    this.hubListners.push({ name: 'RevealCurrentStory', newMethod: () => { this.revealCurrentStory(); } });
    this.hubListners.push({ name: 'CurrentStoryChanged', newMethod: (storyId) => { this.currentStoryChanged(storyId); } });

    for (const listener of this.hubListners) {
      this.sessionHubConnection.on(listener.name, listener.newMethod);
    }
  }

  private unregisterListeners() {
    for (const listener of this.hubListners) {
      this.sessionHubConnection.off(listener.name);
    }
  }

  private getMinimumSelection(): string {
    let lowestIndex = -1;
    const currentStory = this.currentStory;

    for (const key in currentStory.userPoints) {
      if (key) {
        const selection = currentStory.userPoints[key];
        const indexInChoices = this.currentSession.pointChoices.indexOf(selection);

        if (lowestIndex === -1 || lowestIndex > indexInChoices) {
          lowestIndex = indexInChoices;
        }
      }
    }

    return this.currentSession.pointChoices[lowestIndex];
  }

  private getAverageSelection(): string {
    const selectionRecord = { } as Record<string, number>;

    const currentStory = this.currentStory;

    let maxNumberOfOccurrences = -1;
    let pointsWithHighestOccurrences = '';

    for (const key in currentStory.userPoints) {
      if (key) {
        const selection = currentStory.userPoints[key];
        const currentRecord = selectionRecord[selection];

        if (currentRecord) {
          selectionRecord[selection] = currentRecord + 1;
        } else {
          selectionRecord[selection] = 1;
        }

        const selectionFrequency = selectionRecord[selection];

        if (maxNumberOfOccurrences === -1 || maxNumberOfOccurrences < selectionFrequency) {
          pointsWithHighestOccurrences = selection;
          maxNumberOfOccurrences = selectionFrequency;
        }
      }
    }

    return pointsWithHighestOccurrences;
  }

  private getMaximumSelection(): string {
    let highestIndex = -1;
    const currentStory = this.currentStory;

    for (const key in currentStory.userPoints) {
      if (key) {
        const selection = currentStory.userPoints[key];
        const indexInChoices = this.currentSession.pointChoices.indexOf(selection);

        if (highestIndex === -1 || highestIndex < indexInChoices) {
          highestIndex = indexInChoices;
        }
      }
    }

    return this.currentSession.pointChoices[highestIndex];
  }

  private setCurrentStory(storyId: string) {
    this.sessionHubConnection.invoke('CurrentStoryChanged', this.currentSession.sessionId,
      this.hostedSession.hostCode, storyId).then(null, err => {
      console.error(`[SessionHub] Error changing current story: ${err}`);
    });
  }
}
