import { HostedSession } from './../../models/HostedSession';
import { StoryModel } from './../../models/StoryModel';
import { SessionStateResponse } from './../../models/SessionStateResponse';
import { SessionService } from 'src/app/services/session.service';
import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnectionState } from '@microsoft/signalr';
import { UserModel } from 'src/app/models/UserModel';
import { HubListener } from 'src/app/models/HubListener';
import { StorySummaryModel } from 'src/app/models/StorySummaryModel';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.scss']
})
export class SessionComponent implements OnInit {

  currentSession: SessionStateResponse;
  hostedSession: HostedSession;

  sessionHubConnection: signalR.HubConnection;
  hubListeners: HubListener[] = [];

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
  editingStory: StoryModel;
  editingStories: StoryModel[];
  isEditingStory: boolean;
  insertionMode = 'InsertAtTop';

  summaryTableDataSource: StorySummaryModel[];
  summaryTableDisplayedColumns: string[] = ['position', 'title', 'minimum', 'average', 'maximum'];

  constructor(private router: Router,
    private sessionService: SessionService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar) {
    this.currentSession = this.sessionService.getCurrentSession();

    if (!this.currentSession) {
      this.navigateTo('');
    }

    if (this.currentSession.hasFinished) {
      this.endSession();
    }
  }

  ngOnInit(): void {
    if (!this.currentSession.hasFinished) {
      this.initializeSession();
    } else {
      this.loadedSession = true;
    }
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
    if (this.sessionHubConnection) {
      this.sessionHubConnection.stop();
      this.unregisterListeners();
    }

    this.sessionService.resetSession();
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

  getSummary(story: StoryModel): StorySummaryModel {
    return {
      title: story.title,
      minimum: this.getMinimumSelection(story),
      average: this.getAverageSelection(story),
      maximum: this.getMaximumSelection(story)
    };
  }

  endSessionClicked() {
    this.sessionHubConnection.invoke('EndSession', this.currentSession.sessionId, this.hostedSession.hostCode).then(null, err => {
      console.error(`[SessionHub] Error ending session: ${err}`);
    });
  }

  displayDialog(template: TemplateRef<any>) {
    this.dialog.open(template);
  }

  editStories(template: TemplateRef<any>) {
    this.editingStories = JSON.parse(JSON.stringify(this.currentSession.stories));
    this.dialog.open(template);
  }

  addStoryClicked(template: TemplateRef<any>) {
    this.editingStory = { title: '', description: '' };
    this.isEditingStory = false;
    this.displayDialog(template);
  }

  deleteStory(story: StoryModel) {
    if (story.isSelected) {
      this.selectStory(this.editingStories[0]);
    }

    const removalIndex = this.editingStories.indexOf(story);
    this.editingStories.splice(removalIndex, 1);

    if (this.editingStories.length === 0) {
      this.addStory({ title: '', description: '' });
    }
  }

  cloneStory(story: StoryModel) {
    const storyToAdd = Object.assign({}, story);
    storyToAdd.title = !storyToAdd.title.trim() ? '' : `${storyToAdd.title} (copy)`;

    if (storyToAdd.title.trim()) {
      while (!this.isTitleUnique(storyToAdd, this.editingStories, true)) {
        storyToAdd.title = `${storyToAdd.title} (copy)`;
      }
    }

    this.editingStories[this.editingStories.length] = storyToAdd;
    this.selectStory(storyToAdd);
  }

  storySelected(story: StoryModel, template: TemplateRef<any>) {
    this.selectStory(story);
    this.isEditingStory = true;
    this.displayDialog(template);
  }

  saveStory() {
    let index = -1;

    if (!this.isEditingStory) {
      switch (this.insertionMode) {
        case 'InsertAtTop':
          index = 0;
          break;
        default:
          const storyToInsertBeneath = this.editingStories.filter((v) => v.title === this.insertionMode.replace('S:', ''))[0];
          index = this.editingStories.indexOf(storyToInsertBeneath) + 1;
      }

      this.addStory(this.editingStory, index);
    } else {
      const storyToEdit = this.editingStories.filter((v) => v.storyId === this.editingStory.storyId)[0];
      storyToEdit.title = this.editingStory.title;
      storyToEdit.description = this.editingStory.description;
    }
  }

  saveChanges() {
    this.sessionHubConnection.invoke('UpdateStories', this.currentSession.sessionId,
      this.hostedSession.hostCode, this.editingStories.filter((v) => !v.isDeleted)).then(null, err => {
        console.error(`[SessionHub] Error updating stories: ${err}`);
      });
  }

  storyChangesValid(): boolean {
    let valid = true;

    for (const story of this.editingStories) {
      if (!this.isTitleUnique(story, this.editingStories, false)) {
        story.isDuplicate = true;
        valid = false;
      } else {
        story.isDuplicate = false;
      }

      if (!story.title.trim()) {
        valid = false;
      }
    }

    return valid;
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
      this.getSessionState();
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
    this.displaySnackbar(`${user.nickname} has left the session`);
    const removalIndex = this.currentSession.users.indexOf(user);
    this.currentSession.users.splice(removalIndex, 1);
    this.sessionService.setCurrentSessionUsers(this.currentSession.users);
  }

  private userDisconnected(user: UserModel) {
    console.log(`[SessionHub] UserDisconnected: ${user.nickname}`);
    this.currentSession.users.find(u => u.userId === user.userId).isOnline = false;
    this.sessionService.setCurrentSessionUsers(this.currentSession.users);
  }

  private gotSessionState(state: SessionStateResponse) {
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

    if (this.currentSession.hasFinished) {
      this.endSession();
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

  private endSession() {
    this.summaryTableDataSource = [];
    for (let i = 0; i < this.currentSession.stories.length; i++) {
      const story = this.currentSession.stories[i];
      if (story) {
        this.summaryTableDataSource[i] = this.getSummary(story);
        this.summaryTableDataSource[i].position = i + 1;
      }
    }

    if (this.sessionHubConnection) {
      this.unregisterListeners();
      this.sessionHubConnection.stop();
      this.currentSession.hasFinished = true;
    }
  }

  private registerListeners() {
    this.hubListeners.push({ name: 'GetSessionState', newMethod: (state) => { this.gotSessionState(state); } });
    this.hubListeners.push({ name: 'UserList', newMethod: (users) => { this.userList(users); } });
    this.hubListeners.push({ name: 'UserConnected', newMethod: (user) => { this.userConnected(user); } });
    this.hubListeners.push({ name: 'UserLeft', newMethod: (user) => { this.userLeft(user); } });
    this.hubListeners.push({ name: 'UserDisconnected', newMethod: (user) => { this.userDisconnected(user); } });
    this.hubListeners.push({ name: 'MakePointSelection', newMethod: (story) => { this.makePointSelection(story); } });
    this.hubListeners.push({ name: 'RevealCurrentStory', newMethod: () => { this.revealCurrentStory(); } });
    this.hubListeners.push({ name: 'CurrentStoryChanged', newMethod: (storyId) => { this.currentStoryChanged(storyId); } });
    this.hubListeners.push({ name: 'EndSession', newMethod: () => { this.endSession(); } });
    this.hubListeners.push({ name: 'StoriesUpdated', newMethod: () => { this.storiesUpdated(); } });

    for (const listener of this.hubListeners) {
      this.sessionHubConnection.on(listener.name, listener.newMethod);
    }
  }

  private unregisterListeners() {
    for (const listener of this.hubListeners) {
      this.sessionHubConnection.off(listener.name);
    }
  }

  private getMinimumSelection(story: StoryModel): string {
    let lowestIndex = -1;

    for (const key in story.userPoints) {
      if (key) {
        const selection = story.userPoints[key];
        const indexInChoices = this.currentSession.pointChoices.indexOf(selection);

        if (lowestIndex === -1 || lowestIndex > indexInChoices) {
          lowestIndex = indexInChoices;
        }
      }
    }

    return this.currentSession.pointChoices[lowestIndex];
  }

  private getAverageSelection(story: StoryModel): string {
    const selectionRecord = {} as Record<string, number>;

    let maxNumberOfOccurrences = -1;
    let pointsWithHighestOccurrences = [];

    for (const key in story.userPoints) {
      if (key) {
        const selection = story.userPoints[key];
        const currentRecord = selectionRecord[selection];

        if (currentRecord) {
          selectionRecord[selection] = currentRecord + 1;
        } else {
          selectionRecord[selection] = 1;
        }

        const selectionFrequency = selectionRecord[selection];

        if (maxNumberOfOccurrences === -1 || maxNumberOfOccurrences < selectionFrequency) {
          pointsWithHighestOccurrences = [selection];
          maxNumberOfOccurrences = selectionFrequency;
        } else if (maxNumberOfOccurrences === selectionFrequency) {
          pointsWithHighestOccurrences[pointsWithHighestOccurrences.length] = selection;
        }
      }
    }

    const sortedPoints = [];

    for (const i in this.currentSession.pointChoices) {
      if (i) {
        const currentPointChoice = this.currentSession.pointChoices[i];

        if (pointsWithHighestOccurrences.indexOf(currentPointChoice) !== -1) {
          sortedPoints[sortedPoints.length] = currentPointChoice;
        }
      }
    }

    return sortedPoints.join(', ');
  }

  private getMaximumSelection(story: StoryModel): string {
    let highestIndex = -1;

    for (const key in story.userPoints) {
      if (key) {
        const selection = story.userPoints[key];
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

  private displaySnackbar(message: string) {
    this.snackbar.open(message, 'Dismiss', {
      duration: 5000
    });
  }

  private storiesUpdated() {
    this.getSessionState();
    this.displaySnackbar('The host has updated the stories');
  }

  private addStory(story: StoryModel, index: number = -1) {
    if (index === -1) {
      index = this.editingStories.length;
    }

    this.editingStories.splice(index, 0, story);

    this.selectStory(this.editingStories[this.editingStories.length - 1]);
  }

  private selectStory(story: StoryModel) {
    for (const otherStory of this.editingStories) {
      otherStory.isSelected = false;
    }

    story.isSelected = true;
    this.editingStory = Object.assign({}, story);
  }

  private isTitleUnique(story: StoryModel, stories: StoryModel[], checkForNewStory: boolean): boolean {
    return stories.filter((s) => {
      return s.title.trim() === story.title.trim();
    }).length === (checkForNewStory ? 0 : 1);
  }

  private getSessionState() {
    this.loadedSession = false;
    this.sessionHubConnection.invoke('GetSessionState', this.sessionService.getClientRequestDetails()).then(null, err => {
      console.error(`[SessionHub] Error getting session state: ${err}`);
    });
  }
}
