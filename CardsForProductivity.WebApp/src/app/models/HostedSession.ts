import { CreateSessionResponse } from './CreateSessionResponse';
import { CreateSessionRequest } from './CreateSessionRequest';

export interface HostedSession extends CreateSessionRequest, CreateSessionResponse { }
