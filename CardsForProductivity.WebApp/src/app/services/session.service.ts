import { ClientRequestDetails } from './../models/ClientRequestDetails';
import { SessionStateResponse } from './../models/SessionStateResponse';
import { JoinedSession } from './../models/JoinedSession';
import { HostedSession } from './../models/HostedSession';
import { CreateSessionResponse } from './../models/CreateSessionResponse';
import { environment } from './../../environments/environment.prod';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateSessionRequest } from '../models/CreateSessionRequest';
import { JoinSessionRequest } from '../models/JoinSessionRequest';
import { JoinSessionResponse } from '../models/JoinSessionResponse';
import * as signalR from '@microsoft/signalr';
import { UserModel } from '../models/UserModel';

const joinedSessionKey = 'joinedSession';
const hostedSessionKey = 'hostedSession';
const currentSessionKey = 'currentSession';

@Injectable({
    providedIn: 'root'
})

export class SessionService {

    private sessionApi = environment.api + '/api/session';
    private connection: signalR.HubConnection;

    constructor(private http: HttpClient) {
    }

    createSession(createSessionRequest: CreateSessionRequest) {
        return this.http.post(this.sessionApi, createSessionRequest);
    }

    recreateSession() {
        const recreateSessionRequest = JSON.parse(localStorage.getItem(hostedSessionKey)) as CreateSessionRequest;
        return this.http.post(this.sessionApi, recreateSessionRequest);
    }

    joinSession(joinSessionRequest: JoinSessionRequest) {
        return this.http.post(`${this.sessionApi}/join`, joinSessionRequest);
    }

    rejoinSession() {
        const rejoinSessionRequest = JSON.parse(localStorage.getItem(joinedSessionKey)) as JoinSessionRequest;
        return this.http.post(`${this.sessionApi}/join`, rejoinSessionRequest);
    }

    setHostVariables(createSessionRequest: CreateSessionRequest, createSessionResponse: CreateSessionResponse) {
        localStorage.clear();
        let hostedSession = Object.assign({}, createSessionRequest) as HostedSession;
        hostedSession = Object.assign(hostedSession, createSessionResponse);
        localStorage.setItem(hostedSessionKey, JSON.stringify(hostedSession));
        this.inferAndSetCurrentSession(null, hostedSession);
    }

    setJoinVariables(joinSessionRequest: JoinSessionRequest, joinSessionResponse: JoinSessionResponse) {
        localStorage.clear();
        let joinedSession = Object.assign({}, joinSessionRequest) as JoinedSession;
        joinedSession = Object.assign(joinedSession, joinSessionResponse);
        localStorage.setItem(joinedSessionKey, JSON.stringify(joinedSession));
        this.inferAndSetCurrentSession(joinedSession, null);
    }

    setCurrentSessionUsers(users: UserModel[]) {
        const session = this.getCurrentSession();
        session.users = users;
        localStorage.setItem(currentSessionKey, JSON.stringify(session));
    }

    getHostVariables(): HostedSession {
        return JSON.parse(localStorage.getItem(hostedSessionKey)) as HostedSession;
    }

    getJoinVariables(): JoinedSession {
        return JSON.parse(localStorage.getItem(joinedSessionKey)) as JoinedSession;
    }

    getCurrentSession(): SessionStateResponse {
        return JSON.parse(localStorage.getItem(currentSessionKey)) as SessionStateResponse;
    }

    setCurrentSession(state: SessionStateResponse) {
        localStorage.setItem(currentSessionKey, JSON.stringify(state));
    }

    resetSession() {
        localStorage.clear();
    }

    getConnection(): signalR.HubConnection {
        if (!this.connection || this.connection.state === signalR.HubConnectionState.Disconnected) {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(`${environment.api}/SessionHub`)
                .withAutomaticReconnect()
                .configureLogging(signalR.LogLevel.Information)
                .build();
        }

        return this.connection;
    }

    getClientRequestDetails(): ClientRequestDetails {
        const hostedSession = this.getHostVariables();
        const joinedSession = this.getJoinVariables();
        return {
            sessionId: hostedSession ? hostedSession.sessionId : joinedSession.sessionId,
            sessionCode: hostedSession ? hostedSession.sessionCode : joinedSession.sessionCode,
            userId: hostedSession ? hostedSession.userId : joinedSession.userId,
            authCode: hostedSession ? hostedSession.authCode : joinedSession.authCode
        } as ClientRequestDetails;
    }

    private inferAndSetCurrentSession(joinedSession: JoinedSession, hostedSession: HostedSession) {
        const session = {
            sessionId: hostedSession ? hostedSession.sessionId : joinedSession.sessionId,
            sessionCode: hostedSession ? hostedSession.sessionCode : joinedSession.sessionCode,
            userId: hostedSession ? hostedSession.userId : joinedSession.userId,
            isHost: hostedSession ? true : false,
            stories: hostedSession ? hostedSession.stories : joinedSession.stories,
            users: hostedSession ? [] : joinedSession.users,
            pointChoices: hostedSession ? hostedSession.pointChoices : joinedSession.pointChoices
        } as SessionStateResponse;

        localStorage.setItem(currentSessionKey, JSON.stringify(session));
    }
}
