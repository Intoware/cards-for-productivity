import { StoryModel } from './../../models/StoryModel';
import { CreateSessionResponse } from './../../models/CreateSessionResponse';
import { SessionService } from './../../services/session.service';
import { CreateSessionRequest } from './../../models/CreateSessionRequest';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-host',
  templateUrl: './host.component.html',
  styleUrls: ['./host.component.scss']
})
export class HostComponent implements OnInit {

  createSessionRequest = {
    nickname: null,
    stories: [
      {
        title: '',
        description: ''
      }
    ]
  } as CreateSessionRequest;

  result: string;

  isLoading: boolean;
  userSetHostInformation: boolean;

  currentStory: StoryModel;

  constructor(private router: Router,
              private sessionService: SessionService) {
  }

  ngOnInit() {
    this.selectStory(this.createSessionRequest.stories[0]);
  }

  createSession() {
    this.isLoading = true;

    this.sessionService.createSession(this.createSessionRequest).subscribe((response: CreateSessionResponse) => {
      this.sessionService.setHostVariables(this.createSessionRequest, response);
      this.isLoading = false;
      this.navigateTo('lobby');
    }, err => {
      this.isLoading = false;
    });
  }

  recreateSession() {
    this.isLoading = true;

    this.sessionService.recreateSession().subscribe((response: CreateSessionResponse) => {
      const hostedSession = localStorage.getItem('hostedSession');
      this.sessionService.setHostVariables(this.createSessionRequest, response);
      this.isLoading = false;
    }, err => {
      this.isLoading = false;
    });
  }

  goBack() {
    this.router.navigate(['']);
  }

  hostInformationEntered() {
    this.userSetHostInformation = true;
  }

  deleteStory(story: StoryModel) {
    if (story.isSelected) {
      this.selectStory(this.createSessionRequest.stories[0]);
    }

    const removalIndex = this.createSessionRequest.stories.indexOf(story);
    this.createSessionRequest.stories.splice(removalIndex, 1);

    if (this.createSessionRequest.stories.length === 0) {
      this.addStory();
    }
  }

  cloneStory(story: StoryModel) {
    const storyToAdd = Object.assign({}, story);
    storyToAdd.title = !storyToAdd.title.trim() ? '' : `${storyToAdd.title} (copy)`;

    if (storyToAdd.title.trim()) {
      while (!this.isTitleUnique(storyToAdd, this.createSessionRequest.stories, true)) {
        storyToAdd.title = `${storyToAdd.title} (copy)`;
      }
    }

    this.createSessionRequest.stories[this.createSessionRequest.stories.length] = storyToAdd;
    this.selectStory(storyToAdd);
  }

  storySelected(story: StoryModel) {
    this.selectStory(story);
  }

  addStory() {
    this.createSessionRequest.stories[this.createSessionRequest.stories.length] = {
      title: '',
      description: ''
    };

    this.selectStory(this.createSessionRequest.stories[this.createSessionRequest.stories.length - 1]);
  }

  createSessionRequestValid(): boolean {
    let valid = true;

    if (!this.createSessionRequest.nickname.trim()) {
      valid = false;
    }

    for (const story of this.createSessionRequest.stories) {
      if (!this.isTitleUnique(story, this.createSessionRequest.stories, false)) {
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

  isTitleUnique(story: StoryModel, stories: StoryModel[], checkForNewStory: boolean): boolean {
    return stories.filter((s) => {
      return s.title === story.title;
    }).length === (checkForNewStory ? 0 : 1);
  }

  private selectStory(story: StoryModel) {
    for (const otherStory of this.createSessionRequest.stories) {
      otherStory.isSelected = false;
    }

    story.isSelected = true;
    this.currentStory = story;
  }

  private navigateTo(path: string) {
    this.router.navigate([path]);
  }
}
