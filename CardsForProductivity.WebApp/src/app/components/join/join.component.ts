import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { JoinSessionRequest } from 'src/app/models/JoinSessionRequest';
import { Router } from '@angular/router';
import { SessionService } from 'src/app/services/session.service';
import { JoinSessionResponse } from 'src/app/models/JoinSessionResponse';

@Component({
  selector: 'app-join',
  templateUrl: './join.component.html',
  styleUrls: ['./join.component.scss']
})
export class JoinComponent implements OnInit {

  @ViewChild('nickname') nicknameElement: ElementRef;

  joinSessionRequest = {
    nickname: '',
    sessionCode: ''
  } as JoinSessionRequest;

  result: string;

  isLoading: boolean;
  userSetSessionCode: boolean;

  constructor(private router: Router,
              private sessionService: SessionService) {
  }

  ngOnInit() {
  }

  joinSession() {
    this.isLoading = true;

    this.joinSessionRequest.sessionCode = this.joinSessionRequest.sessionCode.toLocaleUpperCase();

    this.sessionService.joinSession(this.joinSessionRequest).subscribe((response: JoinSessionResponse) => {
      this.sessionService.setJoinVariables(this.joinSessionRequest, response);
      this.isLoading = false;

      if (response.hasStarted && !response.hasFinished) {
        this.navigateTo('session');
      } else {
        this.navigateTo('lobby');
      }
    }, err => {
      this.isLoading = false;
    });
  }

  goBack() {
    this.router.navigate(['']);
  }

  sessionCodeEntered() {
    this.userSetSessionCode = true;
    setTimeout(() => this.nicknameElement.nativeElement.focus(), 0);
  }

  private navigateTo(path: string) {
    this.router.navigate([path]);
  }
}
